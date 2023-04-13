using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using UnityNexus.EntityRelated;

namespace UnityNexus.AI_Related.NavMesh
{
    [RequireComponent(typeof(PrefabInstantiationComponent))]
    public class ECSNavMeshAgentComponent : MonoBehaviour
    {
        public Transform EntityToFollow;
        public Vector3 PositionToFollow;
        //the prefab must be added using a PrefabInstatiationComponent
        // public GameObject Prefab;
       
    }
    public class ECSNavMeshAgentBaker : Baker<ECSNavMeshAgentComponent>
    {
        public override void Bake(ECSNavMeshAgentComponent authoring)
        {
            var e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new ECSNavMeshAgentPrefabSetup { });
            AddComponent(e, new ECSNavMeshAgentMoveTo
            {
                entity = authoring.EntityToFollow != null ? GetEntity(authoring.EntityToFollow, TransformUsageFlags.Dynamic) : Entity.Null,
                position = authoring.PositionToFollow
            });
            AddComponent(e, new EntityFollowTransformGO { });
        }
    }
}