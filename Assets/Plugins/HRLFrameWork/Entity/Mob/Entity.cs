using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HRL
{
    public enum EntityType
    {
        Default = 0,
        Player = 1,
        PlayerSummon = 2,
        NPC = 3,
        Enemy = 4,
        SceneEntity = 5,
        Weapon = 6,
        Bullet = 7,
    }

    // Entity 指可活动物体，如玩家，NPC，敌人等
    public class Entity : MonoBehaviour
    {
        public Rigidbody2D rb2D;

        public Animator animator;

        #region EntityType
        public virtual EntityType getEntityType()
        {
            return EntityType.Default;
        }

        public virtual bool isPlayer()
        {
            return getEntityType() == EntityType.Player;
        }
        #endregion

        protected virtual void Awake()
        {
            rb2D = GetComponent<Rigidbody2D>();
            _RegisterController();
            // 先加载Controller，后加载UI，否则读取不到数据
            _RegisterFollowUI();
        }

        protected virtual void Update() { }

        protected virtual void OnDestroy()
        {
            if (current_follow_ui != null)
            {
                Destroy(current_follow_ui.gameObject);
            }
        }

        #region Entity Info
        [ShowInInspector, ReadOnly]
        protected BasicInfo EntityInfo;

        public BasicInfo GetEntityInfo()
        {
            return EntityInfo;
        }

        public virtual void SetEntityInfo(BasicInfo info)
        {
            EntityInfo = info;
        }
        #endregion

        #region Controller
        protected virtual void _RegisterController()
        {
            if (attrController != null)
                attrController.Init(this);
            if (teamController != null)
                teamController.Init(this);
            if (abilityController != null)
                abilityController.Init(this);
        }

        #region Attr
        [Title("属性")]
        public bool ShowAttr = true;
        public MobAttrController attrController
        {
            get
            {
                if (!ShowTeam)
                {
                    return null;
                }
                if (_attrController == null)
                {
                    _attrController = gameObject.GetComponent<MobAttrController>();
                }
                if (_attrController == null)
                {
                    LogHelper.Warning("GetComponent", "can not get component that should on Gameobject", "MobAttrController");
                    _attrController = gameObject.AddComponent<MobAttrController>();
                }
                return _attrController;
            }
        }
        // ShowIf 与 ShowInInspector 冲突，导致无法对prefab进行修改，很怪
        [ShowIf("ShowAttr")]
        public MobAttrController _attrController;
        #endregion

        #region Team
        [Title("队伍")]
        public bool ShowTeam = true;
        public TeamController teamController
        {
            get
            {
                if (!ShowTeam)
                {
                    return null;
                }
                if (_teamController == null)
                {
                    _teamController = gameObject.GetComponent<TeamController>();
                }
                if (_teamController == null)
                {
                    LogHelper.Warning("GetComponent", "can not get component that should on Gameobject", "TeamController");
                    _teamController = gameObject.AddComponent<TeamController>();
                }
                return _teamController;
            }
        }
        [ShowIf("ShowTeam")]
        public TeamController _teamController;

        public virtual void _OnSetTeam() { }
        #endregion

        #region Health
        [Title("生命")]
        public bool ShowHealth = true;
        public PlayerHealth playerHealth
        {
            get
            {
                if (!ShowHealth)
                {
                    return null;
                }
                if (_playerHealth == null)
                {
                    _playerHealth = gameObject.GetComponent<PlayerHealth>();
                }
                return _playerHealth;
            }
        }
        [ShowIf("ShowHealth")]
        public PlayerHealth _playerHealth;
        #endregion

        #region Ability
        [Title("能力")]
        public bool ShowAbility = true;
        public MobAbilityController abilityController
        {
            get
            {
                if (!ShowAbility)
                {
                    return null;
                }
                if (_mobAbilityController == null)
                {
                    _mobAbilityController = gameObject.GetComponent<MobAbilityController>();
                }
                return _mobAbilityController;
            }
        }
        [ShowIf("ShowAbility")]
        public MobAbilityController _mobAbilityController;
        #endregion
        #endregion

        #region FollowUI
        [Title("FollowUI")]
        public bool ShowFollowUI = false;
        [ShowIf("ShowFollowUI")]
        public UIViewInScene followUI;
        private UIViewInScene current_follow_ui;

        protected virtual UIInfo _GetFollowUIInfo()
        {
            UIInfo info = new UIInfo();
            return info;
        }

        protected virtual void _RegisterFollowUI()
        {
            UIInfo info = _GetFollowUIInfo();
            current_follow_ui = UIManager.Instance.CreateFollowingUI(followUI, gameObject, true, info);
            _AfterRegisterFollowUI(current_follow_ui);
        }

        protected virtual void _AfterRegisterFollowUI(UIViewInScene uiView) {}
        #endregion

        public virtual void Damage(int damage = 1, bool knock_back = false, Entity trans_damage_from = null) {}

        public virtual void Killed() {}
    }
}