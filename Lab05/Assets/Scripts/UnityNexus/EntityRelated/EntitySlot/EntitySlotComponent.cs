using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityNexus.Common;

namespace UnityNexus.EntityRelated
{
    /// <summary>
    /// This allows you to "slot" entities into others
    /// </summary>
    public class EntitySlotComponent : MonoBehaviour
    {
        public FirstPersonPlayerAuthoring player;
        public SlotableEntityComponent EntityInSlot;
        public bool captureTransform;
        [EnumFlags]
        public EntitySlotSystem.EntityType AllowedSlotTypes;
        private void OnValidate()
        {
            UpdateTransform();
        }
        // Start is called before the first frame update
        void Start()
        {
            UpdateTransform();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTransform();
        }
        void UpdateTransform()
        {
            if (EntityInSlot != null)
            {
                var slotableType = EnumFlagsAttribute.GetSelectedIndexes(EntityInSlot.SlotType);
                var slotTypes = EnumFlagsAttribute.GetSelectedIndexes(AllowedSlotTypes);
                bool match = false;
                for(int i = 0; i < slotableType.Count; i++)
                {
                    match = slotTypes.Contains(slotableType[i]);
                    if (match) break;
                }
                if (match)
                {
                    if (captureTransform)
                    {
                        transform.position = EntityInSlot.transform.position;
                        transform.rotation = EntityInSlot.transform.rotation;
                    }
                }
                else
                {
                    EntityInSlot = null;
                    Debug.LogWarning("Cannot place Slotable Entity In This Slot because the types don't match");
                }
            }
        }
    }

    public class EntitySlotComponentBaker : Baker<EntitySlotComponent>
    {
        public override void Bake(EntitySlotComponent authoring)
        {
            AddComponent<EntitySlot>(GetEntity(TransformUsageFlags.Dynamic), new EntitySlot
            {
                allowedSlotTypes = authoring.AllowedSlotTypes,
                entity = GetEntity(authoring.EntityInSlot, TransformUsageFlags.Dynamic),
                playerEntity = GetEntity(authoring.player,TransformUsageFlags.Dynamic)
            });
        }
    }
    public struct EntitySlot : IComponentData
    {
        public Entity playerEntity;
        public Entity entity;
        public EntitySlotSystem.EntityType allowedSlotTypes;
    }

    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(LocalToWorldSystem))]
    [RequireMatchingQueriesForUpdate]
    public partial struct EntitySlotSystem : ISystem
    {
        [System.Flags]
        public enum EntityType: uint
        {
            Gun = 1u << 0,
            Ammo = 1u << 1,
            Leathal = 1u << 2,
            Tactical = 1u << 3,
            Melee = 1u << 4,
            Bullet = 1u << 5,
        }


        private partial struct SlotUpdateJob : IJobEntity
        {
            public void Execute(ref EntitySlot slot)
            {

            }
        }

        public void OnCreate(ref SystemState state)
        {

        }
        public void OnUpdate(ref SystemState state)
        {

        }
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}