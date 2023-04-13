using System.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace UnityNexus.EntityRelated
{
    public class PrefabChildEntityLinkComponent : MonoBehaviour
    {
        public Transform EntityThatHasPrefab;
        public string PrefabChilName;
    }
    public class PrefabChildEntityLinkComponentBaker : Baker<PrefabChildEntityLinkComponent>
    {
        public override void Bake(PrefabChildEntityLinkComponent authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PrefabChildEntityLink {
                EntityThatHasPrefab = GetEntity(authoring.EntityThatHasPrefab,TransformUsageFlags.None),
                PrefabChilName = new FixedString64Bytes(authoring.PrefabChilName)
            });
        }
    }
    public struct PrefabChildEntityLink : IComponentData
    {
        public Entity EntityThatHasPrefab;
        public FixedString64Bytes PrefabChilName;
    }
}