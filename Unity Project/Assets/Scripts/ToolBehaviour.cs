using UnityEngine;

namespace ToolTime
{
    public class ToolBehaviour : MonoBehaviour
    {
        float hammer = 3.0f;
        float rohrZange = 1.0f;
        float schraubenZieher = 2.0f;
        float maulschluessel = 2.0f;

        bool roundEnded;

        public GameObject spaceBarPrefab;
        GameObject spaceBar;

        public GameObject ui; // Reference for inventory slot
        GameObject tool;

        BuffController buff;

        private void Awake()
        {
            buff = GameObject.Find("Player").GetComponent<BuffController>();   
        }

        private void Start()
        {
            // Subscribe to global events
            EventSystemController.self.onGlobalTimerEnd += OnGlobalTimerEnd;
            roundEnded = false;

            if (ui == null)
            {
                Debug.LogError("ToolBehaviour: Reference for \"ui\" not set in the inspector");
            }
        }

        void OnGlobalTimerEnd()
        {
            roundEnded = true;
            Destroy(spaceBar);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Displays the "SpaceBar" animation, when inside tool trigger
            if (other.CompareTag("Player") && !roundEnded)
            {
                spaceBar = Instantiate(spaceBarPrefab, new Vector3(transform.position.x, -1.125f, -2.75f), Quaternion.identity);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Destroys the "SpaceBar" animation, when the player leaves the tools trigger
            if (other.CompareTag("Player"))
            {
                Destroy(spaceBar);
            }
        }

        /// <summary>
        /// Repairs the broken objects, return value depends on tool equipped and object that need to be repaired
        /// </summary>
        /// <returns></returns>
        public float RepairAmount()
        {
            tool = ui.transform.GetChild(0).gameObject;

            if (tool.transform.GetChild(0).gameObject.tag == "Hammer" && PlayerController.repairableObjectTag == "Riss")
            {
                //Debug.Log("Riss");
                return hammer + buff.Hammer + buff.ToolBuff;
            }
            else if (tool.transform.GetChild(0).gameObject.tag == "RohrZange" && PlayerController.repairableObjectTag == "Rohr")
            {
                //Debug.Log("Rohr");
                return rohrZange + buff.RohrZange + buff.ToolBuff;
            }
            else if (tool.transform.GetChild(0).gameObject.tag == "SchraubenZieher" && PlayerController.repairableObjectTag == "Generator")
            {
                //Debug.Log("Generator");
                return schraubenZieher + buff.SchraubenZieher + buff.ToolBuff;
            }
            else if (tool.transform.GetChild(0).gameObject.tag == "Maulschluessel" && PlayerController.repairableObjectTag == "Sauerstoff")
            {
                //Debug.Log("Sauerstoff");
                return maulschluessel + buff.MaulSchluessel + buff.ToolBuff;
            }

            return 0;
        }
    }
}

