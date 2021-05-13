using UnityEngine;

namespace ToolTime
{
    public class PlayerInventory : MonoBehaviour
    {
        // Won't allow picking up items, while the skillcheck is active
        bool skillCheck = false;
        public bool SkillCheck
        {
            get
            {
                return skillCheck;
            }
            set
            {
                skillCheck = value;
            }
        }

        public Transform ui; // Reference for tool position inside "ui"

        Collider other; // Needed, so the collider can be accessed in the "update" method
        GameObject equippedTool;
        Rigidbody toolRB;

        Collider toolCollider;
        float distanceFromPlayer = -1.25f; // Z-position of dropped tools
        public float dropForceX = 1; // Force with which the object is thrown to the side
        public float dropForceY = -1; // Force with which the objects is thrown downwards

        bool roundHasEnded;
        AudioSource source;
        public AudioClip pickUp;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            // Subsribe to event system
            EventSystemController.self.onGlobalTimerEnd += RoundHasEnded;
            roundHasEnded = false;

            if (ui == null)
            {
                Debug.LogError("PlayerInventory: Reference for \"ui\" not set in the inspector");
                if (ui = FindObjectOfType<Transform>())
                {
                    Debug.Log("PlayerInventory: Reference for \"ui\" could be found");
                }
            }
        }

        void RoundHasEnded()
        {
            roundHasEnded = true;
            DropCurrentTool();
        }

        private void OnTriggerStay(Collider other)
        {
            // When the player is inside the trigger of a tool, save its reference in the global "other" variable
            if (other.transform.parent.tag == "Tool")
            {
                this.other = other;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // When the player leaves the trigger of a tool, set the global "other" variable to "null"
            if (other.transform.parent.tag == "Tool")
            {
                this.other = null;
            }
        }

        private void Update()
        {
            if (other != null && Input.GetKeyDown(KeyCode.Space) && skillCheck == false && !roundHasEnded)
            {
                source.clip = pickUp;
                source.PlayOneShot(pickUp);

                // If a tool is already equipped, unequip it and place it infront of the player
                if (equippedTool != null)
                {
                    DropCurrentTool();
                }

                // Equip tool and place it inside the "ui"
                // Removes gravity and sets all constrains to "true", so the object won't fall downwards while inside the UI
                other.attachedRigidbody.useGravity = false;
                other.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                // Places the object inside the UI
                equippedTool = other.transform.parent.gameObject;
                equippedTool.transform.SetParent(ui);

                equippedTool.transform.GetChild(2).gameObject.SetActive(false);
                equippedTool.transform.GetChild(3).gameObject.SetActive(true);

                equippedTool.transform.localScale = new Vector3(.9f, .9f, .9f);
                equippedTool.transform.localPosition = new Vector3(-.0125f, -.025f, 0);
                equippedTool.transform.localRotation = Quaternion.Euler(-5, -15, -25);
            }
        }

        void DropCurrentTool()
        {
            if (equippedTool != null)
            {
                equippedTool.transform.parent = null;
                equippedTool.transform.GetChild(2).gameObject.SetActive(true);
                equippedTool.transform.GetChild(3).gameObject.SetActive(false);
                equippedTool.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                // Sets the gravity and constraints to their original values
                toolCollider = equippedTool.GetComponentInChildren<Collider>();
                toolCollider.attachedRigidbody.useGravity = true;
                toolCollider.attachedRigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                // Drops the object infront of the player
                equippedTool.transform.localPosition = new Vector3(transform.position.x, transform.position.y, distanceFromPlayer);
                toolRB = equippedTool.GetComponent<Rigidbody>();
                int direction = Random.Range(-1, 1); // Throws object to the left if "direction < 0", throws object to the right if "direction >= 0"
                toolRB.transform.localRotation = Quaternion.Euler(0, 0, 0);
                toolRB.AddForce(new Vector3(direction < 0 ? -dropForceX : dropForceX, dropForceY, 0), ForceMode.Impulse);
            }
        }
    }
}
