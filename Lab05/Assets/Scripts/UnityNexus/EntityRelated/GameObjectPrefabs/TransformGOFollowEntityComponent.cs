using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityNexus.EntityRelated;

namespace Assets.Scripts.UnityNexus.EntityRelated.GameObjectPrefabs
{
    [RequireComponent(typeof(PrefabInstantiationComponent))]
    public class TransformGOFollowEntityComponent : MonoBehaviour
    {

    }
    public class TransformGOFollowEntityComponentBaker : Baker<TransformGOFollowEntityComponent>
    {
        public override void Bake(TransformGOFollowEntityComponent authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new TransformGOFollowEntity { });
        }
    }
}