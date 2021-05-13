using System.Collections;
using UnityEngine;

namespace ToolTime
{
    public class RepairableObjects : MonoBehaviour
    {
        int id; // Id given to object, when it breaks
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        DamageController dmgController;

        float repairAmount = 2.5f; // Base amount to repair with
        float skillCheckRepairAmount = 3.75f; // Repair amount, when skillcheck is successfull
        int skillCheckPenaltyAmount = 15; // Damage done to object when skillcheck is missed
        int skillCheckChance = 25; // Percentual chance for a skillcheck to spawn

        public float currentHP = 0; // Current HP of object
        public float maxHP = 100; // Maximum HP of object
        public Transform healthBar; // Reference for Healthbar

        GameObject w; // "W" indicator above healthbar

        GameObject ui; // Reference for inventory slot
        ToolBehaviour tool;

        // Cooldown for repairing objects
        float timestamp = 0;
        int cooldown = 1;
        float time;

        //Skillcheck
        GameObject mainCamera; // Camera reference
        public GameObject skillCheckPrefab; // SkillCheck Prefab
        GameObject skillCheck;
        GameObject check; // Moving part inside skillcheck
        SkillCheck skillcheckCS;

        bool isBroken;

        public bool IsBroken
        {
            get { return isBroken; }
            set { isBroken = value; }
        }
        GameObject player;
        CameraFollow cameraCS;
        float cameraOffset = 2.5f;

        // Global timer ended
        bool globalTimerEnd;

        bool soundIsPlaying = false;
        AudioSource source;
        public AudioClip hammer;
        public AudioClip rohrZange;
        public AudioClip schraubenZieher;
        public AudioClip maulSchluessel;

        private void Awake()
        {
            dmgController = FindObjectOfType<DamageController>();
            mainCamera = Camera.main.gameObject;
            w = transform.Find("w").gameObject;
            ui = mainCamera.transform.Find("ToolUI").gameObject;
            player = GameObject.FindObjectOfType<PlayerController>().gameObject;
            cameraCS = mainCamera.GetComponent<CameraFollow>();
            source = GetComponent<AudioSource>();

            DamageController.OnBreak += OnBreak;
            DamageController.OnRepair += OnRepair;
        }

        private void Start()
        {
            // Subscribe to global events
            EventSystemController.self.onGlobalTimerEnd += OnGlobalTimerEnd;
            globalTimerEnd = false;

            if (ui == null)
            {
                Debug.LogError("RepairableObjects: Reference for \"ui\" not set in the inspector");
            }
            if (healthBar == null)
            {
                Debug.LogError(gameObject.transform.GetChild(0).tag + ": Healthbar not set");
            }
            SelectSound();
        }

        void OnGlobalTimerEnd()
        {
            globalTimerEnd = true;
            w.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Displays the "W" indicator when the player enters the objects trigger
            if (isBroken && other.CompareTag("Player") && !globalTimerEnd)
            {
                w.SetActive(true);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // Increases "hp" each second, "W" is pressed
            if (other.gameObject.tag == "Player" && Input.GetKey(KeyCode.W) && currentHP < maxHP && !globalTimerEnd)
            {
                int random = UnityEngine.Random.Range(1, 100); // Random number to determine if a skillcheck can spawn
                tool = ui.GetComponentInChildren<ToolBehaviour>();

                if (Input.GetKeyDown(KeyCode.W) && timestamp <= Time.time - cooldown)
                {
                    timestamp = Time.time;
                    // Set "time" x-amount smaller then the value in "if (time > x) {}" to get a delay for x seconds when key is pressed
                    // Set them both to the same value to get no delay
                    time = 0;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    StartCoroutine(PlaySound());
                    time += Time.deltaTime;
                    // Set value x-amount bigger then the value of "timer" in "if (Input.GetKeyDown(KeyCode.Space) && Time.time >= (timeStamp + cooldown)) {}" to get a delay for x seconds when key is pressed
                    // Set them both to the same value to get no delay
                    if (time > 0)
                    {
                        time -= 1;

                        StartCoroutine(HealthBarPlus(repairAmount + (tool != null ? tool.RepairAmount() : 0)));
                        if (random <= skillCheckChance && IsBroken)
                        {
                            // Only spawns a skillcheck if there is no skillcheck currently active
                            if (mainCamera.transform.Find("SkillCheck(Clone)") == null)
                            {
                                SkillCheck();
                            }
                        }
                    }
                }
                if (Input.GetKeyUp(KeyCode.W))
                {
                    source.Stop();
                    StopCoroutine(PlaySound());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Destroys the active skillcheck if the player moves out of the objects collider
            if (other.gameObject.tag == "Player" && mainCamera.transform.Find("SkillCheck(Clone)") != null)
            {
                skillcheckCS = mainCamera.transform.Find("SkillCheck(Clone)").GetComponentInChildren<SkillCheck>();
                skillcheckCS.DestroySkillCheck();
            }

            // Cheks wich arrow to spawn when the player leaves the objects trigger
            if (other.gameObject.CompareTag("Player"))
            {
                SpawnArrow();
            }

            // Hides the "w" indicator when the player leaves the objects trigger
            if (other.CompareTag("Player"))
            {
                w.SetActive(false);
            }
        }

        private void Update()
        {
            healthBar.localScale = new Vector2((float)currentHP / 100, 1);
            if (currentHP >= maxHP)
            {
                dmgController.RepairObject(id);
                // Destroys any active skillcheck if the object reaches its max HP
                if (skillCheck != null)
                {
                    skillcheckCS = mainCamera.transform.Find("SkillCheck(Clone)").GetComponentInChildren<SkillCheck>();
                    skillcheckCS.DestroySkillCheck();
                }
            }
        }

        /// <summary>
        /// Gradually increases the HealthBar
        /// </summary>
        /// <returns></returns>
        IEnumerator HealthBarPlus(float value)
        {
            for (int i = 0; i < value; i++)
            {
                if (currentHP >= maxHP)
                {
                    yield break;
                }
                currentHP++;
                yield return new WaitForSeconds(.01f);
            }
        }

        /// <summary>
        /// Gradually decreases the HealthBar
        /// </summary>
        /// <returns></returns>
        IEnumerator HealthBarMinus(float value)
        {
            for (int i = 0; i < value; i++)
            {
                if (currentHP <= 0)
                {
                    yield break;
                }
                currentHP--;
                yield return new WaitForSeconds(.01f);
            }
        }

        /// <summary>
        /// Instantiates the SkillCheck
        /// </summary>
        void SkillCheck()
        {
            skillCheck = Instantiate(skillCheckPrefab, new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + 1, mainCamera.transform.position.z + 4), Quaternion.identity);
            skillCheck.transform.SetParent(mainCamera.transform);
            check = skillCheck.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject;
        }

        /// <summary>
        /// Increases HP of object if skillcheck succeeded
        /// </summary>
        public void SkillCheckSuccess()
        {
            StartCoroutine(HealthBarPlus(skillCheckRepairAmount));
        }
        /// <summary>
        /// Decreases HP of object if skillcheck is missed
        /// </summary>
        public void SkillCheckMissed()
        {
            StartCoroutine(HealthBarMinus(skillCheckPenaltyAmount));
        }

        void SpawnArrow()
        {
            // When the object is broken
            if (isBroken)
            {
                // When the object is left of the player
                // When the object is inside "arrowRightList", remove it
                //if (Mathf.Abs(transform.position.x - player.transform.position.x))
                if (transform.position.x < player.transform.position.x)
                {
                    //Mathf.Abs(wert1 - wert2)
                    if (cameraCS.arrowRightList.Count > 0)
                    {
                        for (int i = 0; i < cameraCS.arrowRightList.Count; i++)
                        {
                            if (cameraCS.arrowRightList[i] == id)
                            {
                                cameraCS.arrowRightList.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    // Add the oject to "arrowLeftList"
                    bool temp = false;
                    for (int i = 0; i < cameraCS.arrowLeftList.Count; i++)
                    {
                        // When the object already is inside "arrowLeftList", set "temp" to "true"
                        if (cameraCS.arrowLeftList[i] == id)
                        {
                            temp = true;
                        }
                    }
                    // Only adds the object to "arrowLeftList" if "temp" is "false"
                    if (!temp)
                    {
                        cameraCS.arrowLeftList.Add(id);
                    }
                }

                // When the object is right of the player
                // When the object is inside "arrowLeftList", remove it
                if (transform.position.x > player.transform.position.x)
                {
                    if (cameraCS.arrowLeftList.Count > 0)
                    {
                        for (int i = 0; i < cameraCS.arrowLeftList.Count; i++)
                        {
                            if (cameraCS.arrowLeftList[i] == id)
                            {
                                cameraCS.arrowLeftList.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    // Add the oject to "arrowRightList"
                    bool temp = false;
                    for (int i = 0; i < cameraCS.arrowRightList.Count; i++)
                    {
                        // When the object already is inside "arrowRightList", set "temp" to "true"
                        if (cameraCS.arrowRightList[i] == id)
                        {
                            temp = true;
                        }
                    }
                    // Only adds the object to "arrowRightList" if "temp" is "false"
                    if (!temp)
                    {
                        cameraCS.arrowRightList.Add(id);
                    }
                }
            }
        }

        private void OnBreak(GameObject obj)
        {
            SpawnArrow();
        }

        /// <summary>
        /// Remove the repaired object from the arrowList
        /// </summary>
        /// <param name="obj"></param>
        private void OnRepair(GameObject obj)
        {
            // Broadcast that an object is repaired
            EventSystemController.self.ObjectRepaired();

            if (obj.GetComponent<RepairableObjects>().Id == id) // When the repaired object has the same "Id" as "this" object
            {
                for (int i = 0; i < cameraCS.arrowLeftList.Count; i++)
                {
                    if (cameraCS.arrowLeftList[i] == id) // When the object is inside "arrowLeftList"
                    {
                        cameraCS.arrowLeftList.RemoveAt(i);
                        break;
                    }
                }
                for (int i = 0; i < cameraCS.arrowRightList.Count; i++)
                {
                    if (cameraCS.arrowRightList[i] == id) // When the object is inside "arrowRightList"
                    {
                        cameraCS.arrowRightList.RemoveAt(i);
                        break;
                    }
                }
                // Hides the "w" indicator when the object is repaired
                if (w.activeSelf)
                {
                    w.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Selects a sound to play during reparation, depending on the object
        /// </summary>
        void SelectSound()
        {
            source.volume = 1;
            if (gameObject.transform.GetChild(2).CompareTag("Riss"))
            {
                source.clip = hammer;
                source.volume = .25f;
            }
            else if (gameObject.transform.GetChild(2).CompareTag("Rohr"))
            {
                source.clip = rohrZange;
            }
            else if (gameObject.transform.GetChild(2).CompareTag("Generator"))
            {
                source.clip = schraubenZieher;
            }
            else if (gameObject.transform.GetChild(2).CompareTag("Sauerstoff"))
            {
                source.clip = maulSchluessel;
            }
        }

        /// <summary>
        /// Plays 
        /// </summary>
        /// <returns></returns>
        IEnumerator PlaySound()
        {
            if (!soundIsPlaying)
            {
                soundIsPlaying = true;
                source.Play();
                yield return new WaitForSeconds(source.clip.length);
                soundIsPlaying = false;
            }
        }
    }
}

