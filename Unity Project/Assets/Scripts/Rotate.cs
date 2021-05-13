using UnityEngine;

namespace ToolTime
{
    public class Rotate : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate(new Vector3(.5f, .5f, .5f));
        }
    }
}
