using UnityEngine;

namespace Dragon.Core
{
    public static class ChangeEvaluatorFactory<T>
    {
        public static IChangeEvaluator<T> GetChangeEvaluator()
        {
            if (typeof(T) == typeof(float))
            {
                return new CEF_FloatComparer() as IChangeEvaluator<T>;
            }

            if (typeof(T) == typeof(int))
            {
                return new CEF_IntComparer() as IChangeEvaluator<T>;
            }

            if (typeof(T) == typeof(bool))
            {
                return new CEF_BoolComparer() as IChangeEvaluator<T>;
            }
        
            if (typeof(T) == typeof(string))
            {
                return new CEF_StringComparer() as IChangeEvaluator<T>;
            }
        
            if (typeof(T) == typeof(Vector3))
            {
                return new CEF_Vector3Comparer() as IChangeEvaluator<T>;
            }
        
            if (typeof(T) == typeof(Quaternion))
            {
                return new CEF_QuaternionComparer() as IChangeEvaluator<T>;
            }
        
            if (typeof(T) == typeof(Vector2))
            {
                return new CEF_Vector2Comparer() as IChangeEvaluator<T>;
            }

            if (typeof(T).IsClass)
            {
                return new CEF_ReferenceComparer<T>();
            }
        
            if (typeof(T) == typeof(double))
            {
                return new CEF_DoubleComparer() as IChangeEvaluator<T>;
            }

            return new CEF_DefaultComparer<T>();
        }
    }

    public interface IChangeEvaluator<T>
    {
        public bool Equals(T x, T y);
    }

    public class CEF_DefaultComparer<T> : IChangeEvaluator<T>
    {
        public bool Equals(T x, T y)
        {
            return Equals(x, y);
        }
    }

    public class CEF_FloatComparer : IChangeEvaluator<float>
    {
        public bool Equals(float x, float y)
        {
            return x == y;
        }
    }

    public class CEF_IntComparer : IChangeEvaluator<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y;
        }
    }

    public class CEF_BoolComparer : IChangeEvaluator<bool>
    {
        public bool Equals(bool x, bool y)
        {
            return x == y;
        }
    }

    public class CEF_DoubleComparer : IChangeEvaluator<double>
    {
        public bool Equals(double x, double y)
        {
            return x == y;
        }
    }

    public class CEF_StringComparer : IChangeEvaluator<string>
    {
        public bool Equals(string x, string y)
        {
            return x == y;
        }
    }

    public class CEF_Vector3Comparer : IChangeEvaluator<Vector3>
    {
        public bool Equals(Vector3 x, Vector3 y)
        {
            return x == y;
        }
    }

    public class CEF_Vector2Comparer : IChangeEvaluator<Vector2>
    {
        public bool Equals(Vector2 x, Vector2 y)
        {
            return x == y;
        }
    }

    public class CEF_QuaternionComparer : IChangeEvaluator<Quaternion>
    {
        public bool Equals(Quaternion x, Quaternion y)
        {
            return x == y;
        }
    }

    public class CEF_ReferenceComparer<T> : IChangeEvaluator<T>
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }
    }
}