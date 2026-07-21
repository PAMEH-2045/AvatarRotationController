using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class CurrentModel
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
        AvatarAnimatorControllerProxy.Inst = GetComponent<AvatarAnimatorController>();
        AvatarBigScreenHandlerProxy.Inst = GetComponent<AvatarBigScreenHandler>();
        AvatarMouseTrackingProxy.Inst = GetComponent<AvatarMouseTracking>();
        AvatarBubbleHandlerProxy.Inst = GetComponent<AvatarBubbleHandler>();
        AvatarWindowHandlerProxy.Inst = GetComponent<AvatarWindowHandler>();
    }
    public static T GetComponent<T>() where T : Component
        => gameObject.GetComponent<T>();


    public static class AvatarWindowHandlerProxy
    {
        public static AvatarWindowHandler Inst;
    }
    public static class AvatarBubbleHandlerProxy
    {
        public static AvatarBubbleHandler Inst;
    }
    public static class AvatarMouseTrackingProxy
    {
        public static AvatarMouseTracking Inst;

        static readonly Field<AvatarMouseTracking, Camera> _mainCam = new("mainCam");
        public static Camera mainCam
        {
            get => _mainCam.Getter(Inst);
            set => _mainCam.Setter(Inst, value);
        }
    }
    public static class AvatarBigScreenHandlerProxy
    {
        public static AvatarBigScreenHandler Inst;

        static readonly Field<AvatarBigScreenHandler, bool> _isBigScreenActive = new("isBigScreenActive");
        public static bool isBigScreenActive
        {
            get => _isBigScreenActive.Getter(Inst);
            set => _isBigScreenActive.Setter(Inst, value);
        }
    }
    public static class AvatarAnimatorControllerProxy
    {
        public static AvatarAnimatorController Inst;

        static readonly Field<AvatarAnimatorController, float> _dragLockTimer = new("dragLockTimer");
        public static float dragLockTimer
        {
            get => _dragLockTimer.Getter(Inst);
            set => _dragLockTimer.Setter(Inst, value);
        }

        static readonly Field<AvatarAnimatorController, bool> _mouseHeld = new("mouseHeld");
        public static bool mouseHeld
        {
            get => _mouseHeld.Getter(Inst);
            set => _mouseHeld.Setter(Inst, value);
        }

        static readonly Action<AvatarAnimatorController, bool> _SetDragging = MakeMethod<AvatarAnimatorController, Action<AvatarAnimatorController, bool>>("SetDragging");
        public static void SetDragging(bool value) => _SetDragging(Inst, value);
    }
    static TDelegate MakeMethod<TInstance, TDelegate>(string methodName) where TDelegate : Delegate
    {
        var methodInfo = typeof(TInstance).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), methodInfo);
    }
    class Field<TInstance, TField>
    {
        public Func<TInstance, TField> Getter;
        public Action<TInstance, TField> Setter;
        public Field(string fieldName)
        {
            Getter = MakeGetter<TInstance, TField>(fieldName);
            Setter = MakeSetter<TInstance, TField>(fieldName);
        }
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
