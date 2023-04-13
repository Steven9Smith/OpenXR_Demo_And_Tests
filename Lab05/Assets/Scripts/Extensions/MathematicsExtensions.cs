using Unity.Mathematics;
using UnityEngine;

namespace Egg.Extensions.Mathematics
{
    public class MathematicsExtensions
    {
        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angles)
        {
            return angles * (point - pivot) + pivot;
        }

        public static float ConvertToPersicion(float value, int percision)
        {
            return math.round(value * math.pow(10, percision - 1)) / math.pow(10, percision - 1);
        }

        public static float3 ConvertToPersicion(float3 value, int percision)
        {
            return math.round(value * math.pow(10, percision - 1)) / math.pow(10, percision - 1);
        }
        public static Quaternion ClampRotation(Quaternion q, Vector3 bounds)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
            angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

            float angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
            angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);

            return q.normalized;
        }
        public static quaternion ClampRotation(quaternion q, float3 minBounds, float3 maxBounds)
        {
            Quaternion Q = q;

        //    Debug.Log("clamp x; "+ math.clamp(Q.eulerAngles.x, minBounds.x, maxBounds.x)+", "+minBounds.x+", "+maxBounds.x
        //        + "\nclamp y; "+ math.clamp(Q.eulerAngles.y, minBounds.y, maxBounds.y)+", "+minBounds.y+", "+maxBounds.y
        //        + "\nclamp z; "+ math.clamp(Q.eulerAngles.z, minBounds.z, maxBounds.z)+", "+minBounds.z+", "+maxBounds.z);
            Q.eulerAngles = new Vector3
            {
                x = math.clamp(Q.eulerAngles.x, minBounds.x, maxBounds.x),
                y = math.clamp(Q.eulerAngles.y, minBounds.y, maxBounds.y),
                z = math.clamp(Q.eulerAngles.z, minBounds.z, maxBounds.z)
            };

            return Q;
/*
            float3 eulerAngles = Transforms.TransformsExtensions.ToEuler(q);
            return quaternion.EulerXYZ(new float3
            {
                x = math.clamp(eulerAngles.x, minBounds.x, maxBounds.x),
                y = math.clamp(eulerAngles.y, minBounds.y, maxBounds.y),
                z = math.clamp(eulerAngles.z, minBounds.y, maxBounds.z)
            });*/
        }
        /// <summary>
        /// returns true if x is greater than a and less than b
        /// </summary>
        /// <param name="x">value to check</param>
        /// <param name="a">a bound</param>
        /// <param name="b">another bound</param>
        /// <returns></returns>
        public static bool InRange(float x, float a, float b)
        {
            return x >= a && x <= b;
        }

        public static T GetValue<T>(byte internalIndex, float4 Value)
        {
            switch (internalIndex)
            {
                case 0:
                    if (typeof(T) == typeof(float)) return Convert<T>(Value.x);
                    else return Null<T>();
                case 1:
                    if (typeof(T) == typeof(float)) return Convert<T>(Value.y);
                    else return Null<T>();
                case 2:
                    if (typeof(T) == typeof(float)) return Convert<T>(Value.z);
                    else return Null<T>();
                case 3:
                    if (typeof(T) == typeof(float)) return Convert<T>(Value.w);
                    else return Null<T>();
                case 4:
                    if (typeof(T) == typeof(int)) return Convert<T>(Value.x);
                    else return Null<T>();
                case 5:
                    if (typeof(T) == typeof(int)) return Convert<T>(Value.y);
                    else return Null<T>();
                case 6:
                    if (typeof(T) == typeof(int)) return Convert<T>(Value.z);
                    else return Null<T>();
                case 7:
                    if (typeof(T) == typeof(int)) return Convert<T>(Value.w);
                    else return Null<T>();
                case 8:
                    if (typeof(T) == typeof(bool)) return Convert<T>(Value.x);
                    else return Null<T>();
                case 9:
                    if (typeof(T) == typeof(bool)) return Convert<T>(Value.y);
                    else return Null<T>();
                case 10:
                    if (typeof(T) == typeof(bool)) return Convert<T>(Value.z);
                    else return Null<T>();
                case 11:
                    if (typeof(T) == typeof(bool)) return Convert<T>(Value.w);
                    else return Null<T>();

                default: return Null<T>();
            }
        }

        private static T Convert<T>(float Value)
        {
            if (typeof(T) == typeof(int)) return (T)((object)(int)Value);
            else if (typeof(T) == typeof(bool)) return (T)((object)(Value == 1 ? true : false));
            else if (typeof(T) == typeof(float)) return (T)((object)Value);
            else if (typeof(T) == typeof(double)) return (T)((object)(double)Value);
            else return Null<T>(); //     return (T)System.Convert.ChangeType(obj, typeof(T));
        }

        private static T Convert<T>(float2 Value)
        {
            if (typeof(T) == typeof(int2)) return (T)((object)(int2)Value);
            else if (typeof(T) == typeof(bool2)) return (T)((object)new bool2(Value.x == 1 ? true : false, Value.y == 1 ? true : false));
            else if (typeof(T) == typeof(float2)) return (T)((object)Value);
            else if (typeof(T) == typeof(double2)) return (T)((object)(double2)Value);
            else return Null<T>();
        }

        private static T Convert<T>(float3 Value)
        {
            if (typeof(T) == typeof(int3)) return (T)((object)(int3)Value);
            else if (typeof(T) == typeof(bool3)) return (T)((object)new bool3(Value.x == 1 ? true : false, Value.y == 1 ? true : false, Value.z == 1 ? true : false));
            else if (typeof(T) == typeof(float3)) return (T)((object)Value);
            else if (typeof(T) == typeof(double3)) return (T)((object)(double3)Value);
            else return Null<T>();
        }

        private static T Convert<T>(float4 Value)
        {
            if (typeof(T) == typeof(int4)) return (T)((object)(int4)Value);
            else if (typeof(T) == typeof(bool4)) return (T)((object)new bool4(Value.x == 1 ? true : false, Value.y == 1 ? true : false, Value.z == 1 ? true : false, Value.w == 1 ? true : false));
            else if (typeof(T) == typeof(float4)) return (T)((object)Value);
            else if (typeof(T) == typeof(double4)) return (T)((object)(double4)Value);
            else return Null<T>();
        }

        private static T Null<T>() { return (T)(new object()); }
        // Vector3.MoveTowards
        public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
        {
            float3 a = target - current;
            float magnitude = math.length(a);//.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                return target;
            }
            return current + a / magnitude * maxDistanceDelta;
        }
        // Mathf.MoveTowards
        public static float MoveTowards(float current, float target, float maxDelta)
        {
            if (math.abs(target - current) <= maxDelta)
            {
                return target;
            }
            return current + math.sign(target - current) * maxDelta;
        }
    }
    /*
    public class Noise
    {
        /// <summary>
        /// Type of Noise
        /// </summary>
        public enum NoiseType
        {
            //null type
            Null,
            // Cellular noise, returning F1 and F2 in a float2.
            // Standard 3x3 search window for good F1 and F2 values
            CellularNoise,
            // Cellular noise, returning F1 and F2 in a float2.
            // Speeded up by umath.sing 2x2 search window instead of 3x3,
            // at the expense of some strong pattern artifacts.
            // F2 is often wrong and has sharp discontinuities.
            // If you need a smooth F2, use the slower 3x3 version.
            // F1 is sometimes wrong, too, but OK for most purposes.
            // TL;DR - Faster at the cost of accuracy and artifacts
            CellularNoise2x2,
            // Cellular noise, returning F1 and F2 in a float2.
            // Speeded up by umath.sing 2x2x2 search window instead of 3x3x3,
            // at the expense of some pattern artifacts.
            // F2 is often wrong and has sharp discontinuities.
            // If you need a good F2, use the slower 3x3x3 version.
            CellularNoise2x2x2,
            // Cellular noise, returning F1 and F2 in a float2.
            // 3x3x3 search region for good F2 everywhere, but a lot
            // slower than the 2x2x2 version.
            // The code below is a bit scary even to its author,
            // but it has at least half decent performance on a
            // math.modern GPU. In any case, it beats any software
            // implementation of Worley noise hands down.
            CellularNoise3x3x3,
            // Classic Perlin noise
            ClassicPerlinNoise,
            // Classic Perlin noise
            ClassicPerlinNoise3x3x3,
            // Classic Perlin noise
            ClassicPerlinNoise4x4x4x4,

            // Array and textureless GLSL 2D simplex noise function.
            SimplexNoise,
            // Array and textureless GLSL 2D/3D/4D simplex noise functions
            SimplexNoise3x3x3,
            // Array and textureless GLSL 2D/3D/4D simplex noise functions
            SimplexNoise4x4x4x4,
            // Array and textureless GLSL 2D/3D/4D simplex noise functions
            //	SimplexNoiseGradient,
            //
            // 2-D tiling simplex noise with rotating gradients and analytical derivative.
            // The first component of the 3-element return vector is the noise value,
            // and the second and third components are the x and y partial derivatives.
            //
            //	RotatingNoise,
            // Assumming pnoise is perlin noise
            PerlinNoise,
            PerlinNoise3x3x3,
            PerlinNoise4x4x4x4,

            SRNoise,
            SRDNoise,
            SRDNoise2D,
            SRNoise2D,
            SimplexNoise3x3x3Gradient,

            // Used when noise is merged with other using arithmetic
            Mixed

        }
    }
    [System.Serializable]
    public struct DOTSNoiseData : Unity.Entities.IComponentData
    {
        public Noise.NoiseType NoiseType;
        public float percisionA;
        public float4 fromA, toA;
        public float percisionB;
        public float4 fromB,toB;

        private float4 currentA,currentB;

        public float4 Evaluate(bool4 upOneA,bool4 upOneB,Noise.NoiseType NoiseType)
        {
            if (upOneA.x) { currentA.x+=percisionA; if (currentA.x > toA.x) currentA.x = 0; }; 
            if (upOneA.y) { currentA.x += percisionA; if (currentA.y > toA.y) currentA.y = 0; }; 
            if (upOneA.z) { currentA.z += percisionA; if (currentA.z > toA.z) currentA.z = 0; }; 
            if (upOneA.w) { currentA.w += percisionA; if (currentA.w > toA.w) currentA.w = 0; };
            if (upOneB.x) { currentB.x += percisionB; if (currentB.x > toB.x) currentB.x = 0; };
            if (upOneB.y) { currentB.y += percisionB; if (currentB.y > toB.y) currentB.y = 0; };
            if (upOneB.z) { currentB.z += percisionB; if (currentB.z > toB.z) currentB.z = 0; };
            if (upOneB.w) { currentB.w += percisionB; if (currentB.w > toB.w) currentB.w = 0; };
            return Evaluate(NoiseType, currentA, currentB);
        }
        public float4 Evaluate(Noise.NoiseType NoiseType,float4 valueA,float4 valueB)
        {
            switch (NoiseType)
            {
                case Noise.NoiseType.CellularNoise:
                    return new float4(noise.cellular(valueA.xy), 0, 0);
                    break;
                case Noise.NoiseType.CellularNoise2x2:
                    return new float4(noise.cellular2x2(valueA.xy), 0, 0);
                    break;
                case Noise.NoiseType.CellularNoise2x2x2:
                    return new float4(noise.cellular2x2x2(valueA.xyz), 0);
                    break;
                case Noise.NoiseType.CellularNoise3x3x3:
                    return new float4(noise.cellular(valueA.xyz), 0);
                    break;
                case Noise.NoiseType.ClassicPerlinNoise:
                    return new float4(noise.cnoise(valueA.xy), 0, 0, 0);
                    break;
                case Noise.NoiseType.ClassicPerlinNoise3x3x3:
                    return new float4(noise.cnoise(valueA.xyz), 0, 0, 0);
                    break;
                case Noise.NoiseType.ClassicPerlinNoise4x4x4x4:
                    return new float4(noise.cnoise(valueA), 0, 0, 0);
                    break;
                case Noise.NoiseType.SimplexNoise:
                    return new float4(noise.snoise(valueA.xy), 0, 0, 0);
                    break;
                case Noise.NoiseType.SimplexNoise3x3x3:
                    return new float4(noise.snoise(valueA.xyz), 0, 0, 0);
                    break;
                case Noise.NoiseType.SimplexNoise4x4x4x4:
                    return new float4(noise.snoise(valueA), 0, 0, 0);
                    break;
                case Noise.NoiseType.SRNoise:
                    return new float4(noise.srnoise(valueA.xy), 0, 0, 0);
                    break;
                case Noise.NoiseType.PerlinNoise:
                    return new float4(noise.pnoise(valueA.xy, valueB.xy));
                    break;
                case Noise.NoiseType.PerlinNoise3x3x3:
                    return new float4(noise.pnoise(valueA.xyz, valueB.xyz), 0, 0, 0);
                    break;
                case Noise.NoiseType.PerlinNoise4x4x4x4:
                    return new float4(noise.pnoise(valueA, valueB), 0, 0, 0);
                    break;
                case Noise.NoiseType.SRDNoise2D:
                    return new float4(noise.srdnoise(valueA.xy, valueB.x), 0);
                    break;
                case Noise.NoiseType.SRNoise2D:
                    return new float4(noise.srnoise(valueA.xy, valueB.x), 0, 0, 0);
                    break;
                default:
                    Debug.LogError("Cannot use given NoiseType \"" + NoiseType + "\"");
                    return float4.zero;
                    break;
            }
        }
    }

    /// <summary>
    /// Have to make a copy of GradientAlphaKey since it doesn't have the serializable attribute on it
    /// </summary>
    [System.Serializable]
    public struct ECSGradientAlphaKey
    {
        ///<value>Alpha channel of key. 0 <= alpha <= 1</value> 
        public float alpha;
        ///<value>time of the key. 0 <= time <= 1</value> 
        public float time;
        /// <summary>
        /// Intializes the ECSGradientAlphaKey with the given parameters
        /// </summary>
        /// <param name="_alpha">Alpha channel of key. 0 <= alpha <= 1</param>
        /// <param name="_time">time of the key. 0 <= time <= 1</param>
        public ECSGradientAlphaKey(float _alpha, float _time)
        {
            alpha = _alpha;
            time = _time;
        }

        public override bool Equals(object obj)
        {
            ECSGradientAlphaKey key = (ECSGradientAlphaKey)obj;

            return alpha == key.alpha && time == key.time;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    /// <summary>
    /// Have to make a copy of GradientColorKey since it doesn't have the serializable attribute on it
    /// </summary>
    [System.Serializable]
    public struct ECSGradientColorKey
    {
        /// <value>key color</value>
        public float3 color;
        /// <value>time of the key. 0 <= time <= 1 </value>
        public float time;

        /// <summary>
        /// Initializes a GradientColorKey
        /// </summary>
        /// <param name="col">color of key</param>
        /// <param name="_time">time of the key</param>
        public ECSGradientColorKey(Color col, float _time)
        {
            color = new float3(col.r, col.g, col.b);
            time = _time;
        }
        /// <summary>
        /// Initializes a GradientColorKey
        /// </summary>
        /// <param name="col">color of key</param>
        /// <param name="_time">time of the key</param>
        public ECSGradientColorKey(float3 col, float _time)
        {
            color = col;
            time = _time;
        }
        /// <summary>
        /// returns the color of the ECSGradientColorKey as a UnityEngine.Color
        /// </summary>
        /// <returns></returns>
        
        
        
        public Color GetColor()
        {
            return new Color(color.x, color.y, color.z);
        }

        public override bool Equals(object obj)
        {
            ECSGradientColorKey key = (ECSGradientColorKey)obj;

            return color.Equals(key.color) && time == key.time;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }*/
}
