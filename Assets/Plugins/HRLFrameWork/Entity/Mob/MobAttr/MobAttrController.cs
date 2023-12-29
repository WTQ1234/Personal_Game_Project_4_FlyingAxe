using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HRL
{
    public class MobAttrController : BaseController
    {
        [ShowInInspector]
        public Dictionary<string, MobAttrBase> Attrs;

        public override void Init(Entity owner)
        {
            base.Init(owner);
            Attrs = new Dictionary<string, MobAttrBase>();
        }

        public T GetAttr<T>(string attr_name) where T : MobAttrBase
        {
            if (Attrs.ContainsKey(attr_name))
            {
                return (T)Attrs[attr_name];
            }
            else
            {
                return null;
            }
        }

        public void SetAttr<T>(string attr_name, T attr) where T : MobAttrBase
        {
            if (Attrs.ContainsKey(attr_name))
            {
                return;
            }
            else
            {
                attr.Owner = Owner;
                Attrs.Add(attr_name, attr);
            }
        }
    }

}
