using UnityEngine;

namespace ToolTime
{
    public class SkillCheckTrigger : MonoBehaviour
    {
        // Background
        SpriteRenderer background;
        float posLeft;
        float posRigth;

        // Trigger
        SpriteRenderer trigger;
        float sizeTrigger;

        private void Awake()
        {
            // Size of background
            background = transform.parent.transform.parent.transform.GetChild(1).GetComponent<SpriteRenderer>();
            posLeft = background.bounds.extents.x;
            posRigth = -background.bounds.extents.x;

            // Size of trigger
            trigger = GetComponent<SpriteRenderer>();
            sizeTrigger = trigger.bounds.size.x;
        }

        private void Start()
        {
            float random = Random.Range(posLeft + (sizeTrigger / 2), posRigth - (sizeTrigger / 2)); // X-position inside "background", mines trigger size
            // Chooses negative/positive value
            int randomDir = Random.Range(0, 1);
            random = randomDir == 0 ? random *= -1 : random *= 1;

            transform.position = new Vector2(transform.position.x + random, transform.position.y);
        }
    }
}
