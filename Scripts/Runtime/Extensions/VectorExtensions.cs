using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ToWorldPos(this Vector2 v2)
    {
        return Camera.main.ScreenToWorldPoint(v2);
    }
    public static Vector2 TowardsTarget(this Vector2 v2, Vector2 target, float maxDistance)
    {
        var distance = target - v2;
        return v2 + (distance.normalized * maxDistance);
    }
    public static Vector3 GetSignedPower(this Vector3 original, float xPower = 1, float yPower = 1,
        float zPower = 1)
    {
        var xSign = Mathf.Sign(original.x);
        var ySign = Mathf.Sign(original.y);
        var zSign = Mathf.Sign(original.z);
        Vector3 target =
            new Vector3(
                Mathf.Pow(original.x * xSign, xPower) * xSign,
                Mathf.Pow(original.y * ySign, yPower) * ySign,
                Mathf.Pow(original.z * zSign, zPower) * zSign);

        return target;
    }
    public static Vector3 GetSignedPower(this Vector3 original, float power)
    {
        var xSign = Mathf.Sign(original.x);
        var ySign = Mathf.Sign(original.y);
        var zSign = Mathf.Sign(original.z);
        Vector3 target =
            new Vector3(
                Mathf.Pow(original.x * xSign, power) * xSign,
                Mathf.Pow(original.y * ySign, power) * ySign,
                Mathf.Pow(original.z * zSign, power) * zSign);

        return target;
    }
    public static Vector3 GetClamped(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        float xVal = (x.HasValue) ? Mathf.Clamp(original.x, -x.Value, x.Value) : original.x;
        float yVal = (y.HasValue) ? Mathf.Clamp(original.y, -y.Value, y.Value) : original.y;
        float zVal = (z.HasValue) ? Mathf.Clamp(original.z, -z.Value, z.Value) : original.z;
        return new Vector3(xVal, yVal, zVal);
    }

    public static Vector3 WithRandom(this Vector3 original, float minValue, float maxValue, bool? xyz = null,
        bool? x = null, bool? y = null, bool? z = null)
    {
        float xVal, yVal, zVal;
        if (xyz.HasValue)
        {
            xVal = original.x * Random.Range(minValue, maxValue);
            yVal = original.y * Random.Range(minValue, maxValue);
            zVal = original.z * Random.Range(minValue, maxValue);
        }
        else
        {
            xVal = (x.HasValue) ? original.x * Random.Range(minValue, maxValue) : original.x;
            yVal = (y.HasValue) ? original.y * Random.Range(minValue, maxValue) : original.y;
            zVal = (z.HasValue) ? original.z * Random.Range(minValue, maxValue) : original.z;
        }

        return new Vector3(xVal, yVal, zVal);
    }

    public static Vector3 TowardsTarget(this Vector3 v3, Vector3 target, float maxDistance)
    {
        var distance = target - v3;
        return v3 + (distance.normalized * maxDistance);
    }
    
    public static Quaternion ToQuaternion(this Vector3 euler)
    {
        return Quaternion.Euler(euler);
    }
    
    public static Vector3 ConvertSignedAngles(this Vector3 euler)
    {
        if (euler.x > 180)
        {
            euler.x = euler.x - 360;
        }

        if (euler.y > 180)
        {
            euler.y = euler.y - 360;
        }

        if (euler.z > 180)
        {
            euler.z = euler.z - 360;
        }
        return euler;
    }
    
    public static bool ContainsInBetween(this Vector2Int vector2Int, int value)
    {
        return vector2Int.x <= value && value < vector2Int.y;
    }

    public static bool ContainsInBetween(this Vector2 vector2, int value)
    {
        return vector2.x < value && value < vector2.y;
    }
    
    public static Vector3 MultipliedBy(this Vector3 vector, float value)
    {
        return new Vector3(vector.x * value, vector.y * value, vector.z * value);
    }

    public static Vector3 MultipliedBy(this Vector2 vector, float value)
    {
        return new Vector2(vector.x * value, vector.y * value);
    }
    
    public static Vector2 ToVector2ByXZ(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector2 ToVector2ByXY(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    public static Vector2 ToVector2ByYX(this Vector3 vector3)
    {
        return new Vector2(vector3.y, vector3.x);
    }

    public static Vector2 ToVector2ByYZ(this Vector3 vector3)
    {
        return new Vector2(vector3.y, vector3.z);
    }

    public static Vector2 ToVector2ByZX(this Vector3 vector3)
    {
        return new Vector2(vector3.z, vector3.x);
    }

    public static Vector2 ToVector2ByZY(this Vector3 vector3)
    {
        return new Vector2(vector3.z, vector3.y);
    }

    public static Vector3 ToVector3OnXY(this Vector2 vector2)
    {
        return new Vector3(vector2.x, vector2.y, 0);
    }

    public static Vector3 ToVector3OnXZ(this Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }

    public static float GetRandomFloat(this Vector2 vector2)
    {
        return Random.Range(vector2.x, vector2.y);
    }

    public static Vector3 ToVector3OnZX(this Vector2 vector2)
    {
        return new Vector3(vector2.y, 0, vector2.x);
    }

    public static Vector3 ToVector3OnYX(this Vector2 vector2)
    {
        return new Vector3(vector2.y, vector2.x, 0);
    }

    public static Vector3 ToVector3OnYZ(this Vector2 vector2)
    {
        return new Vector3(0, vector2.x, vector2.y);
    }

    public static Vector3 ToVector3OnZY(this Vector2 vector2)
    {
        return new Vector3(0, vector2.y, vector2.x);
    }
    
    public static Vector3 ScaleWith(this Vector3 vector, Vector3 scaler)
    {
        return Vector3.Scale(vector, scaler);
    }

    public static Vector2 ScaleWith(this Vector2 vector, Vector3 scaler)
    {
        return Vector2.Scale(vector, scaler);
    }

    public static Vector2 WithX(this Vector2 vector, float x)
    {
        return new Vector2(x, vector.y);
    }

    public static Vector2 WithY(this Vector2 vector, float y)
    {
        return new Vector2(vector.x, y);
    }

    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }
    
    public static Vector3 WithAdded(this Vector3 vector, float? addedX = null, float? addedY = null, float? addedZ = null)
    {
        return new Vector3(vector.x + (addedX ?? 0f), vector.y + (addedY ?? 0f), vector.z + (addedZ ?? 0f));
    }
    
    public static Vector3 WithMultiplied(this Vector3 vector, float? mulX = null, float? mulY = null, float? mulZ = null)
    {
        return new Vector3(vector.x * (mulX ?? 1f), vector.y * (mulY ?? 1f), vector.z * (mulZ ?? 1f));
    }

    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }

    public static Vector3 WithY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, y, vector.z);
    }

    public static bool IsZero(this Vector3 vector)
    {
        return vector.x == 0 && vector.y == 0 && vector.z == 0;
    }
    
    public static bool ValidAndNotZero(this Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) && vector.magnitude != 0;
    }
    
    public static Vector3 WithYAdded(this Vector3 vector, float addedY)
    {
        return new Vector3(vector.x, vector.y + addedY, vector.z);
    }

    public static Vector3 AddY(this Vector3 vector, float y)
    {
        return new Vector3(vector.x, vector.y + y, vector.z);
    }
    
    public static Vector3 AddX(this Vector3 vector, float x)
    {
        return new Vector3(vector.x + x, vector.y, vector.z);
    }
    
    public static Vector3 AddZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, vector.z + z);
    }
    
    public static Vector3 SwapYZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.z, vector.y);
    }
    
    public static Vector3 SwapXZ(this Vector3 vector)
    {
        return new Vector3(vector.z, vector.z, vector.x);
    }


    public static Vector3 WithZ(this Vector3 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }
}