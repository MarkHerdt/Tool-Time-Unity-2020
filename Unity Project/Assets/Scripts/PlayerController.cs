using System.Collections;
using UnityEngine;

namespace ToolTime
{
    public class PlayerController : MonoBehaviour
    {
        Rigidbody rb;
        float hInput;
        public float speed = 1; // Speed the player moves with

        // Level finished?
        bool levelFinished;

        Animator animator;

        public static string repairableObjectTag; // Tag of RepairableObject, the player collides with
        GameObject repairableObject; // Reference of "RepairableObject"
        public GameObject RepairableObject { get => repairableObject; }

        AudioSource source;
        public AudioClip movement;
        bool isPlaying = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();

            source = GetComponent<AudioSource>();
        }

        private void Start()
        {
            // Subsribe to event system
            EventSystemController.self.onGlobalTimerEnd += RoundHasEnded;

            levelFinished = false;
        }

        void RoundHasEnded()
        {
            levelFinished = true;
        }

        private void Update()
        {
            // Movement and animations
            hInput = Input.GetAxisRaw("Horizontal");

            if (!levelFinished)
            {

                rb.velocity = new Vector2(hInput * speed, rb.velocity.y);

                animator.SetBool("IsIdle", (hInput == 0));
                animator.SetBool("IsMovingLeft", (hInput < 0));
                animator.SetBool("IsMovingRight", (hInput > 0));
                animator.SetBool("IsRepairing", Input.GetKey(KeyCode.W));
            }
            else
            {
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsMovingLeft", false);
                animator.SetBool("IsMovingRight", false);
                animator.SetBool("IsRepairing", false);
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            // Collision detection witch RepairableObjects
            if (other.transform.parent.tag == "RepairableObject")
            {
                repairableObjectTag = other.tag;
                repairableObject = other.transform.parent.gameObject;
            }
        }
    }
}
