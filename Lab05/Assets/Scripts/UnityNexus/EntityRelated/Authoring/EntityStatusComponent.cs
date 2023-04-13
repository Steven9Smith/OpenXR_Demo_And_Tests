using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace UnityNexus.EntityRelated
{
    public class EntityStatusComponent : MonoBehaviour
    {
        public EntityStatus status;
    }
    public class EntityStatusComponentBaker : Baker<EntityStatusComponent>
    {
        public override void Bake(EntityStatusComponent authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, authoring.status);
            AddComponent(e, new EntityStatusChange { });
            SetComponentEnabled<EntityStatusChange>(e, false);
        }
    }
}