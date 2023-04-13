using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;

namespace UnityNexus.Extensions
{
    public static class MathematicsExtensions
    {

        // *Undocumented*
        public const float kEpsilon = 0.00001F;
        // *Undocumented*
        public const float kEpsilonNormalSqrt = 1e-15f;
        
        // Returns the angle in degrees between /from/ and /to/.
        public static float angle(float2 from, float2 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = math.sqrt(math.lengthsq(from) * math.lengthsq(to));
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = math.clamp(math.dot(from, to) / denominator, -1F, 1F);
            return math.degrees(math.acos(dot));
        }

        // Returns the signed angle in degrees between /from/ and /to/. Always returns the smallest possible angle
        public static float signedAngle(float2 from, float2 to)
        {
            float unsigned_angle = angle(from, to);
            float sign = math.sign(from.x * to.y - from.y * to.x);
            return unsigned_angle * sign;
        }
        // Returns the angle in degrees between /from/ and /to/. This is always the smallest
        public static float angle(float3 from, float3 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            float denominator = math.sqrt(math.lengthsq(from) * math.lengthsq(to));
            if (denominator < kEpsilonNormalSqrt)
                return 0F;

            float dot = math.clamp(math.dot(from, to) / denominator, -1F, 1F);
            return math.degrees(math.acos(dot));
        }

        // The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
        // If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
        // The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
        public static float signedAngle(float3 from, float3 to, float3 axis)
        {
            float unsignedAngle = angle(from, to);

            float cross_x = from.y * to.z - from.z * to.y;
            float cross_y = from.z * to.x - from.x * to.z;
            float cross_z = from.x * to.y - from.y * to.x;
            float sign = math.sign(axis.x * cross_x + axis.y * cross_y + axis.z * cross_z);
            return unsignedAngle * sign;
        }
    }
}
