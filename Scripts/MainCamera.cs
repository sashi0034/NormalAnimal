using UnityEngine;

namespace MyProjects.Scripts
{
    public class MainCamera : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MovePos(Vector3 deltaPos)
        {
            transform.position  += deltaPos;
        }

        public void RotateAround(Vector3 centerPos, float deg)
        {
            transform.RotateAround(centerPos, Vector3.up, deg);
        }
    }
}