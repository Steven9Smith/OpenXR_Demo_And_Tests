using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityNexus.TriggerRelated;
using UnityNexus.Weapons;

namespace UnityNexus.Weapons
{
    public class WeaponDataShootTriggerComponent : MonoBehaviour
    {
        public WeaponDataShootTrigger triggerData;
        public Transform EntityPrefabTrigger1;
        public Transform EntityPrefabTrigger2;
    }
    public class WeaponDataShootTriggerComponentBaker : Baker<WeaponDataShootTriggerComponent>
    {
        public override void Bake(WeaponDataShootTriggerComponent authoring)
        {
            if (authoring.EntityPrefabTrigger1 == null)
                Debug.Log("Detected null Trigger Entity!");
            if (authoring.EntityPrefabTrigger2 == null)
                Debug.Log("Detected null Trigger Entity!");
            authoring.triggerData.spawnEvent.entity = GetEntity(authoring.EntityPrefabTrigger1, TransformUsageFlags.Dynamic);
            authoring.triggerData.spawnEvent2.entity = GetEntity(authoring.EntityPrefabTrigger2, TransformUsageFlags.Dynamic);
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), authoring.triggerData);
            
        }
    }
}