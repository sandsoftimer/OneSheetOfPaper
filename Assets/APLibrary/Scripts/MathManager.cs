/*
 * Developer E-mail: sandsoftimer@gmail.com
 * Facebook Account: https://www.facebook.com/md.imran.hossain.902
 * This is a manager which will give common functional supports. 
 * like Math Calculator.
 *  
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace com.alphapotato.utility
{
    public class MathManager : MonoBehaviour
    {
        public Vector3 CenterOfTransform(Transform[] _transform)
        {
            Vector3[] vectors = new Vector3[_transform.Length];
            int index = 0;
            foreach (Transform item in _transform)
            {
                vectors[index++] = item.position;
            }
            return CenterOfVectors(vectors);
        }

        public Vector3 CenterOfVectors(Vector3[] vectors)
        {
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Length;
        }

        // Genarate Teajectory points
        public Vector3[] GetParabolaPoints(Rigidbody rigidbody, Vector3 pos, Vector3 velocity, int steps)
        {
            Vector3[] results = new Vector3[steps];

            float timestep = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
            Vector3 gravityAccel = Physics.gravity * 1 * timestep * timestep;
            float drag = 1f - timestep * rigidbody.drag;
            Vector3 moveStep = velocity * timestep;

            for (int i = 0; i < steps; ++i)
            {
                moveStep += gravityAccel;
                moveStep *= drag;
                pos += moveStep;
                results[i] = pos;
            }

            return results;
        }

        public Vector3 GetVelocityForThisPoint(Vector3 startPosition, Vector3 endPosition, float angle)
        {
            Vector3 direction = endPosition - startPosition;
            float h = direction.y;
            direction.y = 0;
            float distance = direction.magnitude;
            float a = angle * Mathf.Deg2Rad;
            direction.y = distance * Mathf.Tan(a);
            distance += h / Mathf.Tan(a);

            // calculate velocity
            float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
            return velocity * direction.normalized;
        }

        public Vector3 GetForceForThisPoint(Vector3 startPosition, Vector3 endPosition, float angle, float mass)
        {
            return GetVelocityForThisPoint(startPosition, endPosition, angle) * mass / Time.fixedDeltaTime;
        }

        // Generate random normalized direction
        public static Vector2 GetRandomDirection2D()
        {
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDirection3D()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        public Vector3 GetDirectionToPosition(Vector3 fromPosition)
        {
            Vector3 mouseWorldPosition = GetWorldTouchPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }

        public Vector3 GetWorldTouchPosition()
        {
            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                return ray.GetPoint(distance);
            }

            return Vector3.zero;
        }

        //public Vector3 GetWorldTouchPosition(GameObject collider)
        //{
        //    RaycastHit hit;
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out hit, 1<< col))
        //    {
        //        return hit.point;
        //    }

        //    return Vector3.zero;
        //}

        public Vector3 GetWorldTouchPosition(GameObject go)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1 << go.layer))
            {
                return hit.point;
            }

            return go.transform.position;
        }

        public float WrapAngle(float angle)
        {
            angle %= 360;
            return angle = angle > 180 ? angle - 360 : angle;
        }

        public float UnwrapAngle(float angle)
        {
            if (angle >= 0)
                return angle;

            angle = -angle % 360;

            return 360 - angle;
        }

        // Is Mouse over a UI Element? Used for ignoring World clicks through UI
        public bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            else
            {
                PointerEventData pe = new PointerEventData(EventSystem.current);
                pe.position = Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pe, hits);
                return hits.Count > 0;
            }
        }

    }
}
