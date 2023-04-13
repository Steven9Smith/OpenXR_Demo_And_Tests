using System.Collections;
using Unity.AI.Navigation;
using Unity.Entities;
using UnityEngine;
using UnityNexus.EntityRelated;

namespace UnityNexus.AI_Related.NavMesh
{
    [RequireComponent(typeof(PrefabInstantiationComponent))]
    public class ECSNavMeshSurfaceComponent : MonoBehaviour
    {
    }
    public class ECSNavMeshSurfaceBaker : Baker<ECSNavMeshSurfaceComponent>
    {
        public override void Bake(ECSNavMeshSurfaceComponent authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            AddComponent(e,new ECSNavMeshSurfacePrefabSetup { });
            AddComponent(e, new TransformGOFollowEntity { });
        }
    }
}