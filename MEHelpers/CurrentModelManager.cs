using MEHelper.ReflectionWrapper;
using System;
using UnityEngine;

namespace MEHelper
{
    internal class CurrentModelManager : MonoBehaviour
    {
        public static CurrentModelManager Instance { get; private set; }

        public static event Action OnAvatarSwitch;

        public Transform ModelRoot { get; private set; }

        public GameObject ModelGO { get; private set; }

        readonly float avatarScanInterval = 0.25f;
        float nextAvatarScan;

        [SerializeField]
        private GameObject settings;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);


            var modelRootGO = GameObject.Find("Model");
            if (modelRootGO != null)
                ModelRoot = modelRootGO.transform;


            var comps = settings.GetComponents<MonoBehaviour>();
            foreach (var comp in comps)
            {
                AdapterCache.UpdateOrRegister(comp);
            }
        }
        void Update()
        {
            if (Time.unscaledTime >= nextAvatarScan)
            {
                UpdateCurrentAvatar();
                nextAvatarScan = Time.unscaledTime + avatarScanInterval;
            }
        }
        void UpdateCurrentAvatar()
        {
            if (!ModelRoot) return;

            for (int i = 0; i < ModelRoot.childCount; i++)
            {
                var child = ModelRoot.GetChild(i).gameObject;
                if (!child.activeInHierarchy) continue;
                if (ModelGO == child) return;
                ModelGO = child;

                UpdateAvatarComponents();
                OnAvatarSwitch?.Invoke();

                return;
            }
        }
        void UpdateAvatarComponents()
        {
            var comps = ModelGO.GetComponents<MonoBehaviour>();
            foreach (var comp in comps)
            {
                AdapterCache.UpdateOrRegister(comp);
            }
        }
    }
}
