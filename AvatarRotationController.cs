using Kirurobo;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class AvatarRotationController : MonoBehaviour
{
    public float smoothFactor = 0.1f;
    public float scrollRotationSpeed = 100f;
    public float mouseRotationSpeed = 5f;
    public float turnАroundThreshold = 65f;

    bool suppressFrame;
    float targetRotation;
    bool isHolding;
    Vector3 lastMousePos;
    GameObject bubbleGO;
    MonoBehaviour avatarScaleController;
    AvatarAnimatorController controller;
    AvatarWindowHandler avatarWindowHandler;
    Camera targetCameraOrigin;
    Camera trackingCam;
    Vector3 mirroredMainCamPos;

    void Awake()
    {
        CurrentModel.OnAwake();
    }
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
        CurrentModel.OnUpdate();

        if (MenuActions.IsMovementBlocked()) return;
        if (UniWindowController.current.isClickThrough && !isHolding) return;
        if (CurrentModel.ModelGO == null) return;

        if (suppressFrame) avatarWindowHandler.targetCamera = targetCameraOrigin;

        UpdateTrackingCamera();

        bool alt = Input.GetKey(KeyCode.LeftAlt);
        bool leftBtn = Input.GetMouseButton(0);
        bool mouseRotation = alt && leftBtn;
        bool scrollRotation = alt && Input.mouseScrollDelta.y != 0f;

        UpdateHoldingState(alt, leftBtn);

        var currentMousePos = Input.mousePosition;
        var mouseDeltaX = (lastMousePos - currentMousePos).x;
        lastMousePos = currentMousePos;

        if (!(mouseRotation || scrollRotation)) return;

        avatarScaleController.enabled = false;

        targetRotation = CurrentModel.ModelGO.transform.localRotation.eulerAngles.y;

        if (mouseRotation)
        {
            targetRotation += mouseDeltaX * mouseRotationSpeed;

            avatarWindowHandler.targetCamera = null;
            suppressFrame = true;
        }
        else if (scrollRotation)
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            targetRotation += scrollDelta * scrollRotationSpeed;
        }
        
        ApplyRotation();

        avatarScaleController.enabled = true;
    }
    void OnDisable()
    {
        CurrentModel.OnAvatarSwitch -= OnAvatarSwitch;
    }
    void OnAvatarSwitch()
    {

        controller = CurrentModel.AvatarAnimatorControllerProxy.Inst;
        avatarScaleController = CurrentModel.AvatarScaleControllerProxy.Inst;

        bubbleGO = CurrentModel.AvatarBubbleHandlerProxy.Inst.attachTarget;
        avatarWindowHandler = CurrentModel.AvatarWindowHandlerProxy.Inst;
        targetCameraOrigin = avatarWindowHandler.targetCamera;

        CurrentModel.AvatarMouseTrackingProxy.mainCam = trackingCam;
    }
    void UpdateTrackingCamera()
    {
        var modelRotation = CurrentModel.ModelGO.transform.rotation.eulerAngles.y;
        if (Mathf.Abs(Mathf.DeltaAngle(0f, modelRotation)) <= turnАroundThreshold)
        {
            trackingCam.transform.position = mirroredMainCamPos;
        }
        else
        {
            trackingCam.transform.position = Camera.main.transform.position;
        }
    }
    void UpdateHoldingState(bool alt, bool leftBtn)
    {
        if (alt & leftBtn)
        {
            if (!isHolding)
            {
                isHolding = true;
                controller.BlockDraggingOverride = true;
            }
        }
        else if (isHolding)
        {
            isHolding = false;

            bool isBigScreenActive = CurrentModel.AvatarBigScreenHandlerProxy.isBigScreenActive;
            if (!isBigScreenActive)
            {
                CurrentModel.AvatarAnimatorControllerProxy.dragLockTimer = 0f;
                CurrentModel.AvatarAnimatorControllerProxy.mouseHeld = true;
                controller.BlockDraggingOverride = false;

                if (leftBtn)
                {
                    CurrentModel.AvatarAnimatorControllerProxy.SetDragging(true);
                }
            }
        }
    }
    void ApplyRotation()
    {
        float currentRotation = CurrentModel.ModelGO.transform.localRotation.eulerAngles.y;

        float t = 1f - Mathf.Pow(1f - smoothFactor, Time.deltaTime * 60f);
        //float t = 1f - Mathf.Exp(-smoothFactor * Time.deltaTime);
        float smoothedRotation = Mathf.Lerp(currentRotation, targetRotation, t);

        if (Mathf.Abs(smoothedRotation - currentRotation) > 0.0001f)
        {
            Vector3 newRotation = new Vector3(0f, smoothedRotation, 0f);
            CurrentModel.ModelGO.transform.localEulerAngles = newRotation;
            bubbleGO.transform.localEulerAngles = newRotation;
        }
    }
}