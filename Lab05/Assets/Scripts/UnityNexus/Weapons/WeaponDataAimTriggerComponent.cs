using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityNexus.TriggerRelated;
using UnityNexus.Weapons;

namespace UnityNexus.Weapons
{
    public class WeaponDataAimTriggerComponent : MonoBehaviour
    {
        public WeaponDataAimTrigger triggerData;
    }
    public class WeaponDataAimTriggerComponentBaker : Baker<WeaponDataAimTriggerComponent>
    {
        public override void Bake(WeaponDataAimTriggerComponent authoring)
        {
            if (authoring.triggerData.triggerEntitySpawnEvent &&
                authoring.GetComponent<TriggerEntitySpawnEventComponent>() == null)
                Debug.LogError($"Cannot add ComponentData! triggerEntitySpawnEvent is true but no TriggerEntitySpawnEventComponent was added to the GameObject!");
            else if (authoring.triggerData.triggerEntityStatusChangeEvent &&
              authoring.GetComponent<TriggerEntityStatusChangeEventComponent>() == null)
                Debug.LogError($"Cannot add ComponentData! triggerEntitySpawnEvent is true but no TriggerEntityStatusChangeEventComponent was added to the GameObject!");
           
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), authoring.triggerData);
        }
    }
}