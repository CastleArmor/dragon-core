using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Dragon.Core
{
    public static class DCorePhysics
    {
        public static void OwnerIgnoreCollision(List<Collider> ownerCurrentlyIgnored,List<Collider> ownerColliders,List<Collider> newIgnoredColliders)
        {
            //first re enable interaction between currently ignored colliders...
            for (int i = 0; i < ownerCurrentlyIgnored.Count; i++)
            {
                if (ownerCurrentlyIgnored[i] == null)
                {
                    continue;
                }

                foreach (Collider ownerCol in ownerColliders)
                {
                    Physics.IgnoreCollision(ownerCol,ownerCurrentlyIgnored[i],false);
                }
            }
            ownerCurrentlyIgnored.Clear();
        
            for (int i = 0; i < newIgnoredColliders.Count; i++)
            {
                foreach (Collider ownerCol in ownerColliders)
                {
                    Physics.IgnoreCollision(ownerCol,newIgnoredColliders[i],true);
                }
                ownerCurrentlyIgnored.Add(newIgnoredColliders[i]);
            }
        }
        
        public static void ClearOwnerIgnoreCollision(List<Collider> ownerCurrentlyIgnored,List<Collider> ownerColliders)
        {
            //first re enable interaction between currently ignored colliders...
            for (int i = 0; i < ownerCurrentlyIgnored.Count; i++)
            {
                if (ownerCurrentlyIgnored[i] == null)
                {
                    continue;
                }

                foreach (Collider ownerCol in ownerColliders)
                {
                    Physics.IgnoreCollision(ownerCol,ownerCurrentlyIgnored[i],false);
                }
            }
            ownerCurrentlyIgnored.Clear();
        }
        
        public static Vector2 ClampVelocity(Vector2 velocity, float maxSpeed, float slowdownFactor, bool slowWhenNotFacingTarget, Vector2 forward)
        {
            // Max speed to use for this frame
            var currentMaxSpeed = maxSpeed * slowdownFactor;

            // Check if the agent should slow down in case it is not facing the direction it wants to move in
            if (slowWhenNotFacingTarget && (forward.x != 0 || forward.y != 0))
            {
                float currentSpeed;
                var normalizedVelocity = DVectorMath.Normalize(velocity, out currentSpeed);
                float dot = Vector2.Dot(normalizedVelocity, forward);

                // Lower the speed when the character's forward direction is not pointing towards the desired velocity
                // 1 when velocity is in the same direction as forward
                // 0.2 when they point in the opposite directions
                float directionSpeedFactor = Mathf.Clamp(dot + 0.707f, 0.2f, 1.0f);
                currentMaxSpeed *= directionSpeedFactor;
                currentSpeed = Mathf.Min(currentSpeed, currentMaxSpeed);

                // Angle between the forwards direction of the character and our desired velocity
                float angle = Mathf.Acos(Mathf.Clamp(dot, -1, 1));

                // Clamp the angle to 20 degrees
                // We cannot keep the velocity exactly in the forwards direction of the character
                // because we use the rotation to determine in which direction to rotate and if
                // the velocity would always be in the forwards direction of the character then
                // the character would never rotate.
                // Allow larger angles when near the end of the path to prevent oscillations.
                angle = Mathf.Min(angle, (20f + 180f * (1 - slowdownFactor * slowdownFactor)) * Mathf.Deg2Rad);

                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);

                // Determine if we should rotate clockwise or counter-clockwise to move towards the current velocity
                sin *= Mathf.Sign(normalizedVelocity.x * forward.y - normalizedVelocity.y * forward.x);
                // Rotate the #forward vector by #angle radians
                // The rotation is done using an inlined rotation matrix.
                // See https://en.wikipedia.org/wiki/Rotation_matrix
                return new Vector2(forward.x * cos + forward.y * sin, forward.y * cos - forward.x * sin) * currentSpeed;
            }
            else
            {
                return Vector2.ClampMagnitude(velocity, currentMaxSpeed);
            }
        }

        public static Vector3 CalculateDeltaPlacementVelocity(Vector3 oldPosition, Vector3 newPosition, float maxDistance, float speedPower, float maxSpeed)
        {
            Vector3 direction = newPosition - oldPosition;
            float distance = direction.magnitude;

            direction.Normalize();

            float distanceSpeedFactor = Mathf.Clamp(distance / maxDistance, 0, 1);

            float speed = Mathf.Clamp(distanceSpeedFactor * speedPower, 0, maxSpeed);

            return direction * speed;
        }

        public static Vector3 CalculateDeltaPlacementAngularVelocity(Quaternion oldRotation, Quaternion newRotation)
        {
            float angleInDegrees;
            Vector3 rotationAxis;

            //Quaternion deltaDirectionRotation = Quaternion.FromToRotation(transform.forward, direction);

            Quaternion deltaRotation = newRotation * Quaternion.Inverse(oldRotation);

            deltaRotation.ToAngleAxis(out angleInDegrees, out rotationAxis);

            if (angleInDegrees > 180f)
                angleInDegrees -= 360f;

            // Here I drop down to 0.9f times the desired movement,
            // since we'd rather undershoot and ease into the correct angle
            // than overshoot and oscillate around it in the event of errors.

            if (angleInDegrees == 0)
            {
                return Vector3.zero;
            }

            Vector3 angularSpeed = (0.95f * Mathf.Deg2Rad * angleInDegrees / Time.fixedDeltaTime) * rotationAxis.normalized;

            return angularSpeed;
        }

        public static Vector3 CalculateArcStartForce(Vector3 target, Vector3 origin, float time)
        {
            Vector3 distance = target - origin;
            Vector3 distanceXZ = distance;
            distanceXZ.y = 0f;
            float Sy = distance.y;
            float Sxz = distanceXZ.magnitude;
            float Vxz = Sxz / time;
            float Vy = Sy / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;
            Vector3 result = distanceXZ.normalized;
            result *= Vxz;
            result.y = Vy;
            if (distance.z == 0f && distance.y == 0f && distance.x == 0f)
            { result = Vector3.zero; }
            return result;
        }

        public static void GetProjectileLaunchVelocity(Vector3 initialPositionWorld, Vector3 targetPositionWorld, float time, out Vector3 globalVelocity, out Vector3 localVelocity)
        {
            float yInitial = 0;
            float yFinal = initialPositionWorld.y - targetPositionWorld.y;
            float yGravityPos = -Physics.gravity.y;
            float verticalSpeed = (-yFinal - yInitial + 0.5f * yGravityPos * time * time)/time;
            
            Vector3 projectileXZPos = initialPositionWorld.WithY(0);
            Vector3 targetXZPos = targetPositionWorld.WithY(0);
            float flatDistance = Vector3.Distance(projectileXZPos, targetXZPos);
            float horizontalSpeed = flatDistance / time;
            
            Quaternion rotation = Quaternion.LookRotation(projectileXZPos.DirectionTo(targetXZPos), Vector3.up);
            float angle = Mathf.Atan2(verticalSpeed, horizontalSpeed) / Mathf.Rad2Deg;

            // create the velocity vector in local space and get it in global space
            localVelocity = new Vector3(0f, verticalSpeed, horizontalSpeed);
            
            Matrix4x4 projectileTRS = new Matrix4x4();
            projectileTRS.SetTRS(initialPositionWorld, rotation, Vector3.one);
            
            globalVelocity = projectileTRS.MultiplyVector(localVelocity);
        }


        public static Vector3 GetHorizontalVector(Vector3 AtoB, Vector3 gravityBase)
        {
            Vector3 output;
            Vector3 perpendicular = Vector3.Cross(AtoB, gravityBase);
            perpendicular = Vector3.Cross(gravityBase, perpendicular);
            output = Vector3.Project(AtoB, perpendicular);
            return output;
        }

        public static Vector3 GetVerticalVector(Vector3 AtoB, Vector3 gravityBase)
        {
            Vector3 output;
            output = Vector3.Project(AtoB, gravityBase);
            return output;
        }

        public static Vector3[] HitTargetBySpeed(Vector3 startPosition, Vector3 targetPosition, Vector3 gravityBase, float launchSpeed)
        {
            Vector3 AtoB = targetPosition - startPosition;
            Vector3 horizontal = GetHorizontalVector(AtoB, gravityBase);
            float horizontalDistance = horizontal.magnitude;
            Vector3 vertical = GetVerticalVector(AtoB, gravityBase);
            float verticalDistance = vertical.magnitude * Mathf.Sign(Vector3.Dot(vertical, -gravityBase));

            float x2 = horizontalDistance * horizontalDistance;
            float v2 = launchSpeed * launchSpeed;
            float v4 = launchSpeed * launchSpeed * launchSpeed * launchSpeed;

            float gravMag = gravityBase.magnitude;

            float launchTest = v4 - (gravMag * ((gravMag * x2) + (2 * verticalDistance * v2)));

            Vector3[] launch = new Vector3[2];

            if (launchTest < 0)
            {
                launch[0] = (horizontal.normalized * launchSpeed * Mathf.Cos(45.0f * Mathf.Deg2Rad)) - (gravityBase.normalized * launchSpeed * Mathf.Sin(45.0f * Mathf.Deg2Rad));
                launch[1] = (horizontal.normalized * launchSpeed * Mathf.Cos(45.0f * Mathf.Deg2Rad)) - (gravityBase.normalized * launchSpeed * Mathf.Sin(45.0f * Mathf.Deg2Rad));
            }
            else
            {
                float[] tanAngle = new float[2];
                tanAngle[0] = (v2 - Mathf.Sqrt(v4 - gravMag * ((gravMag * x2) + (2 * verticalDistance * v2)))) / (gravMag * horizontalDistance);
                tanAngle[1] = (v2 + Mathf.Sqrt(v4 - gravMag * ((gravMag * x2) + (2 * verticalDistance * v2)))) / (gravMag * horizontalDistance);

                float[] finalAngle = new float[2];
                finalAngle[0] = Mathf.Atan(tanAngle[0]);
                finalAngle[1] = Mathf.Atan(tanAngle[1]);
                launch[0] = (horizontal.normalized * launchSpeed * Mathf.Cos(finalAngle[0])) - (gravityBase.normalized * launchSpeed * Mathf.Sin(finalAngle[0]));
                launch[1] = (horizontal.normalized * launchSpeed * Mathf.Cos(finalAngle[1])) - (gravityBase.normalized * launchSpeed * Mathf.Sin(finalAngle[1]));
            }

            return launch;
        }

        public static Vector3 HitTargetByAngle(Vector3 startPosition, Vector3 targetPosition, Vector3 gravityBase, float limitAngle)
        {
            if (limitAngle == 90 || limitAngle == -90)
            {
                return Vector3.zero;
            }

            Vector3 AtoB = targetPosition - startPosition;
            Vector3 horizontal = GetHorizontalVector(AtoB, gravityBase);
            float horizontalDistance = horizontal.magnitude;
            Vector3 vertical = GetVerticalVector(AtoB, gravityBase);
            float verticalDistance = vertical.magnitude * Mathf.Sign(Vector3.Dot(vertical, -gravityBase));

            float angleX = Mathf.Cos(Mathf.Deg2Rad * limitAngle);
            float angleY = Mathf.Sin(Mathf.Deg2Rad * limitAngle);

            float gravityMag = gravityBase.magnitude;

            if (verticalDistance / horizontalDistance > angleY / angleX)
            {
                return Vector3.zero;
            }

            float destSpeed = (1 / Mathf.Cos(Mathf.Deg2Rad * limitAngle)) * Mathf.Sqrt((0.5f * gravityMag * horizontalDistance * horizontalDistance) / ((horizontalDistance * Mathf.Tan(Mathf.Deg2Rad * limitAngle)) - verticalDistance));
            Vector3 launch = ((horizontal.normalized * angleX) - (gravityBase.normalized * angleY)) * destSpeed;
            return launch;
        }

        public static Vector3 HitTargetAtTime(Vector3 startPosition, Vector3 targetPosition, Vector3 gravityBase, float timeToTarget)
        {
            Vector3 AtoB = targetPosition - startPosition;
            Vector3 horizontal = GetHorizontalVector(AtoB, gravityBase);
            float horizontalDistance = horizontal.magnitude;
            Vector3 vertical = GetVerticalVector(AtoB, gravityBase);
            float verticalDistance = vertical.magnitude * Mathf.Sign(Vector3.Dot(vertical, -gravityBase));

            float horizontalSpeed = horizontalDistance / timeToTarget;
            float verticalSpeed = (verticalDistance + ((0.5f * gravityBase.magnitude) * (timeToTarget * timeToTarget))) / timeToTarget;

            Vector3 launch = (horizontal.normalized * horizontalSpeed) - (gravityBase.normalized * verticalSpeed);
            return launch;
        }
    }
}