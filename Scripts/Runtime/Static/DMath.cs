using UnityEngine;

namespace Dragon.Core
{
    public static class DMath
    {
        public static bool RollPercentageChance(float chance)
        {
            float random = Random.Range(0f, 1f);
            return random <= chance;
        }
        
        public static Vector3 RotateTowards(Quaternion currentRotation,Vector3 targetDirection,Vector3 characterUp, Vector3 characterForward,float rotateRate,bool isPlanar = true)
        {
            Vector3 worldDirection = targetDirection;
            if (isPlanar)
                worldDirection = Vector3.ProjectOnPlane(worldDirection,characterUp);

            if (worldDirection.IsZero())
                return worldDirection;

            Quaternion targetRotation = Quaternion.LookRotation(worldDirection, characterUp);

            Quaternion outputRotation = Quaternion.Slerp(currentRotation, targetRotation, rotateRate * Mathf.Deg2Rad * Time.deltaTime);
            worldDirection = outputRotation * Vector3.forward;

            return worldDirection;
        }
        
        public static Matrix4x4 CreateDirectionTransformer(Vector3 worldDirection)
        {
            Quaternion lookRotation = Quaternion.LookRotation(worldDirection, Vector3.up);
            Matrix4x4 lookMatrix = new Matrix4x4();
            lookMatrix.SetTRS(Vector3.zero,lookRotation,Vector3.one);
            return lookMatrix;
        }
        
        public static Quaternion QuaternionDifference(Quaternion from, Quaternion to)
        {
            return from * Quaternion.Inverse(to);
        }
        
        public static Quaternion PlaneRotation(Vector3 normal)
        {
            return Quaternion.AngleAxis(0, normal);
        }
        
        public static Vector2 Quadratic(float a, float b, float c)
        {
            float sqrtpart = b * b - 4 * a * c;
            float x, x1, x2, img;
            if (sqrtpart > 0)
            {
                x1 = (-b + Mathf.Sqrt(sqrtpart)) / (2 * a);
                x2 = (-b - Mathf.Sqrt(sqrtpart)) / (2 * a);
                return new Vector2(x1,x2);
            }
            else if (sqrtpart < 0)
            {
                sqrtpart = -sqrtpart;
                x = -b / (2 * a);
                img = Mathf.Sqrt(sqrtpart) / (2 * a);
                return new Vector2(x,x);
            }
            else
            {
                x = (-b + Mathf.Sqrt(sqrtpart)) / (2 * a);
           
                return new Vector2(x,x);
            }
        }

        public static bool IsCachedTimePassedThreshold(float cachedStartTime, float currentTime, float waitTime)
        {
            return (currentTime - cachedStartTime) >= waitTime;
        }
        
        public static Vector3 CalculateMovingTargetProjectileAimPosition(Vector3 projStartPos, Vector3 targetVelocity, Vector3 targetExactPos, float launchSpeed)
        {
            float projExactTravelDistance = projStartPos.DistanceTo(targetExactPos);
            float projAirTime = projExactTravelDistance / launchSpeed;
            Vector3 targetDisp = projAirTime * targetVelocity;
            Vector3 targetHypoPos = targetDisp + targetExactPos;
            Vector3 projHypoDisp = targetHypoPos - projStartPos;
            Vector3 projExactDisp = targetExactPos - projStartPos;
            float projHypoExactDistanceDiff = projHypoDisp.magnitude - projExactDisp.magnitude;
            float projHypoExactTimeDiff = projAirTime * projHypoExactDistanceDiff / projExactTravelDistance;
            Vector3 excessDisp = projHypoExactTimeDiff * targetVelocity;
            Vector3 aimPos = targetExactPos + excessDisp + targetDisp;
            return aimPos;
        }
        
        public static Vector3 CalculateMovingTargetProjectileAimPosition(Vector3 projStartPos, Vector3 targetVelocity, Vector3 targetExactPos, float launchSpeed,float gravity)
        {
            CalculateTrajectory(projStartPos, targetExactPos, launchSpeed, gravity, out float launchAngle);
            float lateralLaunchSpeed = Mathf.Cos(Mathf.Deg2Rad * launchAngle) * launchSpeed;

            bool lateralSpeedIsValid = !float.IsNaN(lateralLaunchSpeed) || lateralLaunchSpeed != 0;
            float validLateralLaunchSpeed = (lateralSpeedIsValid ? lateralLaunchSpeed : 1f);

            float projExactTravelDistance = projStartPos.DistanceTo(targetExactPos);
            float projAirTime = projExactTravelDistance / validLateralLaunchSpeed;
            Vector3 targetDisp = projAirTime * targetVelocity;
            Vector3 targetHypoPos = targetDisp + targetExactPos;
            Vector3 projHypoDisp = targetHypoPos - projStartPos;
            Vector3 projExactDisp = targetExactPos - projStartPos;
            float projHypoExactDistanceDiff = projHypoDisp.magnitude - projExactDisp.magnitude;
            float projHypoExactTimeDiff = projAirTime * projHypoExactDistanceDiff / projExactTravelDistance;
            Vector3 excessDisp = projHypoExactTimeDiff * targetVelocity;
            Vector3 aimPos = targetExactPos + excessDisp + targetDisp;
            
            CalculateTrajectory(projStartPos, aimPos, launchSpeed, gravity, out launchAngle);
            
            Quaternion lookRotation = Quaternion.Euler(-launchAngle,0,0);

            return projStartPos+(lookRotation*projStartPos.DirectionTo(aimPos).WithY(0))*5f;
        }
        
        /// <summary>
        /// Calculate the launch angle. Doesn't take drag into account.
        /// </summary>
        /// <returns>Angle to be fired on in Degrees.</returns>
        /// <param name="start">The muzzle.</param>
        /// <param name="end">Wanted hit point.</param>
        /// <param name="gravity">Gravity, generally set to Physics.gravity.</param>
        /// <param name="muzzleVelocity">Muzzle velocity.</param>
        public static bool CalculateTrajectory(Vector3 start, Vector3 end, float muzzleVelocity, float gravity,out float angle){//, out float highAngle){
 
            Vector3 dir = end - start;
            float vSqr = muzzleVelocity * muzzleVelocity;
            float y = dir.y;
            dir.y = 0.0f;
            float x = dir.sqrMagnitude;
            float g = -gravity;
 
            float uRoot = vSqr * vSqr - g * (g * (x) + (2.0f * y * vSqr));
 
 
            if (uRoot < 0.0f) {
 
                //target out of range.
                angle = -45.0f;
                //highAngle = -45.0f;
                return false;
            }
 
            //        float r = Mathf.Sqrt (uRoot);
            //        float bottom = g * Mathf.Sqrt (x);
 
            angle = -Mathf.Atan2 (g * Mathf.Sqrt (x), vSqr + Mathf.Sqrt (uRoot)) * Mathf.Rad2Deg;
            //highAngle = -Mathf.Atan2 (bottom, vSqr - r) * Mathf.Rad2Deg;
            return true;
 
        }
 
        /// <summary>
        /// Gets the ballistic path.
        /// </summary>
        /// <returns>The ballistic path.</returns>
        /// <param name="startPos">Start position.</param>
        /// <param name="forward">Forward direction.</param>
        /// <param name="velocity">Velocity.</param>
        /// <param name="gravity">If not set, it will be 0.</param>
        /// <param name="timeResolution">Time from frame to frame.</param>
        /// <param name="maxTime">Max time to simulate, will be clamped to reach height 0 (aprox.).</param>
 
        public static Vector3[] GetBallisticPath(Vector3 startPos, Vector3 forward, float velocity, float timeResolution,float gravity = 0f,float maxTime = Mathf.Infinity){
 
            maxTime = Mathf.Min (maxTime, GetTimeOfFlight (velocity, Vector3.Angle (forward, Vector3.up) * Mathf.Deg2Rad, startPos.y,gravity));
            Vector3[] positions = new Vector3[Mathf.CeilToInt(maxTime / timeResolution)];
            Vector3 velVector = forward * velocity;
            int index = 0;
            Vector3 curPosition = startPos;
            for (float t = 0.0f; t < maxTime; t += timeResolution) {
           
                if (index >= positions.Length)
                    break;//rounding error using certain values for maxTime and timeResolution
           
                positions [index] = curPosition;
                curPosition += velVector * timeResolution;
                velVector += Physics.gravity * timeResolution;
                index++;
            }
            return positions;
        }
 
        public static Vector3 GetHitPosition(Vector3 startPos, Vector3 forward, float velocity){
 
            Vector3[] path = GetBallisticPath (startPos, forward, velocity, .35f);
            RaycastHit hit;
            for (int i = 1; i < path.Length; i++) {
 
                //Debug.DrawRay (path [i - 1], path [i] - path [i - 1], Color.red, 10f);
                if (Physics.Raycast (path [i - 1], path [i] - path [i - 1], out hit, (path [i] - path [i - 1]).magnitude)) {
                    return hit.point;
                }
            }
 
            return Vector3.zero;
        }
       
 
        public static float CalculateMaxTrajectoryRange(float muzzleVelocity,float gravity){
            return (muzzleVelocity * muzzleVelocity) / -gravity;
        }
 
        public static float GetTimeOfFlight(float vel, float angle, float height,float gravity){
 
            return (2.0f * vel * Mathf.Sin (angle)) / -gravity;
        }
        
        public static Vector3 RandomVector3(Bounds bounds, Transform referencedTransform = null)
        {
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 position = new Vector3(x, y, z);

            Vector3 worldPosition = position;

            return worldPosition;
        }

        public static Vector3 RandomVector3(Vector2 xRange, Vector2 yRange, Vector2 zRange, Transform referencedTransform = null)
        {
            float x = Random.Range(xRange.x, xRange.y);
            float y = Random.Range(yRange.x, yRange.y);
            float z = Random.Range(zRange.x, zRange.y);

            Vector3 position = new Vector3(x, y, z);

            Vector3 worldPosition = position;

            if (referencedTransform != null)
            {
                worldPosition = referencedTransform.TransformPoint(position);
            }

            return worldPosition;
        }

        public static bool RandomBool()
        {
            int rand = Random.Range(0, 2);
            return rand == 1;
        }
        
        public static float GetPercentage(float min, float max, float value)
        {
            if (min == 0 && max == 0) return 1f;
            return (value - min) / (max - min);
        }

        /// <summary>Maps a value between startMin and startMax to be between targetMin and targetMax</summary>
        public static float MapTo(float startMin, float startMax, float targetMin, float targetMax, float value)
        {
            return Mathf.Lerp(targetMin, targetMax, Mathf.InverseLerp(startMin, startMax, value));
        }


        /// <summary>
        /// ax^2 + bx + c
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float QuadriaticPlus(float a, float b, float c)
        {
            return -b + Mathf.Sqrt(b * b - 4 * a * c) / 2 * a;
        }

        /// <summary>
        /// ax^2 + bx + c
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float QuadriaticNeg(float a, float b, float c)
        {
            return -b - Mathf.Sqrt(b * b - 4 * a * c) / 2 * a;
        }

        public static int Sigma(int amount)
        {
            return amount * (amount + 1) / 2;
        }

        public static float ClampMin(float target, float minimum)
        {
            if(target < minimum)
            {
                return minimum;
            }
            else
            {
                return target;
            }
        }

        public static float ClampMax(float target, float maximum)
        {
            if (target > maximum)
            {
                return maximum;
            }
            else
            {
                return target;
            }
        }

        public static int ClampMin(int target, int minimum)
        {
            if (target < minimum)
            {
                return minimum;
            }
            else
            {
                return target;
            }
        }

        public static int ClampMax(int target, int maximum)
        {
            if (target > maximum)
            {
                return maximum;
            }
            else
            {
                return target;
            }
        }
    }
}