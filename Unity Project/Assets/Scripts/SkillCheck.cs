using UnityEngine;

namespace ToolTime
{
    public class SkillCheck : MonoBehaviour
    {
        bool collision = false; // Collision with "Trigger" inside "SkillCheck"
        PlayerInventory skillcheck; // Won't allow picking up objects, when the skillcheck is active
        PlayerController playerCollision; // Reference of object the player collides with

        BuffController trigger;

        SpriteRenderer spriteRenderer;

        public GameObject skillCheckSuccess;
        public GameObject skillCheckFailed;

        public AudioClip success;
        public AudioClip failure;

        private void Awake()
        {
            skillcheck = GameObject.Find("Player").GetComponent<PlayerInventory>();
            playerCollision = GameObject.Find("Player").GetComponent<PlayerController>();
            trigger = GameObject.Find("Player").GetComponent<BuffController>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            skillcheck.SkillCheck = true;
            gameObject.transform.parent.GetChild(0).transform.localScale = new Vector3(trigger.Skillcheck, gameObject.transform.parent.GetChild(0).transform.localScale.y, gameObject.transform.parent.GetChild(0).transform.localScale.z);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Trigger")
            {
                this.collision = true;
                spriteRenderer.color = Color.green;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Trigger")
            {
                this.collision = true;
                spriteRenderer.color = Color.green;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Trigger")
            {
                this.collision = false;
                spriteRenderer.color = Color.white;
            }
        }

        void Update()
        {
            // When the space key is pressed at the right moment
            if (collision == true && Input.GetKeyDown(KeyCode.Space))
            {
                if (playerCollision.RepairableObject != null)
                {
                    AudioSource.PlayClipAtPoint(success, transform.position);
                    Instantiate(skillCheckSuccess, transform.parent.position, Quaternion.identity);
                    playerCollision.RepairableObject.GetComponent<RepairableObjects>().SkillCheckSuccess();
                }
                skillcheck.SkillCheck = false;
                Destroy(gameObject.transform.parent.transform.parent.gameObject);
            }
            // When the space key is pressed at the wrong moment
            else if (collision == false && Input.GetKeyDown(KeyCode.Space))
            {
                if (playerCollision.RepairableObject != null)
                {
                    AudioSource.PlayClipAtPoint(failure, transform.position);
                    Instantiate(skillCheckFailed, transform.parent.position, Quaternion.identity);
                    playerCollision.RepairableObject.GetComponent<RepairableObjects>().SkillCheckMissed();
                }
                skillcheck.SkillCheck = false;
                Destroy(gameObject.transform.parent.transform.parent.gameObject);
            }
        }

        /// <summary>
        /// Destroys the "SkillCheck" GameObject after one Animation
        /// </summary>
        public void DestroySkillCheck()
        {
            if (playerCollision.RepairableObject != null)
            {
                AudioSource.PlayClipAtPoint(failure, transform.position);
                Instantiate(skillCheckFailed, transform.parent.position, Quaternion.identity);
                playerCollision.RepairableObject.GetComponent<RepairableObjects>().SkillCheckMissed();
            }
            skillcheck.SkillCheck = false;
            Destroy(gameObject.transform.parent.transform.parent.gameObject);
        }
    }
}
