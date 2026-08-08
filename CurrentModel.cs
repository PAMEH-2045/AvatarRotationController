using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace AvatarRotationController
{
    internal class CurrentModel
    {
        public static event Action OnAvatarSwitch;
        public static GameObject gameObject { get; private set; }
        public static Transform Root { get; private set; }
        public static Transform transform => gameObject.transform;

        static readonly float avatarScanInterval = 0.25f;
        static float nextAvatarScan;

        public static void OnStart()
        {
            var modelRootGO = GameObject.Find("Model");
            if (modelRootGO != null)
                Root = modelRootGO.transform;
        }
        public static void OnUpdate()
        {
            if (Time.unscaledTime >= nextAvatarScan)
            {
                UpdateCurrentAvatar();
                nextAvatarScan = Time.unscaledTime + avatarScanInterval;
            }
        }
        static void UpdateCurrentAvatar()
        {
            if (!Root) return;

            for (int i = 0; i < Root.childCount; i++)
            {
                var child = Root.GetChild(i).gameObject;
                if (!child.activeInHierarchy) continue;
                if (gameObject == child) return;
                gameObject = child;

                UpdateAvatarComponents();
                OnAvatarSwitch?.Invoke();

                return;
            }
        }
        static void UpdateAvatarComponents()
        {
            AvatarAnimatorController.Inst = GetComponent<global::AvatarAnimatorController>();
            AvatarBigScreenHandler.Inst = GetComponent<global::AvatarBigScreenHandler>();
            AvatarMouseTracking.Inst = GetComponent<global::AvatarMouseTracking>();
            AvatarBubbleHandler.Inst = GetComponent<global::AvatarBubbleHandler>();
            AvatarWindowHandler.Inst = GetComponent<global::AvatarWindowHandler>();
        }
        public static T GetComponent<T>() where T : Component
            => gameObject.GetComponent<T>();


        public static class AvatarWindowHandler
        {
            public static global::AvatarWindowHandler Inst;
        }
        public static class AvatarBubbleHandler
        {
            public static global::AvatarBubbleHandler Inst;
        }
        public static class AvatarMouseTracking
        {
            public static global::AvatarMouseTracking Inst;

            static readonly Field<global::AvatarMouseTracking, Camera> _mainCam = new("mainCam");
            public static Camera mainCam
            {
                get => _mainCam.Getter(Inst);
                set => _mainCam.Setter(Inst, value);
            }
            public static Quaternion spineInitRot => _spineInitRot.Getter(Inst);
            static readonly Field<global::AvatarMouseTracking, Quaternion> _spineInitRot = new("spineInitRot");
            public static Transform spineBone => _spineBone.Getter(Inst);
            static readonly Field<global::AvatarMouseTracking, Transform> _spineBone = new("spineBone");
            public static Transform chestBone => _chestBone.Getter(Inst);
            static readonly Field<global::AvatarMouseTracking, Transform> _chestBone = new("chestBone");
            public static Transform spineDriver => _spineDriver.Getter(Inst);
            static readonly Field<global::AvatarMouseTracking, Transform> _spineDriver = new("spineDriver");
            public static Transform upperChestBone => _upperChestBone.Getter(Inst);
            static readonly Field<global::AvatarMouseTracking, Transform> _upperChestBone = new("upperChestBone");
            public static float spineTrackingWeight
            {
                get => _spineTrackingWeight.Getter(Inst);
                set => _spineTrackingWeight.Setter(Inst, value);
            }
            static readonly Field<global::AvatarMouseTracking, float> _spineTrackingWeight = new("spineTrackingWeight");
        }
        public static bool IsBigScreenActive => AvatarBigScreenHandler.isBigScreenActive;
        public static class AvatarBigScreenHandler
        {
            public static global::AvatarBigScreenHandler Inst;

            public static bool isBigScreenActive => _isBigScreenActive.Getter(Inst);
            static readonly Field<global::AvatarBigScreenHandler, bool> _isBigScreenActive = new("isBigScreenActive");
        }
        public static class AvatarAnimatorController
        {
            public static global::AvatarAnimatorController Inst;

            static readonly Field<global::AvatarAnimatorController, float> _dragLockTimer = new("dragLockTimer");
            public static float dragLockTimer
            {
                get => _dragLockTimer.Getter(Inst);
                set => _dragLockTimer.Setter(Inst, value);
            }

            static readonly Field<global::AvatarAnimatorController, bool> _mouseHeld = new("mouseHeld");
            public static bool mouseHeld
            {
                get => _mouseHeld.Getter(Inst);
                set => _mouseHeld.Setter(Inst, value);
            }

            static readonly Field<global::AvatarAnimatorController, bool> _isDragging = new("isDragging");
            public static bool isDragging
            {
                get => _isDragging.Getter(Inst);
                set => _isDragging.Setter(Inst, value);
            }

            public static void SetDragging(bool value) => _SetDragging(Inst, value);
            static readonly Action<global::AvatarAnimatorController, bool> _SetDragging = MakeMethod<global::AvatarAnimatorController, Action<global::AvatarAnimatorController, bool>>("SetDragging");
        }
        static TDelegate MakeMethod<TInstance, TDelegate>(string methodName) where TDelegate : Delegate
        {
            var methodInfo = typeof(TInstance).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), methodInfo);
        }
        class Field<TInstance, TField>(string fieldName)
        {
            public Func<TInstance, TField> Getter = MakeGetter<TInstance, TField>(fieldName);
            public Action<TInstance, TField> Setter = MakeSetter<TInstance, TField>(fieldName);
        }
        static Action<TInstance, TField> MakeSetter<TInstance, TField>(string fieldName)
        {
            var fieldInfo = typeof(TInstance).GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            var instanceParam = Expression.Parameter(typeof(TInstance));
            MemberExpression fieldAccess;
            if (fieldInfo.IsStatic)
                fieldAccess = Expression.Field(null, fieldInfo);
            else
                fieldAccess = Expression.Field(instanceParam, fieldInfo);
            var value = Expression.Parameter(typeof(TField));
            var assign = Expression.Assign(fieldAccess, value);

            var lambda = Expression.Lambda<Action<TInstance, TField>>(assign, instanceParam, value);

            return lambda.Compile();
        }
        static Func<TInstance, TField> MakeGetter<TInstance, TField>(string fieldName)
        {
            var fieldInfo = typeof(TInstance).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var instanceParam = Expression.Parameter(typeof(TInstance));
            MemberExpression fieldAccess;
            if (fieldInfo.IsStatic)
                fieldAccess = Expression.Field(null, fieldInfo);
            else
                fieldAccess = Expression.Field(instanceParam, fieldInfo);

            var lambda = Expression.Lambda<Func<TInstance, TField>>(fieldAccess, instanceParam);

            return lambda.Compile();
        }
    }
}