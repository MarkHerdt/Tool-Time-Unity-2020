using System.Collections.Generic;
using UnityEngine;

namespace ToolTime
{
    // Manages the damage system
    public class DamageController : MonoBehaviour
    {
        bool levelFinished;

        public static int id;

        List<GameObject> listOfGoodObjects = new List<GameObject>();
        List<GameObject> listOfBrokenObjects = new List<GameObject>();

        public List<GameObject> ListOfBrokenObjects
        {
            get
            {
                return listOfBrokenObjects;
            }
        }

        float timer;
        float breakObjectIntervall = 5f;

        public float GetTotalDamage
        {
            get
            {
                return (float)listOfBrokenObjects.Count / (listOfGoodObjects.Count + listOfBrokenObjects.Count);
            }
        }

        GameObject player;
        public GameObject exclamationMark;
        float exclamationMarkTimer = 0f;

        AudioSource source;
        public AudioClip alert;

        public delegate void ReturnObject(GameObject obj);
        public static event ReturnObject OnBreak;
        public static event ReturnObject OnRepair;

        private void Awake()
        {
            player = GameObject.FindObjectOfType<PlayerController>().gameObject;

            source = GetComponent<AudioSource>();
        }

        void Start()
        {
            // Subscribe to event system
            EventSystemController.self.onRandomObjectBreaks += OnCountdownTick;
            EventSystemController.self.onObjectRepaired += OnObjectIsRepaired;
            EventSystemController.self.onGlobalTimerEnd += OnGlobalTimerEnd;

            levelFinished = false;

            if (GetAllRepairableObjects().Length > 0)
            {
                foreach (GameObject repairableObject in GetAllRepairableObjects())
                {
                    id++;
                    repairableObject.GetComponent<RepairableObjects>().Id = id; // Gives each object its uniq id
                    listOfGoodObjects.Add(repairableObject);
                }
                //Debug.Log("Zu reparierende Objekte gefunden: " + listOfGoodObjects.Count);
            }
            else
            {
                //Debug.Log("Keine zu reparierenden Objekte gefunden!");
            }
        }

        void OnGlobalTimerEnd()
        {
            levelFinished = true;

            exclamationMark.SetActive(false);

            foreach (GameObject tmp in listOfBrokenObjects)
            {
                tmp.transform.Find("HealthBar").gameObject.SetActive(false);
            }

            EventSystemController.self.ChangeDamageBar(GetTotalDamage);
        }

        void OnObjectIsRepaired()
        {
            EventSystemController.self.BroadcastTotalDamage(GetTotalDamage);
            EventSystemController.self.ChangeDamageBar(GetTotalDamage);
        }

        void OnCountdownTick()
        {
            BreakRandomObject();
            //Debug.Log("DamageController Broadcast: " + (GetTotalDamage * 100));
        }

        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    BreakRandomObject();
            //}
            //else if (Input.GetKeyDown(KeyCode.Y))
            //{
            //    RepairRandomObject();
            //}

            CheckShowExclamationMark();
            //exclamationMark = Instantiate(exclamationMarkPrefab, new Vector3(player.transform.position.x - .15f, player.transform.position.y + 1.25f, player.transform.position.z), Quaternion.identity);

        }

        // Get all repairable objects in the level
        GameObject[] GetAllRepairableObjects()
        {
            GameObject[] repairableObjects;
            repairableObjects = GameObject.FindGameObjectsWithTag("RepairableObject");
            if (repairableObjects.Length > 0)
            {
                return repairableObjects;
            }
            return null;
        }

        /// <summary>
        /// Breaks a random object on the map
        /// </summary>
        public void BreakRandomObject()
        {
            if (listOfGoodObjects.Count > 0)
            {
                // Broadcast Message
                EventSystemController.self.BroadcastTotalDamage(GetTotalDamage);
                EventSystemController.self.ChangeDamageBar(GetTotalDamage);

                GameObject tmp = null;

                // Select random object from the "good" list
                int randomSelection = Random.Range(0, listOfGoodObjects.Count - 1);

                // Set selected object as reference to work with
                tmp = listOfGoodObjects[randomSelection];

                // Move broken object to "broken" list
                listOfBrokenObjects.Add(listOfGoodObjects[randomSelection]);

                // Remove broken object from "good" list
                listOfGoodObjects.RemoveAt(randomSelection);

                tmp.GetComponent<RepairableObjects>().IsBroken = true; // Sets the Objects status to "broken"
                tmp.GetComponent<RepairableObjects>().currentHP = 0; // Sets "hp" to 0
                tmp.transform.GetChild(1).gameObject.SetActive(true); // Activates the "broken" model
                tmp.transform.GetChild(0).gameObject.SetActive(false); // Deactivates the "normal" model
                tmp.transform.Find("HealthBar").gameObject.SetActive(true); // Activates the HealthBar
                OnBreak.Invoke(tmp);

                // Show exclamation mark above player
                ResetShowExclamationMark();
                source.clip = alert;
                source.volume = .5f;
                source.Play();

                DebugShowTotalDamage();

                EventSystemController.self.ChangeDamageBar(GetTotalDamage);

                // Kill copy of broken object
                tmp = null;
            }
        }

        void CheckShowExclamationMark()
        {
            exclamationMark.transform.position = new Vector3(player.transform.position.x - .15f, player.transform.position.y + 1.25f, player.transform.position.z);
            exclamationMark.transform.SetParent(player.transform);

            exclamationMarkTimer -= Time.deltaTime;
            if (exclamationMarkTimer <= 0)
            {
                exclamationMark.SetActive(false);
            }
        }

        void ResetShowExclamationMark()
        {
            exclamationMarkTimer = 2.5f;
            exclamationMark.SetActive(true);
        }

        /// <summary>
        /// Sets a repaired object back to its repaired status
        /// </summary>
        public void RepairObject(int id)
        {
            foreach (GameObject item in listOfBrokenObjects)
            {
                if (item.GetComponent<RepairableObjects>().Id == id)
                {
                    item.GetComponent<RepairableObjects>().IsBroken = false;
                    item.transform.GetChild(0).gameObject.SetActive(true); // Activates the "normal" model
                    item.transform.GetChild(1).gameObject.SetActive(false); // Deactivates the "broken" model
                    item.transform.Find("HealthBar").gameObject.SetActive(false); // Deactivates the HealthBar

                    listOfGoodObjects.Add(item);
                    ListOfBrokenObjects.Remove(item);

                    OnRepair.Invoke(item);

                    EventSystemController.self.ChangeDamageBar(GetTotalDamage);
                    break;
                }
            }
        }

        // Only for debugging!!!!
        void RepairRandomObject()
        {
            if (listOfBrokenObjects.Count > 0)
            {
                // Select random object from the "good" list
                int randomSelection = Random.Range(0, listOfBrokenObjects.Count - 1);

                listOfBrokenObjects[randomSelection].GetComponent<RepairableObjects>().IsBroken = false;
                listOfBrokenObjects[randomSelection].transform.GetChild(1).gameObject.SetActive(true); // Activates the "normal" model
                listOfBrokenObjects[randomSelection].transform.GetChild(2).gameObject.SetActive(false); // Deactivates the "broken" model
                listOfBrokenObjects[randomSelection].transform.Find("HealthBar").gameObject.SetActive(false); // Deactivates the HealthBar

                // Move broken object to "broken" list
                listOfGoodObjects.Add(listOfBrokenObjects[randomSelection]);
                // Remove broken object from "good" list
                listOfBrokenObjects.RemoveAt(randomSelection);

                DebugShowTotalDamage();

                EventSystemController.self.ChangeDamageBar(GetTotalDamage);
            }
        }

        void DebugShowTotalDamage()
        {
            string goodObjects = "";
            string brokenObjects = "";
            // Get all good object names
            if (listOfGoodObjects.Count > 0)
            {
                foreach (GameObject foo in listOfGoodObjects)
                {
                    goodObjects += "" + foo.name + ", ";
                }
                goodObjects = goodObjects.Substring(0, goodObjects.Length - 2);
            }
            // Get all broken object names
            if (listOfBrokenObjects.Count > 0)
            {
                foreach (GameObject foo in listOfBrokenObjects)
                {
                    brokenObjects += "" + foo.name + ", ";
                }
                brokenObjects = brokenObjects.Substring(0, brokenObjects.Length - 2);
            }
        }
    }
}
