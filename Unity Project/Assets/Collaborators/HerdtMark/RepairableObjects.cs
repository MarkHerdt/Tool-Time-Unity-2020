using System.Collections;
using UnityEngine;

namespace HerdtMark
{
    public class RepairableObjects : MonoBehaviour
    {
        int id; // Id given to object, when it breaks
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        DamageController dmgController;

        int repairAmount = 10; // Base amount to repair with
        int skillCheckRepairAmount = 10; // Repair amount, when skillcheck is successfull
        int skillCheckPenaltyAmount = 10; // Damage done to object when skillcheck is missed
        int skillCheckChance = 20; // Percentual chance for a skillcheck to spawn

        public int currentHP = 0; // Current HP of object
        public int maxHP = 100; // Maximum HP of object
        public Transform healthBar; // Reference for Healthbar

        GameObject ui; // Reference for inventory slot
        //--ToolBehaviour tool;

        // Cooldown for repairing objects
        float timestamp = 0;
        int cooldown = 1;
        float time;

        //Skillcheck
        GameObject mainCamera; // Camera reference
        public GameObject skillCheckPrefab; // SkillCheck Prefab
        GameObject skillCheck;
        GameObject check; // Moving part inside skillcheck
        //--SkillCheck skillcheckCS;

        bool isBroken;
        public bool IsBroken
        {
            get
            {
                return isBroken;
            }
            set
            {
                isBroken = value;
            }
        }

        private void Awake()
        {
            dmgController = FindObjectOfType<DamageController>();
            mainCamera = Camera.main.gameObject;
            ui = mainCamera.transform.Find("ToolUI").gameObject;
        }

        private void Start()
        {
            if (ui == null)
            {
                Debug.LogError("RepairableObjects: Reference for \"ui\" not set in the inspector");
            }
            if (healthBar == null)
            {
                Debug.LogError(gameObject.transform.GetChild(0).tag + ": Healthbar not set");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // Increases "hp" each second, "W" is pressed
            if (other.gameObject.tag == "Player" && Input.GetKey(KeyCode.W) && currentHP < maxHP)
            {
                int random = Random.Range(1, 100); // Random number to determine if a skillcheck can spawn
                //--tool = ui.GetComponentInChildren<ToolBehaviour>();

                if (Input.GetKeyDown(KeyCode.W) && timestamp <= Time.time - cooldown)
                {
                    timestamp = Time.time;
                    // Set "time" x-amount smaller then the value in "if (time > x) {}" to get a delay for x seconds when key is pressed
                    // Set them both to the same value to get no delay
                    time = 0;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    time += Time.deltaTime;
                    // Set value x-amount bigger then the value of "timer" in "if (Input.GetKeyDown(KeyCode.Space) && Time.time >= (timeStamp + cooldown)) {}" to get a delay for x seconds when key is pressed
                    // Set them both to the same value to get no delay
                    if (time > 0)
                    {
                        time -= 1;
                        //--
                        //StartCoroutine(HealthBarPlus(repairAmount + (tool != null ? tool.RepairAmount() : 0)));
                        //if (random <= skillCheckChance)
                        //{
                        //    // Only spawns a skillcheck if there is no skillcheck currently active
                        //    if (mainCamera.transform.Find("SkillCheck(Clone)") == null)
                        //    {
                        //        SkillCheck();
                        //    }
                        //}
                        //--
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Destroys the active skillcheck if the player moves out of the objects collider
            if (other.gameObject.tag == "Player" && mainCamera.transform.Find("SkillCheck(Clone)") != null)
            {
                //--skillcheckCS = mainCamera.transform.Find("SkillCheck(Clone)").GetComponentInChildren<SkillCheck>();
                //--skillcheckCS.DestroySkillCheck();
            }
        }

        private void Update()
        {
            healthBar.localScale = new Vector2((float)currentHP / 100, 1);
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
                    dmgController.RepairObject(id);
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


    }
}

