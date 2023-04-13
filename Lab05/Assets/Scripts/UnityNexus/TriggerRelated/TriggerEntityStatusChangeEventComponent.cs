using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityNexus.TriggerRelated;

namespace UnityNexus.TriggerRelated
{
    public class TriggerEntityStatusChangeEventComponent : MonoBehaviour
    {
        public TriggerEntityStatusChangeEvent triggerEvent;
        public TriggerEventStatusTracker triggerEventTracker;
    }
    public class TriggerEntityStatusChangeEventComponentBaker : Baker<TriggerEntityStatusChangeEventComponent>
    {
        public override void Bake(TriggerEntityStatusChangeEventComponent authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, authoring.triggerEvent);
            AddComponent(e, authoring.triggerEventTracker);
            AddComponent(e, new IsECSTriggerEvent { });
        }
    }
}