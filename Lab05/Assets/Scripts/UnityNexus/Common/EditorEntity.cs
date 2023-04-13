using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace UnityNexus.Common
{
    [System.Serializable]
    public struct EditorEntity : IComponentData
    {
        public Unity.Entities.Entity e;
        public int Index;
        public int Version;

        public EditorEntity(Unity.Entities.Entity e)
        {
            this.e = e;
            Index = e.Index;
            Version = e.Version;
        }
        public static EditorEntity Null => new EditorEntity{
            e = Unity.Entities.Entity.Null,
            Index = Unity.Entities.Entity.Null.Index,
            Version = Unity.Entities.Entity.Null.Version
        };
        public bool Equals(EditorEntity other)
        {
            return e.Equals(other.e);
        }
        public bool Equals(Unity.Entities.Entity other)
        {
            return e.Equals(other);
        }
    }
}