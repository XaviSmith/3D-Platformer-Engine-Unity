using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// The bee is more of a hazard than an enemy so pulled it into its own class.
    /// </summary>
    public class BeeEnemy : MonoBehaviour
    {
        [SerializeField] bool showDestinationGizmo;
        [SerializeField] bool moves = true;
        [SerializeField] Vector3 moveTo = Vector3.zero;
        [SerializeField] float moveTime = 1f;
        [SerializeField] float rotationTime = 1f;

        Vector3 startPos;
        Vector3 Destination => returning ? startPos : startPos + moveTo;

        bool returning;

        // Start is called before the first frame update
        void Start()
        {
            if(moves)
            {
                startPos = transform.position;
                StartCoroutine(RotateCoroutine());
            }
            
        }

        IEnumerator RotateCoroutine()
        {
            float timer = 0f;
            Quaternion _rotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(Destination - transform.position);
            while(timer < rotationTime)
            {
                yield return null;

                timer += Time.deltaTime;

                //Vector3 newDirection = Vector3.RotateTowards(transform.forward, destination, timer);
                transform.rotation = Quaternion.Lerp(_rotation, targetRotation, timer/rotationTime);
            }
       
            StartCoroutine(MoveCoroutine());
        }

        IEnumerator MoveCoroutine()
        {
            float timer = 0f;
            Vector3 origin = transform.position;

            while(timer < moveTime)
            {
                yield return null;
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(origin, Destination, timer / moveTime);
            }

            returning = !returning;

            StartCoroutine(RotateCoroutine());
        }

        private void OnDrawGizmos()
        {
            if(showDestinationGizmo && moves)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawLine(transform.position, transform.position + moveTo);
            }
        }
    }
}

