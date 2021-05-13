using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolTime
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target; // Reference for object to follow
        public float smoothSpeed = 1; // Delay before camera starts to move
        public float cameraYOffset = 0; // Offset for camera on the y-axis

        float x;

        bool roundEnded;

        public GameObject toolUI;
        public GameObject arrowLeft;
        public GameObject arrowRight;
        public List<int> arrowLeftList = new List<int>();
        public List<int> arrowRightList = new List<int>();

        private void Start()
        {
            // Subscribe to event list
            EventSystemController.self.onGlobalTimerEnd += GlobalTimerEnd;
            roundEnded = false;

            if (target == null)
            {
                Debug.LogError("CameraFollow: Reference for \"target\" not set in the inspector");
            }
        }

        void GlobalTimerEnd()
        {
            roundEnded = true;

            arrowLeftList.Clear();
            arrowLeft.SetActive(false);

            arrowRightList.Clear();
            arrowRight.SetActive(false);

            toolUI.SetActive(false);
        }

        private void Update()
        {
            if (target.position.x < 4)
            {
                x = 4;
            }
            else if (target.position.x > 37)
            {
                x = 37;
            }
            else
                x = target.position.x;

            if (arrowLeftList.Count > 0)
            {
                arrowLeft.SetActive(true);
            }
            else
                arrowLeft.SetActive(false);
            if (arrowRightList.Count > 0)
            {
                arrowRight.SetActive(true);
            }
            else
                arrowRight.SetActive(false);
        }

        private void LateUpdate()
        {
            if (target == null) return; // Won't move camera if no target is set to follow
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(x, target.position.y + cameraYOffset, transform.position.z), smoothSpeed * Time.deltaTime);
        }
    }
}
