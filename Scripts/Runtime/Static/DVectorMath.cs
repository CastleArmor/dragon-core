using UnityEngine;

namespace Dragon.Core
{
    public static class DVectorMath
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="desiredForward">in world rotation</param>
        /// <param name="customForwardVector">in local rotation</param>
        /// <param name="customUpVector">in world rotation</param>
        /// <returns></returns>
        public static Vector3 CalculateAngularVectorToward(Transform transform, Vector3 desiredForward,
            Vector3? customForwardVector = null, Vector3? customUpVector = null)
        {
            Vector3 upVector;

            if (customUpVector.HasValue && customUpVector.Value != Vector3.zero)
            {
                upVector = customUpVector.Value;
            }
            else
            {
                upVector = Vector3.up;
            }


            float signedAngle =
                DVectorMath.CalculateSignedAngleTowards(transform, desiredForward, customForwardVector, customUpVector:upVector);
            Vector3 rotDiff = signedAngle * upVector;
            /*Debug.Log("forward vector : " + forwardVector + " desired forward : " +
                  desiredForward + " Signed angle : " + signedAngle);*/
#if UNITY_EDITOR
            Debug.DrawRay(transform.position,desiredForward);
#endif
            return rotDiff;
        }
        /// <summary>
        /// returns closest point on a line
        /// </summary>
        /// <param name="linePnt">point the line passes through</param>
        /// <param name="lineDir">unit vector in direction of line, either direction works</param>
        /// <param name="pnt">the point to find nearest on line for</param>
        /// <returns></returns>
        public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
        {
            lineDir.Normalize(); //this needs to be a unit vector
            var v = pnt - linePnt;
            var d = Vector3.Dot(v, lineDir);
            //return linePnt + Vector3.Project(pnt - linePnt, lineDir);
            return linePnt + lineDir * d;
        }
        public static float CalculateSignedAngleTowards(Transform transform, Vector3 desiredForward,
            Vector3? customForwardVector = null, Vector3? customUpVector = null)
        {
            Vector3 forwardVector, upVector;
            if (customForwardVector.HasValue && customForwardVector.Value != Vector3.zero)
            {
                forwardVector = transform.TransformDirection(customForwardVector.Value);
            }
            else
            {
                forwardVector = transform.forward;
            }

            if (customUpVector.HasValue && customUpVector.Value != Vector3.zero)
            {
                upVector = customUpVector.Value;
            }
            else
            {
                upVector = Vector3.up;
            }
            float signedAngle = -Vector3.SignedAngle(desiredForward, forwardVector, upVector);
            return signedAngle;
        }
        /// <summary>
        /// Normalize vector and also return the magnitude.
        /// This is more efficient than calculating the magnitude and normalizing separately
        /// </summary>
        public static Vector3 Normalize(Vector3 v, out float magnitude)
        {
            magnitude = v.magnitude;
            // This is the same constant that Unity uses
            if (magnitude > 1E-05f)
            {
                return v / magnitude;
            }
            else
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Normalize vector and also return the magnitude.
        /// This is more efficient than calculating the magnitude and normalizing separately
        /// </summary>
        public static Vector2 Normalize(Vector2 v, out float magnitude)
        {
            magnitude = v.magnitude;
            // This is the same constant that Unity uses
            if (magnitude > 1E-05f)
            {
                return v / magnitude;
            }
            else
            {
                return Vector2.zero;
            }
        }

        public static Vector3 GetMidPoint(Vector3 pointStart, Vector3 pointEnd)
        {
            return pointStart + (pointEnd - pointStart) / 2;
        }

        public static Vector3 GetMidPoint(params Vector3[] vectors)
        {
            Vector3 totalVector = Vector3.zero;
            foreach(Vector3 vector in vectors)
            {
                totalVector += vector;
            }

            return totalVector / vectors.Length;
        }

        public static Vector3 GetMidPoint(params Component[] components)
        {
            Vector3 totalVector = Vector3.zero;
            foreach (Component component in components)
            {
                if (component == null) continue;
                totalVector += component.transform.position;
            }

            return totalVector / components.Length;
        }

        public static Vector3 GetMidPoint(params GameObject[] gameObjects)
        {
            Vector3 totalVector = Vector3.zero;
            foreach (GameObject go in gameObjects)
            {
                totalVector += go.transform.position;
            }

            return totalVector / gameObjects.Length;
        }
    }
}