using Kirurobo;
using MEHelper;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class AvatarRotationController : MonoBehaviour
{
    public float smoothFactor = 0.1f;
    public float scrollRotationSpeed = 100f;
    public float mouseRotationSpeed = 5f;

    bool suppressFrame;
    float targetRotation;
    bool isHolding;
    Vector3 lastMousePos;
    GameObject currentModel;
    GameObject bubbleGO;
    MonoBehaviour avatarScaleController;
    AvatarAnimatorController controller;
    AvatarWindowHandler avatarWindowHandler;
    Camera targetCameraOrigin;
    Camera trackingCam;
    Vector3 mirroredMainCamPos;

    void OnEnable()
    {
        CurrentModel.OnAvatarSwitch += OnAvatarSwitch;
    }
    void Start()
    {
        trackingCam = transform.GetComponentInChildren<Camera>(true);
        trackingCam.nearClipPlane = Camera.main.nearClipPlane;
        mirroredMainCamPos = new Vector3(
            Camera.main.transform.position.x, 
            Camera.main.transform.position.y,
            CurrentModel.ModelRoot.position.z - Camera.main.transform.position.z
        );
    }
    void Update()
    {
        if (MenuActions.IsMovementBlocked()) return;

        if (UniWindowController.current.isClickThrough && !isHolding) return;

        if (!currentModel) return;

        if (suppressFrame) avatarWindowHandler.targetCamera = targetCameraOrigin;

        var modelRotation = CurrentModel.GameObject.transform.rotation.eulerAngles.y;
        if (modelRotation <= 65f || modelRotation >= 295f)
        {
            trackingCam.transform.position = mirroredMainCamPos;
        }
        else
        {
            trackingCam.transform.position = Camera.main.transform.position;
        }

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
            var isBigScreenActive = CurrentModel.C<AvatarBigScreenHandler>.Get<bool>("isBigScreenActive");
            if (!isBigScreenActive)
            {
                CurrentModel.C<AvatarAnimatorController>.Set("dragLockTimer", 0f);
                CurrentModel.C<AvatarAnimatorController>.Set("mouseHeld", true);
                controller.BlockDraggingOverride = false;
                if (leftBtn)
                {
                    CurrentModel.C<AvatarAnimatorController>.Call("SetDragging", true);
                }
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

            avatarWindowHandler.targetCamera = null;
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
    void OnDisable()
    {
        CurrentModel.OnAvatarSwitch -= OnAvatarSwitch;
    }
    void OnAvatarSwitch()
    {
        currentModel = CurrentModel.GameObject;

        controller = CurrentModel.GetComponent<AvatarAnimatorController>();
        avatarScaleController = CurrentModel.GetComponent<AvatarScaleController>();

        bubbleGO = CurrentModel.GetComponent<AvatarBubbleHandler>().attachTarget;
        avatarWindowHandler = CurrentModel.GetComponent<AvatarWindowHandler>();
        targetCameraOrigin = avatarWindowHandler.targetCamera;

        CurrentModel.C<AvatarMouseTracking>.F["mainCam"] = trackingCam;
    }
}