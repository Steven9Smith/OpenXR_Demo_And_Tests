using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityNexus.Common;
using UnityNexus.EntityRelated;

namespace UnityNexus.EntityRelated
{
    public class SlotableEntityComponent : MonoBehaviour
    {
        public EntitySlotComponent Slot;
        [EnumFlags]
        public EntitySlotSystem.EntityType SlotType;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public class SlotableEntityComponentBaker : Baker<SlotableEntityComponent>
    {
        public override void Bake(SlotableEntityComponent authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(e, new SlotableEntity
            {
                slotTypeMask = EnumFlagsAttribute.EnumToBitmask(EnumFlagsAttribute.GetSelectedIndexes(authoring.SlotType))
            });
            if (authoring.Slot != null)
                AddComponent(e, new EquipedInSlot
                {
                    slotEquipedIn = GetEntity(authoring.Slot, TransformUsageFlags.Dynamic),
                });
        }
    }
    public struct SlotableEntity : IComponentData
    {
        public uint slotTypeMask;
    }
    public struct EquipedInSlot : IComponentData
    {
        public Entity slotEquipedIn;
    }

    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(LocalToWorldSystem))]
    public partial struct SlotableEntitySystem : ISystem
    {
      //  EntityQuery SlotableEntityQuery;
       /* private partial struct SlotableEntityJob : IJobEntity
        {
            public ComponentLookup<LocalTransform> GetLT;
            public void Execute(ref LocalTransform transform,ref SlotableEntity slot)
            {
                if (!slot.slotEntity.Equals(Entity.Null))
                {
                  
                }
            }
        }*/
        public void OnCreate(ref SystemState state)
        {
         /*   SlotableEntityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<LocalTransform>()
                .WithAll<SlotableEntity>()
                .Build(ref state);*/
        }
        public void OnUpdate(ref SystemState state)
        {
            var LTLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);
           
            foreach( var(transform,equipedInSlot) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EquipedInSlot>>()){
                var slotLT = LTLookup[equipedInSlot.ValueRO.slotEquipedIn];
                transform.ValueRW = new LocalTransform
                {
                    Position = slotLT.Position,
                    Rotation = slotLT.Rotation,
                    Scale = transform.ValueRO.Scale
                };
            }
        }
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}