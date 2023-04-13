using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace UnityNexus.Common
{
    [System.Serializable]
    public struct EntityObjectLink_Managed
    {
        public Object obj;
        public Entity entity;
    }
    [System.Serializable]
    public struct EntityObjectLink_ComponentData : IComponentData
    {
        public Entity entity;
        // this will be have to handled manually
        public int ReferenceGameObjectArrayIndex;
    }
    [System.Serializable]
    public struct EntityGameObjectLinke_Managed
    {
        public Entity entity;
        public GameObject gameObject;
    }
}