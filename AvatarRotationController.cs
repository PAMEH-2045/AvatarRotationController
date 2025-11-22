using Kirurobo;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class AvatarRotationController : MonoBehaviour
{

    public float smoothFactor = 0.1f;
    public float scrollRotationSpeed = 100f;
    public float mouseRotationSpeed = 5f;

    private float targetRotation;
    private bool isHolding;
    private Vector3 lastMousePos;
    private Transform modelRoot;
    private GameObject currentModel;
    private GameObject bubbleGO;
    private MonoBehaviour avatarScaleController;
    private AvatarAnimatorController controller;

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

        if (MenuActions.IsMovementBlocked())
            return;

        if (UniWindowController.current.isClickThrough && !isHolding)
            return;


        if (modelRoot != null)
        {
            GameObject activeModel = null;
            for (int i = 0; i < modelRoot.childCount; i++)
            {
                var child = modelRoot.GetChild(i);
                if (child.gameObject.activeInHierarchy)
                {
                    activeModel = child.gameObject;
                    break;
                }
            }

            if (activeModel != currentModel)
            {
                currentModel = activeModel;
                controller = currentModel != null ? currentModel.GetComponent<AvatarAnimatorController>() : null;
                bubbleGO = currentModel.GetComponent<AvatarBubbleHandler>().attachTarget;
            }
        }


        bool leftBtn = Input.GetMouseButton(0);
        bool alt = Input.GetKey(KeyCode.LeftAlt);

        if (isHolding)
        {
            isHolding = false;
            controller.BlockDraggingOverride = false;
        }
        if (!alt || controller.isDragging)
            return;
        if (leftBtn && alt)
        {
            isHolding = true;
            controller.BlockDraggingOverride = true;
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
        else if (mouse != 0f && alt && leftBtn)
        {
            avatarScaleController.enabled = false;
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
}