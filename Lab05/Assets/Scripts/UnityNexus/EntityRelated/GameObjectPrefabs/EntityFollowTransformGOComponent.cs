using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityNexus.EntityRelated;

namespace EntityRelated.GameObjectPrefabs
{
    [RequireComponent(typeof(PrefabInstantiationComponent))]
    public class EntityFollowTransformGOComponent : MonoBehaviour
    {

    }
    public class TransformGOFollowEntityComponentBaker : Baker<EntityFollowTransformGOComponent>
    {
        public override void Bake(EntityFollowTransformGOComponent authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new TransformGOFollowEntity { });
        }
    }
}