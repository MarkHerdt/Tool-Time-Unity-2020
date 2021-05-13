using System.Collections.Generic;
using UnityEngine;

namespace HerdtMark
{
    // Manages the damage system
    public class DamageController : MonoBehaviour
    {
        public static int id;

        List<GameObject> listOfGoodObjects = new List<GameObject>();
        List<GameObject> listOfBrokenObjects = new List<GameObject>();

        float timer;
        float breakObjectIntervall = 5;

        public float GetTotalDamage
        {
            get
            {
                return (float)listOfBrokenObjects.Count / (listOfGoodObjects.Count + listOfBrokenObjects.Count);
            }
        }


        void Start()
        {
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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                //BreakRandomObject();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                //RepairRandomObject();
            }

            timer += Time.deltaTime;
            if (timer >= breakObjectIntervall)
            {
                BreakRandomObject();
                breakObjectIntervall = Random.Range(10, 20);
                timer = 0;
            }
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
        void BreakRandomObject()
        {
            if (listOfGoodObjects.Count > 0)
            {
                // Select random object from the "good" list
                int randomSelection = Random.Range(0, listOfGoodObjects.Count - 1);
                Debug.Log(listOfGoodObjects[randomSelection].name + " ist kaputt gegangen");
                // Move broken object to "broken" list
                listOfBrokenObjects.Add(listOfGoodObjects[randomSelection]);
                // Remove broken object from "good" list
                listOfGoodObjects.RemoveAt(randomSelection);

                listOfBrokenObjects[randomSelection].GetComponent<RepairableObjects>().currentHP = 0; // Sets "hp" to 0
                listOfBrokenObjects[randomSelection].transform.GetChild(2).gameObject.SetActive(true); // Activates the "broken" model
                listOfBrokenObjects[randomSelection].transform.GetChild(1).gameObject.SetActive(false); // Deactivates the "normal" model
                listOfBrokenObjects[randomSelection].transform.Find("HealthBar").gameObject.SetActive(true); // Activates the HealthBar

                DebugShowTotalDamage();

                EventSystemController.self.ChangeDamageBar(GetTotalDamage);

                Debug.Log("Object Broken");
            }
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
                    item.transform.GetChild(1).gameObject.SetActive(true); // Activates the "normal" model
                    item.transform.GetChild(2).gameObject.SetActive(false); // Deactivates the "broken" model
                    item.transform.Find("HealthBar").gameObject.SetActive(false); // Deactivates the HealthBar
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
                Debug.Log(listOfBrokenObjects[randomSelection].name + " ist wieder repariert");
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
                Debug.Log("OK:    " + goodObjects);
            }
            // Get all broken object names
            if (listOfBrokenObjects.Count > 0)
            {
                foreach (GameObject foo in listOfBrokenObjects)
                {
                    brokenObjects += "" + foo.name + ", ";
                }
                brokenObjects = brokenObjects.Substring(0, brokenObjects.Length - 2);
                Debug.Log("Kaputt:    " + brokenObjects);
            }
            // Show total damage
            Debug.Log("Gesamtschaden: " + Mathf.RoundToInt(GetTotalDamage * 100) + "%");
        }
    }
}
