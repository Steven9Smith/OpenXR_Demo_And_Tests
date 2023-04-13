using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Egg.Extensions.Camera
{
    public class CameraExtensions
    {
        public enum CameraPerspectiveMode
        {
            Orthographic,
            Perspective
        }

        // Matrix Blender functions
        public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
        {
            Matrix4x4 ret = new Matrix4x4();
            for (int i = 0; i < 16; i++)
                ret[i] = Mathf.Lerp(from[i], to[i], time);
            return ret;
        }

        /*    public static bool LerpFromTo(Camera camera, Matrix4x4 src, Matrix4x4 dest, float duration)
            {
                float startTime = Time.time;
                while (Time.time - startTime < duration)
                {
                    camera.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration);
                    //  return false;
                }
                camera.projectionMatrix = dest;
                return true;
            }

            public static bool ExecutePerspectiveChange(Camera camera, float timeStamp, CameraExtensionData cameraExtensionData)
            {
                if (cameraExtensionData.CameraPerspectiveMode == CameraPerspectiveMode.Orthographic)
                    return LerpFromTo(camera, camera.projectionMatrix, cameraExtensionData.orthographicViewMatrix, 1f);
                else if (cameraExtensionData.CameraPerspectiveMode == CameraPerspectiveMode.Perspective)
                    return LerpFromTo(camera, camera.projectionMatrix, cameraExtensionData.perspectiveViewMatrix, 1f);
                else
                {
                    Debug.LogError("Invalid Camera Perspective detected");
                    return false;
                }
            }*/

        public struct CameraExtensionData : IComponentData
        {
            public float4x4 orthographicViewMatrix, perspectiveViewMatrix;
            public float fov, nearClipPlane, farClipPlane, orthographicSize;
            public float aspectRatio;
            public CameraPerspectiveMode CameraPerspectiveMode;
        }
    }
}
