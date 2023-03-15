using UnityEngine;

namespace Dragon.Core
{
    public static class _DCoreVectorCalculationExtensions
    {
        public static float DistanceTo(this Rigidbody rigidbody, Rigidbody toRigidbody)
        {
            return Vector3.Distance(rigidbody.position, toRigidbody.position);
        }

        public static Vector3 Pos(this Component component)
        {
            return component.transform.position;
        }

        public static Quaternion Rot(this Component component)
        {
            return component.transform.rotation;
        }

        public static float DistanceTo(this Rigidbody rigidbody, Transform transform)
        {
            return Vector3.Distance(rigidbody.position, transform.position);
        }

        public static float DistanceTo(this Transform transform, Rigidbody toRigidbody)
        {
            return Vector3.Distance(transform.position, toRigidbody.position);
        }
    
        public static float DistanceTo(this Component component, Vector3 position)
        {
            return Vector3.Distance(component.transform.position, position);
        }
    
        public static float DistanceTo(this Component component, Component other)
        {
            return Vector3.Distance(component.transform.position, other.transform.position);
        }
    
        public static float DistanceTo(this Component component, Transform other)
        {
            return Vector3.Distance(component.transform.position, other.position);
        }

        public static float DistanceTo(this Transform transform, Transform toTransform)
        {
            return Vector3.Distance(transform.position, toTransform.position);
        }

        public static float DistanceTo(this Vector3 position, Transform toTransform)
        {
            return Vector3.Distance(position, toTransform.position);
        }

        public static float DistanceTo(this Vector3 position, Rigidbody toRigidbody)
        {
            return Vector3.Distance(position, toRigidbody.position);
        }

        public static float DistanceTo(this Rigidbody rigidbody, Vector3 position)
        {
            return Vector3.Distance(rigidbody.position, position);
        }

        public static float DistanceTo(this Transform transform, Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }

        public static float DistanceTo(this Vector3 position, Vector3 toPosition)
        {
            return Vector3.Distance(position, toPosition);
        }

        public static Vector3 DirectionTo(this Vector3 from, Vector3 destination)
        {
            return (destination - from).normalized;
        }
    
        public static Vector3 VectorTo(this Vector3 from, Vector3 destination)
        {
            return (destination - from);
        }

        public static Vector3 Absolute(this Vector3 vector)
        {
            Vector3 newVector = new Vector3(Mathf.Abs(vector.x),Mathf.Abs(vector.y), Mathf.Abs(vector.z));
            return newVector;
        }

        public static float Dot(this Vector3 lhs, Vector3 rhs)
        {
            return Vector3.Dot(lhs, rhs);
        }

        /// <summary>
        /// Negative values means counter-clockwise, positives means clockwise.
        /// A bit heavy.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="desiredRotationAngle"></param>
        /// <param name="clockwiseStartDirection"></param>
        /// <param name="clockwiseEndDirection"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float GetClampedRotationAngle(this Vector3 current, float desiredRotationAngle,
            Vector3 clockwiseStartDirection, Vector3 clockwiseEndDirection, Vector3 axis)
        {
            Vector3 toDir = clockwiseEndDirection;
            Vector3 fromDir = clockwiseStartDirection;
            Vector3 currentDir = current;

            float allowedRotationAngle = desiredRotationAngle;

            if (allowedRotationAngle == 0) return allowedRotationAngle;

            if (allowedRotationAngle > 0)
            {
                float currentAngles = fromDir.ClockwiseRadians3D(currentDir, axis) * Mathf.Rad2Deg;
                float maxAngles = fromDir.ClockwiseRadians3D(toDir, axis) * Mathf.Rad2Deg;

                if (allowedRotationAngle.Abs() > maxAngles - currentAngles)
                {
                    allowedRotationAngle = maxAngles - currentAngles;
                    if (allowedRotationAngle != 0)
                    {
                        allowedRotationAngle -= 0.01f;
                    }
                }
            
                if (maxAngles < currentAngles + allowedRotationAngle)
                {
                    allowedRotationAngle = maxAngles - currentAngles;
                    if (allowedRotationAngle != 0)
                    {
                        allowedRotationAngle -= 0.01f;
                    }
                }

                if (allowedRotationAngle == 0) return 0;
            }
            else
            {
                float currentAngles = toDir.CounterClockwiseRadians3D(currentDir, axis) * Mathf.Rad2Deg;
                float maxAngles = toDir.CounterClockwiseRadians3D(fromDir, axis) * Mathf.Rad2Deg;
        
                if (allowedRotationAngle.Abs() > maxAngles - currentAngles)
                {
                    allowedRotationAngle = maxAngles - currentAngles;
                    if (allowedRotationAngle != 0)
                    {
                        allowedRotationAngle -= 0.01f;
                    }
                    allowedRotationAngle *= -1;
                }
        
                if (maxAngles < currentAngles + allowedRotationAngle.Abs())
                {
                    allowedRotationAngle = maxAngles - currentAngles;
                    if (allowedRotationAngle != 0)
                    {
                        allowedRotationAngle -= 0.01f;
                    }
                    allowedRotationAngle *= -1;
                }
            
                if (allowedRotationAngle == 0) return allowedRotationAngle;
            }

            return allowedRotationAngle;
        }
    
        public static Vector3 RotateClamped(this Vector3 currentDirection, float angle, Vector3 clockwiseStartDirection, Vector3 clockwiseEndDirection, Vector3 normal)
        {
            Vector3 toDir = clockwiseEndDirection;
            Vector3 fromDir = clockwiseStartDirection;
            Vector3 currentDir = currentDirection;
            Vector3 desiredDir = currentDir;

            if (angle == 0) return currentDir;

            if (angle > 0)
            {
                float currentAngles = fromDir.ClockwiseRadians3D(currentDir, normal) * Mathf.Rad2Deg;
                float maxAngles = fromDir.ClockwiseRadians3D(toDir, normal) * Mathf.Rad2Deg;

                if (angle.Abs() > maxAngles - currentAngles)
                {
                    angle = maxAngles - currentAngles;
                    if (angle != 0)
                    {
                        angle -= 0.01f;
                    }
                }
            
                if (maxAngles < currentAngles + angle)
                {
                    angle = maxAngles - currentAngles;
                    if (angle != 0)
                    {
                        angle -= 0.01f;
                    }
                }
            
                if (angle == 0) return currentDir;
            
                Quaternion rotation = Quaternion.AngleAxis(angle, normal);
                desiredDir = rotation * currentDir; 
            }
            else
            {
                float currentAngles = toDir.CounterClockwiseRadians3D(currentDir, normal) * Mathf.Rad2Deg;
                float maxAngles = toDir.CounterClockwiseRadians3D(fromDir, normal) * Mathf.Rad2Deg;
        
                if (angle.Abs() > maxAngles - currentAngles)
                {
                    angle = maxAngles - currentAngles;
                    if (angle != 0)
                    {
                        angle -= 0.01f;
                    }
                    angle *= -1;
                }
        
                if (maxAngles < currentAngles + angle.Abs())
                {
                    angle = maxAngles - currentAngles;
                    if (angle != 0)
                    {
                        angle -= 0.01f;
                    }
                    angle *= -1;
                }
            
                if (angle == 0) return currentDir;
            
                Quaternion rotation = Quaternion.AngleAxis(angle, normal);
                desiredDir = rotation * currentDir; 
            }

            return desiredDir;
        }
    
        public static float CounterClockwiseRadians2D(this Vector2 from, Vector2 to)
        {
            float dot = Vector2.Dot(from,to);
            float det = from.x * to.y - from.y * to.x;
            float angle = Mathf.Atan2(-det, -dot) + Mathf.PI;
            return angle;
        }

        public static float ClockwiseRadians3D(this Vector3 from, Vector3 to, Vector3 normal)
        {
            Vector3 cross = Vector3.Cross(from, to);
            float constDot = Vector3.Dot(normal, cross);
            float sign = Mathf.Sign(constDot);
    
            cross = sign == 0 ? normal : cross * sign;

            Vector3 realFrom = RotateToNewNormal(from, cross, Vector3.up);
            Vector3 realTo = RotateToNewNormal(to, cross, Vector3.up);

            return ClockwiseRadians2D(realFrom.ToVector2ByXZ(), realTo.ToVector2ByXZ());
        }
    
        public static float CounterClockwiseRadians3D(this Vector3 from, Vector3 to, Vector3 normal)
        {
            Vector3 cross = Vector3.Cross(from, to);
            float constDot = Vector3.Dot(normal, cross);
            float sign = Mathf.Sign(constDot);
    
            cross = sign == 0 ? normal : cross * sign;

            Vector3 realFrom = RotateToNewNormal(from, cross, Vector3.up);
            Vector3 realTo = RotateToNewNormal(to, cross, Vector3.up);

            return CounterClockwiseRadians2D(realFrom.ToVector2ByXZ(), realTo.ToVector2ByXZ());
        }

        public static Vector3 RotateToNewNormal(this Vector3 vector, Vector3 currentNormal, Vector3 targetNormal)
        {
            Quaternion rotation = Quaternion.FromToRotation(currentNormal, targetNormal);
            Vector3 rotated = rotation * vector;
            return rotated;
        }

        public static float ClockwiseRadians2D(this Vector2 from, Vector2 to)
        {
            float dot = Vector2.Dot(to,from);
            float det = to.x * from.y - to.y * from.x;
            float angle = Mathf.Atan2(-det, -dot) + Mathf.PI;
            return angle;
        }

        public static Vector3 VectorDistanceTo(this Transform from, Vector3 destination)
        {
            Vector3 directionVector = (destination - from.position);
            return directionVector.Absolute();
        }
    
        /// <summary>
        /// Returns all positive vector to represent distances in each axe.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public static Vector3 VectorDistanceTo(this Transform from, Transform destination)
        {
            Vector3 directionVector = (destination.position - from.position);
            return directionVector.Absolute();
        }
    
        public static Vector3 VectorDistanceTo(this Vector3 from, Transform destination)
        {
            Vector3 directionVector = (destination.position - from);
            return directionVector.Absolute();
        }
    
        public static Vector3 VectorDistanceTo(this Rigidbody from, Vector3 destination)
        {
            Vector3 directionVector = (destination - from.position);
            return directionVector.Absolute();
        }
    
        public static Vector3 VectorDistanceTo(this Rigidbody from, Transform destination)
        {
            Vector3 directionVector = (destination.position - from.position);
            return directionVector.Absolute();
        }
    
        public static Vector3 VectorDistanceTo(this Rigidbody from, Rigidbody destination)
        {
            Vector3 directionVector = (destination.position - from.position);
            return directionVector.Absolute();
        }
    
        public static Vector3 VelocityTo(this Transform fromTransform, Vector3 destination)
        {
            return Vector3.ClampMagnitude(destination - fromTransform.position,fromTransform.DistanceTo(destination));
        }
    
        public static Vector3 VelocityTo(this Vector3 from, Vector3 destination)
        {
            return Vector3.ClampMagnitude(destination - from,from.DistanceTo(destination));
        }
    
        public static Vector3 VelocityTo(this Vector3 from, Transform destination)
        {
            return Vector3.ClampMagnitude(destination.position - from,from.DistanceTo(destination));
        }

        public static Vector3 DirectionTo(this Transform fromTransform, Vector3 destination)
        {
            return (destination - fromTransform.position).normalized;
        }

        public static Vector3 DirectionTo(this Rigidbody fromRigidbody, Vector3 destination)
        {
            return (destination - fromRigidbody.position).normalized;
        }

        public static Vector3 DirectionTo(this Vector3 from, Transform toTransform)
        {
            return (toTransform.position - from).normalized;
        }
    
        public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
        {
            return (to - from).normalized;
        }
    
        public static float DistanceTo(this Vector2 from, Vector2 to)
        {
            return Vector2.Distance(from, to);
        }

        public static Vector3 DirectionTo(this Vector3 from, Rigidbody toRigidbody)
        {
            return (toRigidbody.position - from).normalized;
        }
    
        public static Vector3 DirectionTo(this Transform fromTransform, Transform destinationTransform)
        {
            return (destinationTransform.position - fromTransform.position).normalized;
        }

        public static Vector3 DirectionTo(this Rigidbody fromRigidbody, Transform destinationTransform)
        {
            return (destinationTransform.position - fromRigidbody.position).normalized;
        }

        public static Vector3 Rotate(this Vector3 vector, Vector3 normal, float angle)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, normal);
            Vector3 rotated = rotation * vector;
            return rotated;
        }

        public static Vector3 DirectionTo(this Transform fromTransform, Rigidbody destinationRigidbody)
        {
            return (destinationRigidbody.position - fromTransform.position).normalized;
        }

        public static Vector3 DirectionTo(this Rigidbody fromTransform, Rigidbody destinationRigidbody)
        {
            return (destinationRigidbody.position - fromTransform.position).normalized;
        }
    }
}