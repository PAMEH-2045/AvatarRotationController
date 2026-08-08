using UnityEngine;

namespace AvatarRotationController
{
    internal class LoopManager : MonoBehaviour
    {
        internal static LoopManager Inst;

        AvatarRotationController controller;
        SpineRotator rotator;

        void Awake()
        {
            Singleton();
            controller = new();
            rotator = new();
        }
        void Start()
        {
            CurrentModel.OnStart();
            controller.OnStart();
        }
        void OnEnable()
        {
            CurrentModel.OnAvatarSwitch += controller.OnAvatarSwitch;
        }
        void Update()
        {
            CurrentModel.OnUpdate();
        }
        void OnDisable()
        {
            CurrentModel.OnAvatarSwitch -= controller.OnAvatarSwitch;
        }
        void Singleton()
        {
            if (Inst != null && Inst != this)
            {
                Destroy(this);
            }
            else
            {
                Inst = this;
            }
        }
        internal void Update_minus10()
        {
            controller.OnPreUpdate();
        }
        internal void Update_plus10()
        {
            controller.OnPostUpdate();
        }
        internal void LateUpdate_minus10()
        {
            rotator.OnPreLateUpdate();
        }
        internal void LateUpdate_plus10()
        {
            rotator.OnPostLateUpdate();
        }
    }

    [DefaultExecutionOrder(-10)]
    internal class _minus10 : MonoBehaviour
    {
        void Update() => LoopManager.Inst?.Update_minus10();
        void LateUpdate() => LoopManager.Inst?.LateUpdate_minus10();
    }

    [DefaultExecutionOrder(+10)]
    internal class _plus10 : MonoBehaviour
    {
        void Update() => LoopManager.Inst?.Update_plus10();
        void LateUpdate() => LoopManager.Inst?.LateUpdate_plus10();
    }
}
