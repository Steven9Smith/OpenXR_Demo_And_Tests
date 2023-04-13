using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityNexus.Physics
{
    public class EntityPrefabSpawnImpulseRequestComponent : MonoBehaviour
    {
        public EntityPrefabSpawnImpulseRequest request;
    }
    public class EntityPrefabSpawnImpulseRequestComponentBaker : Baker<EntityPrefabSpawnImpulseRequestComponent>
    {
public override void Bake(EntityPrefabSpawnImpulseRequestComponent authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic),authoring.request);
        }
    }
}