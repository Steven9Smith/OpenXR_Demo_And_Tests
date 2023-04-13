using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace UnityNexus.Common{

    [System.Serializable]
    public class InGameEntitiesList
    {
        public List<Entity> entities;
        private EntityManager em;
        private EntityQuery query;
        private EntityQuery NULL_QUERY = new EntityQuery();

        public void SetEntityManager(EntityManager em) {
            this.em = em;
        }
        public void SetEntityQuery(EntityQuery query)
        {
            this.query = query;
        }
        public void Update()
        {
            if (!query.Equals(NULL_QUERY) && query.CalculateEntityCount() > 0)
            {
                entities.Clear();
                var e = query.ToEntityArray(Unity.Collections.Allocator.Temp).ToArray();
                entities.AddRange(e);
            }
        }
    }
}
