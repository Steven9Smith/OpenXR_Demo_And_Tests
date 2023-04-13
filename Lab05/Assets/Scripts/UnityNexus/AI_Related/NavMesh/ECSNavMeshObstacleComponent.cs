using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityNexus.EntityRelated;

namespace UnityNexus.AI_Related.NavMesh
{
    [RequireComponent(typeof(PrefabInstantiationComponent))]
    public class ECSNavMeshObstacleComponent : MonoBehaviour
    {
        public bool followUsingLocalTransform;
    }
    public class ECSNavMeshObstacleBaker : Baker<ECSNavMeshObstacleComponent>
    {
        public override void Bake(ECSNavMeshObstacleComponent authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new ECSNavMeshObstaclePrefabSetup { });
            AddComponent(e, new TransformGOFollowEntity {
                useLocalTransform = authoring.followUsingLocalTransform
            });
        }
    }
}