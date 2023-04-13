using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using log4net.Util;

namespace UnityNexus.Extensions
{
    //NOT WORKING RIGHT YET
    public class EntityFollowEntity : MonoBehaviour
    {
        public GameObject ToFollow;
        public Vector3 PositionOffset, RotationOffset
            ;
        public bool UpdateOnLateUpdate = false;
        // Start is called before the first frame update
        void Start()
        {
            transform.position = ToFollow.transform.position + PositionOffset;
            transform.rotation = ToFollow.transform.rotation;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (UpdateOnLateUpdate)
            {
                transform.position = ToFollow.transform.position + PositionOffset;
                transform.rotation = ToFollow.transform.rotation;
            }
        }

        void Update()
        {
            if (!UpdateOnLateUpdate)
            {
                transform.position = ToFollow.transform.position + PositionOffset;
                transform.rotation = ToFollow.transform.rotation;
            }
        }
    }
    public class EntityFollowEntityBaker : Baker<EntityFollowEntity>
    {
        public override void Bake(EntityFollowEntity authoring)
        {
            if (authoring.ToFollow == null) return;
            Entity e = GetEntity(authoring.ToFollow, TransformUsageFlags.Dynamic);
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            if (e.Equals(Entity.Null))
            {
                Debug.LogWarning("Entity that is to be followed and not an Entity. remove the ConvertToEntity on this component to have the GameObject be followed");
                return;
            }

            AddComponent(entity, new EntityFollowEntityData {
                Value = e,
                Offset = new LocalTransform { Position = authoring.PositionOffset, Rotation = Quaternion.Euler(authoring.RotationOffset), Scale = 1 },
                Distance = authoring.PositionOffset.magnitude
            });
        }
    }
    public struct EntityFollowEntityData : IComponentData
    {
        public Entity Value;
        public float Distance;
        public LocalTransform Offset;
    }
    public partial class EntityFollowEntitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithBurst()
                .ForEach((Entity e, in EntityFollowEntityData entityFollowEntityData) =>
                {
                    var followingTransform = SystemAPI.GetComponent<LocalTransform>(entityFollowEntityData.Value);
                    var eTransform = SystemAPI.GetComponent<LocalTransform>(e);
                    eTransform.Position = followingTransform.Position + ((entityFollowEntityData.Offset.Position * followingTransform.Forward() + entityFollowEntityData.Offset.Position * followingTransform.Up()   ) *entityFollowEntityData.Distance);
                    eTransform.Rotation = math.mul(followingTransform.Rotation, entityFollowEntityData.Offset.Rotation);
                    SystemAPI.SetComponent(e, eTransform);

                })
                .Schedule();
        }
    }
}
