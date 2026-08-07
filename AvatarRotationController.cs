using Kirurobo;
using NAudio.CoreAudioApi;
using UnityEngine;

namespace AvatarRotationController
{
    [DefaultExecutionOrder(-10)] // Should run before AvatarScaleController to disable it properly
    public class AvatarRotationController : MonoBehaviour
    {
        [SerializeField] private float scrollRotationSpeed = 10f;
        [SerializeField] private float mouseRotationSpeed = 0.5f;
        [SerializeField] private float turnАroundThreshold = 65f;
        [SerializeField] private bool turnАround = true;
        [SerializeField] private bool blockDragging = true;

        bool isHolding;
        Vector3 lastMousePos;

        GameObject bubbleGO;

        MonoBehaviour avatarScaleController;
        AvatarAnimatorController controller;

        AvatarWindowHandler avatarWindowHandler;
        Camera targetCameraOrigin;

        Vector3 mirroredMainCamPos;
        Camera trackingCam;

        MMDevice defaultDeviceOrigin;

        public static bool isTurnedAround;

        void OnEnable()
        {
            CurrentModel.OnAvatarSwitch += OnAvatarSwitch;
        }
        void Start()
        {
            CurrentModel.OnStart();

            trackingCam = transform.GetComponentInChildren<Camera>(true);
            trackingCam.nearClipPlane = Camera.main.nearClipPlane;
            mirroredMainCamPos = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                CurrentModel.Root.position.z - Camera.main.transform.position.z
            );

            avatarScaleController = GameObject.FindFirstObjectByType<AvatarScaleController>();

            try
            {
                Settings.AddSettings(this);
            }
            catch { }
        }
        void Update()
        {
            CurrentModel.OnUpdate();

            if (CurrentModel.gameObject == null) return;
            if (UniWindowController.current.isClickThrough && !isHolding) return;

            avatarWindowHandler.targetCamera = targetCameraOrigin;

            UpdateTrackingCamera();

            bool alt = Input.GetKey(KeyCode.LeftAlt);
            bool leftBtn = Input.GetMouseButton(0);
            bool mouseRotation = alt && leftBtn;
            bool scrollRotation = alt && Input.mouseScrollDelta.y != 0f;

            UpdateHoldingState(alt, leftBtn);

            var currentMousePos = Input.mousePosition;
            var mouseDeltaX = (lastMousePos - currentMousePos).x;
            lastMousePos = currentMousePos;

            if (!mouseRotation && !scrollRotation) return;

            avatarScaleController.enabled = false;

            var targetRotation = CurrentModel.transform.localRotation.eulerAngles.y;

            if (mouseRotation)
            {
                targetRotation += mouseDeltaX * mouseRotationSpeed;

                avatarWindowHandler.targetCamera = null;
            }
            else if (scrollRotation)
            {
                float scrollDelta = Input.mouseScrollDelta.y;
                targetRotation += scrollDelta * scrollRotationSpeed;
            }

            ApplyRotation(targetRotation);

            avatarScaleController.enabled = true;
        }
        void LateUpdate()
        {
            SpineRotator.OnPreLateUpdate();
        }
        void OnDisable()
        {
            CurrentModel.OnAvatarSwitch -= OnAvatarSwitch;
        }
        void OnAvatarSwitch()
        {
            controller = CurrentModel.AvatarAnimatorController.Inst;

            bubbleGO = CurrentModel.AvatarBubbleHandler.Inst.attachTarget;
            avatarWindowHandler = CurrentModel.AvatarWindowHandler.Inst;
            targetCameraOrigin = avatarWindowHandler.targetCamera;

            CurrentModel.AvatarMouseTracking.mainCam = trackingCam;

            defaultDeviceOrigin = CurrentModel.AvatarAnimatorController.defaultDevice;
        }
        void UpdateTrackingCamera()
        {
            if (!SaveLoadHandler.Instance.data.enableMouseTracking)
            {
                if (isTurnedAround)
                {
                    trackingCam.transform.position = Camera.main.transform.position;
                    isTurnedAround = false;
                }  
                
                return;
            }

            var modelRotation = CurrentModel.transform.rotation.eulerAngles.y;
            if (Mathf.Abs(Mathf.DeltaAngle(0f, modelRotation)) <= turnАroundThreshold && turnАround)
            {
                trackingCam.transform.position = mirroredMainCamPos;
                isTurnedAround = true;
            }
            else
            {
                trackingCam.transform.position = Camera.main.transform.position;
                isTurnedAround = false;
            }
        }
        void UpdateHoldingState(bool alt, bool leftBtn)
        {
            if (alt & leftBtn)
            {
                if (!isHolding)
                {
                    isHolding = true;
                    CurrentModel.AvatarAnimatorController.defaultDevice = null;
                    if (blockDragging) controller.BlockDraggingOverride = true;
                }
            }
            else if (isHolding)
            {
                isHolding = false;

                CurrentModel.AvatarAnimatorController.defaultDevice = defaultDeviceOrigin;

                if (!CurrentModel.IsBigScreenActive && blockDragging)
                {
                    CurrentModel.AvatarAnimatorController.dragLockTimer = 0f;
                    CurrentModel.AvatarAnimatorController.mouseHeld = true;
                    controller.BlockDraggingOverride = false;

                    if (leftBtn)
                    {
                        CurrentModel.AvatarAnimatorController.SetDragging(true);
                    }
                }
            }
        }
        void ApplyRotation(float targetRotation)
        {
            Vector3 newRotation = new Vector3(0f, targetRotation, 0f);
            CurrentModel.transform.localEulerAngles = newRotation;
            bubbleGO.transform.localEulerAngles = newRotation;
        }
    }
}