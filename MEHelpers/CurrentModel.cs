using MEHelper.ReflectionWrapper;
using System;
using UnityEngine;

namespace MEHelper
{
    public static class CurrentModel
    {
        public static event Action OnAvatarSwitch
        {
            add => CurrentModelManager.OnAvatarSwitch += value;
            remove => CurrentModelManager.OnAvatarSwitch -= value;
        }

        public static GameObject GameObject { get => CurrentModelManager.Instance.ModelGO; }
        public static Transform ModelRoot { get => CurrentModelManager.Instance.ModelRoot; }

        public static T GetComponent<T>() where T : class => 
            (T)AdapterCache.Get<T>().Obj;

        public class C<T> where T : class
        {
            private static C<T> _instance;
            public static C<T> F => _instance ?? (_instance = new C<T>());

            public static T Obj { get => (T)AdapterCache.Get<T>().Obj; }

            public static object Call(string methodName, params object[] args) =>
                AdapterCache.Get<T>().InvokeMethod(methodName, args);

            public static TReturn Call<TReturn>(string methodName, params object[] args) =>
                (TReturn)AdapterCache.Get<T>().InvokeMethod(methodName, args);

            public static TReturn Get<TReturn>(string fieldName) =>
                (TReturn)AdapterCache.Get<T>().GetValue(fieldName);

            public static void Set(string fieldName, object value) =>
                AdapterCache.Get<T>().SetValue(fieldName, value);

            public object this[string fieldName]
            {
                get => AdapterCache.Get<T>().GetValue(fieldName);
                set => AdapterCache.Get<T>().SetValue(fieldName, value);
            }
        }
    }
}
