using UnityEngine;

public static class _DCoreMathExtensions
{
    public static float Abs(this float value)
    {
        return Mathf.Abs(value);
    }
        
    public static int RoundToInt(this float value)
    {
        return Mathf.RoundToInt(value);
    }

    public static int CeilToInt(this float value)
    {
        return Mathf.CeilToInt(value);
    }

    public static int FloorToInt(this float value)
    {
        return Mathf.FloorToInt(value);
    }

    public static bool RollChance(this float chance)
    {
        return DMath.RollPercentageChance(chance);
    }

    public static float Clamp(this float value,float min,float max)
    {
        return Mathf.Clamp(value, min, max);
    }

    public static float Clamp01(this float value)
    {
        return Mathf.Clamp01(value);
    }
        
    public static float Map(this float value, float valueMin, float valueMax, float resultMin, float resultMax)
    {
        if (valueMin == valueMax) return resultMin;
        return Mathf.Lerp(resultMin, resultMax, ((value - valueMin) / (valueMax - valueMin)));
    }

    public static Quaternion ToAxisRotation(this float value, Vector3 angleAxis)
    {
        return Quaternion.AngleAxis(value, angleAxis);
    }
}