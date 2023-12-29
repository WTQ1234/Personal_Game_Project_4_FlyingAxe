using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HRL
{
    public class MobAbilityController : BaseController
    {
        public Entity currentTarget;

        public Dictionary<string, MobAbilityBase> abilities;

        public override void Init(Entity owner)
        {
            base.Init(owner);
            abilities = new Dictionary<string, MobAbilityBase>();
            MobAbilityBase[] abilities_array = GetComponents<MobAbilityBase>();
            foreach (MobAbilityBase ability in abilities_array)
            {
                RegisterMobAbility(ability);
            }
        }

        #region Target
        public void OnSetEntityTarget(Entity target)
        {
            currentTarget = target;
            _OnSetTargetEntity();
        }

        private void _OnSetTargetEntity()
        {
            foreach(var pair in abilities)
            {
                pair.Value.OnSetEntityTarget(currentTarget);
            }
        }

        public Entity OnGetEntityTarget()
        {
            return currentTarget;
        }
        #endregion

        private void RegisterMobAbility(MobAbilityBase ability)
        {
            abilities.Add(ability.GetName(), ability);
            ability.AbilityController = this;
        }

        public T GetMobAbilityByName<T>(string Name) where T : PlayerAbility_Attack
        {
            if (abilities.ContainsKey(Name))
            {
                return (T)abilities[Name];
            }
            else
            {
                return null;
            }
        }
    }
}
