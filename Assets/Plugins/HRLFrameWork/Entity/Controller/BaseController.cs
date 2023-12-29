using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HRL
{
    public class BaseController : MonoBehaviour
    {
        [ReadOnly]
        public Entity Owner;

        public virtual void Init(Entity owner)
        {
            Owner = owner;
        }

        private void Awake()
        {
            
        }

        private void Update()
        {
            
        }
    }
}