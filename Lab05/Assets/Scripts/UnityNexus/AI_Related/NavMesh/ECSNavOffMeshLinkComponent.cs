using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using UnityNexus.AI_Related.NavMesh;
using UnityNexus.EntityRelated;

namespace Assets.Scripts.UnityNexus.AI_Related.NavMesh
{
    [DisallowMultipleComponent]
    public class ECSNavOffMeshLinkComponent : MonoBehaviour
    {
        public Transform NavMeshSurfaceTransform;
        public Transform start;
        public Transform end;
    }
    public class ECSNavOffMeshLinkComponentBaker : Baker<ECSNavOffMeshLinkComponent>
    {

        public override void Bake(ECSNavOffMeshLinkComponent authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            
            AddComponentObject(e, new ECSNavOffMeshLinkPrefabSetup
            {
                start = GetEntity(authoring.start, TransformUsageFlags.None),
                end = GetEntity(authoring.end, TransformUsageFlags.None),
                ANavMeshSurfaceEntity = GetEntity(authoring.NavMeshSurfaceTransform, TransformUsageFlags.None),
                startHasPrefabInstantiateComponent = authoring.start.GetComponent<PrefabInstantiationComponent>() != null,
                endHasPrefabInstantiateComponent = authoring.end.GetComponent<PrefabInstantiationComponent>() != null,
            });
            AddComponent(e, new TransformGOFollowEntity { });
        }
    }
}