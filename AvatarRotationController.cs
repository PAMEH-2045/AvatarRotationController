using Kirurobo;
using System.Reflection;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class AvatarRotationController : MonoBehaviour
{

    public float smoothFactor = 0.1f;
    public float scrollRotationSpeed = 100f;
    public float mouseRotationSpeed = 5f;

    float avatarScanInterval = 0.25f;
    float nextAvatarScan;
    bool suppressFrame;
    float targetRotation;
    bool isHolding;
    Vector3 lastMousePos;
    Transform modelRoot;
    GameObject currentModel;
    GameObject bubbleGO;
    MonoBehaviour avatarScaleController;
    AvatarAnimatorController controller;
    FieldInfo controllerDragLockTimer;
    FieldInfo controllerMouseHeld;
    MethodInfo controllerSetDragging;
    AvatarWindowHandler avatarWindowHandler;
    FieldInfo targetCamera;
    Camera targetCameraOrigin;

    void Start()
    {
        var settings = GameObject.FindAnyObjectByType<AvatarScaleController>();
        avatarScaleController = settings.GetComponent<AvatarScaleController>();

        var modelRootGO = GameObject.Find("Model");
        if (modelRootGO != null)
            modelRoot = modelRootGO.transform;
    }

    void Update()
    {

        if (MenuActions.IsMovementBlocked()) return;

        if (UniWindowController.current.isClickThrough && !isHolding) return;

        if (Time.unscaledTime >= nextAvatarScan)
        {
            UpdateCurrentAvatar();
            nextAvatarScan = Time.unscaledTime + avatarScanInterval;
        }

        if (!currentModel) return;

        if (suppressFrame) targetCamera.SetValue(avatarWindowHandler, targetCameraOrigin);


        bool leftBtn = Input.GetMouseButton(0);
        bool alt = Input.GetKey(KeyCode.LeftAlt);

        if (leftBtn && alt)
        {
            isHolding = true;
            controller.BlockDraggingOverride = true;
        }
        else if (isHolding)
        {
            isHolding = false;
            controllerDragLockTimer.SetValue(controller, 0f);
            controllerMouseHeld.SetValue(controller, true);
            controller.BlockDraggingOverride = false;

            if (leftBtn)
            {
                controllerSetDragging.Invoke(controller, new object[] { true });
            }
        }


        var currentMousePos = Input.mousePosition;
        var mouse = (lastMousePos - currentMousePos).x;
        lastMousePos = currentMousePos;

        float scroll = Input.mouseScrollDelta.y;

        targetRotation = currentModel.transform.localRotation.eulerAngles.y;
        if (scroll != 0f && alt)
        {
            avatarScaleController.enabled = false;
            targetRotation = targetRotation + scroll * scrollRotationSpeed;

        }
        else if (alt && leftBtn)
        {
            avatarScaleController.enabled = false;

            targetCamera.SetValue(avatarWindowHandler, null);
            suppressFrame = true;

            targetRotation = targetRotation + mouse * mouseRotationSpeed;
        }

        float current = currentModel.transform.localRotation.eulerAngles.y;
        float smoothed = Mathf.Lerp(
            current,
            targetRotation,
            1f - Mathf.Pow(1f - smoothFactor, Time.deltaTime * 60f)
        );

        if (Mathf.Abs(smoothed - current) > 0.0001f)
        {
            currentModel.transform.localEulerAngles = new Vector3(0, smoothed, 0);
            bubbleGO.transform.localEulerAngles = new Vector3(0, smoothed, 0);
        }

        avatarScaleController.enabled = true;
    }

    void OnAvatarSwitch()
    {
        controller = currentModel.GetComponent<AvatarAnimatorController>();
        controllerDragLockTimer = typeof(AvatarAnimatorController).GetField("dragLockTimer", BindingFlags.Instance | BindingFlags.NonPublic);
        controllerMouseHeld = typeof(AvatarAnimatorController).GetField("mouseHeld", BindingFlags.Instance | BindingFlags.NonPublic);
        controllerSetDragging = typeof(AvatarAnimatorController).GetMethod("SetDragging", BindingFlags.Instance | BindingFlags.NonPublic);

        bubbleGO = currentModel.GetComponent<AvatarBubbleHandler>().attachTarget;

        avatarWindowHandler = currentModel.GetComponent<AvatarWindowHandler>();
        targetCamera = typeof(AvatarWindowHandler).GetField("targetCamera", BindingFlags.Instance | BindingFlags.Public);
        targetCameraOrigin = (Camera)targetCamera.GetValue(avatarWindowHandler);
    }

    void UpdateCurrentAvatar()
    {
        if (!modelRoot) return;

        for (int i = 0; i < modelRoot.childCount; i++)
        {
            var child = modelRoot.GetChild(i).gameObject;
            if (!child.activeInHierarchy) continue;
            if (currentModel == child) return;
            currentModel = child;

            OnAvatarSwitch();

            return;
        }
    }
}