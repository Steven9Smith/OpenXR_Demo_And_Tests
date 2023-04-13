using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Egg.Extensions.Transforms
{
    public static class TransformsExtensions
    {
        /// <value>Direction of the normal of the collision</value>
        public enum Direction
        {
            X, Y, Z
        }
        /// <value>type of bounds to be used in physics calculations</value>
        public enum Bound
        {
            Outside,
            Inside
        }
        /// <summary>
        /// Checks if all the float values are 0 or is any of them contain a NaN
        /// </summary>
        /// <param name="q">quaternion to check</param>
        /// <returns>true if this quaternion is invalid, false otherwise</returns>
        public static bool IsInvalidQuaternion(quaternion q)
        {
            return q.value.x == 0 && q.value.y == 0 && q.value.z == 0 && q.value.w == 0 || (float.IsNaN(q.value.x) || float.IsNaN(q.value.z) || float.IsNaN(q.value.w) || float.IsNaN(q.value.y));
        }
        /// <summary>
        /// checks if a float3 either has an Inf or a NaN.
        /// </summary>
        /// <param name="f">float3 to check</param>
        /// <returns>true if this float3 is invalid, false otherwise</returns>
        public static bool IsInvalidFloat3(float3 f)
        {
            return (float.IsNaN(f.x) || float.IsNaN(f.y) || float.IsNaN(f.z)) || (float.IsInfinity(f.x) || float.IsInfinity(f.y) || float.IsInfinity(f.z));
        }

        public static float3 ZeroInvalidFloat3(float3 f)
        {
            if (float.IsNaN(f.x) || float.IsInfinity(f.x)) f.x = 0;
            if (float.IsNaN(f.y) || float.IsInfinity(f.y)) f.y = 0;
            if (float.IsNaN(f.z) || float.IsInfinity(f.z)) f.z = 0;
            return f;
        }

        public static Matrix4x4 GetTRSMatrix(Transform transform)
        {
            return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        }

        public static Matrix4x4 GetTRSMatrix(RigidTransform transform, float3 scale)
        {
            return Matrix4x4.TRS(transform.pos, transform.rot, scale);
        }

 /*       public static Matrix4x4 GetTRSMatrix(Translation translation, Rotation rotation, NonUniformScale scale)
        {
            return Matrix4x4.TRS(translation.Value, rotation.Value, scale.Value);
        }*/

        public static Matrix4x4 GetTRSMatrix(LocalTransform transform)
        {
            return Matrix4x4.TRS(transform.Position, transform.Rotation,new float3(transform.Scale, transform.Scale, transform.Scale));
        }

        public static Matrix4x4 GetTRSMatrix(LocalTransform transform, float3 scale)
        {
            return Matrix4x4.TRS(transform.Position, transform.Rotation, scale);
        }

        public static Matrix4x4 GetTRSMatrix(float3 translation, quaternion rotation, float3 scale)
        {
            return Matrix4x4.TRS(translation, rotation, scale);
        }

        public static float3 TransformPoint(RigidTransform transform, float3 vector, float3 localScale)
        {
            return (Quaternion)transform.rot * Vector3.Scale(vector, localScale) + (Vector3)transform.pos;
        }

        public static float3 TransformDirection(quaternion rot, float3 vec)
        {
            return math.mul(rot, vec);
        }

        public static float3 TransformDirection(Matrix4x4 a, float3 localDirectionDirection)
        {
            return a.MultiplyVector(localDirectionDirection);
        }

        public static quaternion FromToRotation(float3 from, float3 to)
            => quaternion.AxisAngle(
                angle: math.acos(math.clamp(math.dot(math.normalize(from), math.normalize(to)), -1f, 1f)),
                axis: math.normalize(math.cross(from, to))
            );
        // the scale extensions are untested! use at your own risk!
      /*  public static void RemoveScaleFromLocalToWorld(this LocalTransform ltw, int percision = 5)
        {
            ltw.Value.c0.x /= s.Value;
            ltw.Value.c1.y /= s.Value;
            ltw.Value.c2.z /= s.Value;
        }
        public static void RemoveScaleFromLocalToWorld(this LocalToWorld ltw, NonUniformScale s, int percision = 5)
        {
            ltw.Value.c0.x /= s.Value.x;
            ltw.Value.c1.y /= s.Value.y;
            ltw.Value.c2.z /= s.Value.z;
        }
        public static void RemoveScaleFromLocalToWorld(this LocalToWorld ltw, CompositeScale s, int percision = 5)
        {
            math.mul(ltw.Value,math.inverse(s.Value));
        }*/

        // NOT BURST FREINDLY
        public static float3 ToEuler(quaternion q) { return ((Quaternion)(q)).eulerAngles; }
        public static float3 ToEuler(float4x4 q) { return ((Quaternion)((new quaternion(q)))).eulerAngles; }

        // Note: taken from Unity.Animation/Core/MathExtensions.cs, which will be moved to Unity.Mathematics at some point
        //       after that, this should be removed and the Mathematics version should be used
        #region toEuler
        public static float3 toEuler(quaternion q, math.RotationOrder order = math.RotationOrder.Default)
        {
            const float epsilon = 1e-6f;

            //prepare the data
            var qv = q.value;
            var d1 = qv * qv.wwww * new float4(2.0f); //xw, yw, zw, ww
            var d2 = qv * qv.yzxw * new float4(2.0f); //xy, yz, zx, ww
            var d3 = qv * qv;
            var euler = new float3(0.0f);

            const float CUTOFF = (1.0f - 2.0f * epsilon) * (1.0f - 2.0f * epsilon);

            switch (order)
            {
                case math.RotationOrder.ZYX:
                {
                    var y1 = d2.z + d1.y;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = -d2.x + d1.z;
                        var x2 = d3.x + d3.w - d3.y - d3.z;
                        var z1 = -d2.y + d1.x;
                        var z2 = d3.z + d3.w - d3.y - d3.x;
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), math.atan2(z1, z2));
                    }
                    else     //zxz
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.z, d1.y, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z);     //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.ZXY:
                {
                    var y1 = d2.y - d1.x;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = d2.x + d1.z;
                        var x2 = d3.y + d3.w - d3.x - d3.z;
                        var z1 = d2.z + d1.y;
                        var z2 = d3.z + d3.w - d3.x - d3.y;
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), math.atan2(z1, z2));
                    }
                    else     //zxz
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.z, d1.y, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z);     //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.YXZ:
                {
                    var y1 = d2.y + d1.x;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = -d2.z + d1.y;
                        var x2 = d3.z + d3.w - d3.x - d3.y;
                        var z1 = -d2.x + d1.z;
                        var z2 = d3.y + d3.w - d3.z - d3.x;
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), math.atan2(z1, z2));
                    }
                    else     //yzy
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.x, d1.z, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z);     //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.YZX:
                {
                    var y1 = d2.x - d1.z;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = d2.z + d1.y;
                        var x2 = d3.x + d3.w - d3.z - d3.y;
                        var z1 = d2.y + d1.x;
                        var z2 = d3.y + d3.w - d3.x - d3.z;
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), math.atan2(z1, z2));
                    }
                    else     //yxy
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.x, d1.z, d2.y, d1.x);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z);     //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.XZY:
                {
                    var y1 = d2.x + d1.z;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = -d2.y + d1.x;
                        var x2 = d3.y + d3.w - d3.z - d3.x;
                        var z1 = -d2.z + d1.y;
                        var z2 = d3.x + d3.w - d3.y - d3.z;
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), math.atan2(z1, z2));
                    }
                    else     //xyx
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.x, d1.z, d2.z, d1.y);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z);     //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), math.asin(y1), 0.0f);
                    }

                    break;
                }

                case math.RotationOrder.XYZ:
                {
                    var y1 = d2.z - d1.y;
                    if (y1 * y1 < CUTOFF)
                    {
                        var x1 = d2.y + d1.x;
                        var x2 = d3.z + d3.w - d3.y - d3.x;
                        var z1 = d2.x + d1.z;
                        var z2 = d3.x + d3.w - d3.y - d3.z;
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), math.atan2(z1, z2));
                    }
                    else     //xzx
                    {
                        y1 = math.clamp(y1, -1.0f, 1.0f);
                        var abcd = new float4(d2.z, d1.y, d2.x, d1.z);
                        var x1 = 2.0f * (abcd.x * abcd.w + abcd.y * abcd.z);     //2(ad+bc)
                        var x2 = math.csum(abcd * abcd * new float4(-1.0f, 1.0f, -1.0f, 1.0f));
                        euler = new float3(math.atan2(x1, x2), -math.asin(y1), 0.0f);
                    }

                    break;
                }
            }

            return eulerReorderBack(euler, order);
        }

        static float3 eulerReorderBack(float3 euler, math.RotationOrder order)
        {
            switch (order)
            {
                case math.RotationOrder.XZY:
                    return euler.xzy;
                case math.RotationOrder.YZX:
                    return euler.zxy;
                case math.RotationOrder.YXZ:
                    return euler.yxz;
                case math.RotationOrder.ZXY:
                    return euler.yzx;
                case math.RotationOrder.ZYX:
                    return euler.zyx;
                case math.RotationOrder.XYZ:
                default:
                    return euler;
            }
        }

        #endregion

        #region RotateAround
        /// <summary>
        /// Rotates a transform around the given Pivot point by the given rotation.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="pivotPoint"></param>
        /// <param name="rot"></param>
        public static void RotateAround(Transform transform, Vector3 pivotPoint, Quaternion rot)
        {
            transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
            transform.rotation = rot * transform.rotation;
        }

        public static float3 RotateAround(float3 position, float3 pivotPoint, quaternion rot)
        {
            return math.mul(rot, (position - pivotPoint)) + pivotPoint;
        }

        /// <summary>
        /// Rotates a transform around the given Pivot point by the given rotation.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="pivotPoint"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static LocalToWorld RotateAround(LocalToWorld transform, float3 pivotPoint, quaternion rot)
        {
            return new LocalToWorld
            {
                Value = new float4x4(math.mul(rot, transform.Rotation),
                    math.mul(rot, (transform.Position - pivotPoint)) + pivotPoint
                )
            };
        }

        public static Vector3 RotatePointAroundAxis(Vector3 point, float angle, Vector3 axis)
        {
            Quaternion q = Quaternion.AngleAxis(angle, axis);
            return q * point; //Note: q must be first (point * q wouldn't compile)
        }

        public static Vector3 RotateDirectionVector(Vector3 direction, Vector3 rotation)
        {
            direction.Normalize();
            Quaternion rot = Quaternion.Euler(rotation); // Rotate [angle] degrees about the x axis.
            direction = rot * direction;
            return direction;
        }

        public static Vector3 RotateDirectionVector(Vector3 direction, Quaternion rot)
        {
            direction.Normalize();
            // Quaternion rot = Quaternion.Euler(rotation); // Rotate [angle] degrees about the x axis.
            direction = rot * direction;
            return direction;
        }

        /// <summary>
        /// Rotates a transform around the given Pivot point by the given angle on the given axis.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="pivotPoint"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public static void RotateAround(Transform transform, Vector3 pivotPoint, Vector3 axis, float angle)
        {
            Quaternion rot = Quaternion.AngleAxis(angle, axis);
            transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
            transform.rotation = rot * transform.rotation;
        }

        /// <summary>
        /// Rotates a transform around the given Pivot point by the given angle on the given axis.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="pivotPoint"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static LocalToWorld RotateAround(LocalToWorld transform, float3 pivotPoint, float3 axis, float angle)
        {
            quaternion rot = quaternion.AxisAngle(axis, angle);
            return new LocalToWorld
            {
                Value = new float4x4(
                    math.mul(rot, transform.Rotation),
                    math.mul(rot, (transform.Position - pivotPoint)) + pivotPoint)
            };
        }

        #endregion
        public static LocalToWorld Create(float3 position, quaternion rotation, float3 localScale, float3 lossyScale)
        {
            var a = new LocalToWorld { Value = new float4x4(rotation, position) };
            a.Value.c0.x *= localScale.x * lossyScale.x;
            a.Value.c1.y *= localScale.y * lossyScale.y;
            a.Value.c2.z *= localScale.z * lossyScale.z;
            return a;
        }
        public static LocalToWorld Create(float3 position,quaternion rotation,float3 scale)
        {
            var a = new LocalToWorld { Value = new float4x4(rotation, position) };
            a.Value.c0.x *= scale.x;
            a.Value.c1.y *= scale.y;
            a.Value.c2.z *= scale.z;
            return a;
        }
        public static LocalToWorld Create(float3 position, quaternion rotation, float scale)
        {
            var a = new LocalToWorld { Value = new float4x4(rotation, position) };
            a.Value.c0.x *= scale;
            a.Value.c1.y *= scale;
            a.Value.c2.z *= scale;
            return a;
        }
        public static LocalToWorld Create(float3 position, quaternion rotation)
        {
            var a = new LocalToWorld { Value = new float4x4(rotation, position) };
            return a;
        }
    }
}
