using UnityEngine;

namespace AvatarRotationController
{
    [DefaultExecutionOrder(+10)] // Should run after AvatarMouseTracking to override its values
    internal class SpineRotator : MonoBehaviour
    {
        static Transform spineBone => CurrentModel.AvatarMouseTracking.spineBone;
        static Transform chestBone => CurrentModel.AvatarMouseTracking.chestBone;
        static Transform spineDriver => CurrentModel.AvatarMouseTracking.spineDriver;
        static Transform upperChestBone => CurrentModel.AvatarMouseTracking.upperChestBone;
        static Quaternion spineInitRot => CurrentModel.AvatarMouseTracking.spineInitRot;
        static float spineBlend => CurrentModel.AvatarMouseTracking.Inst.spineBlend;
        static float spineSmoothness => CurrentModel.AvatarMouseTracking.Inst.spineSmoothness;
        static float spineMinRotation => CurrentModel.AvatarMouseTracking.Inst.spineMinRotation;
        static float spineMaxRotation => CurrentModel.AvatarMouseTracking.Inst.spineMaxRotation;

        static Quaternion cachedDriverRotation;
        static Quaternion cachedSpineRotation;
        static Quaternion cachedChestRotation;
        static Quaternion cachedUpperChestRotation;
        internal static void OnPreLateUpdate() // LateUpdate before AvatarMouseTracking
        {
            if (AvatarRotationController.isTurnedAround)
            {
                cachedDriverRotation = spineDriver.localRotation;
                cachedSpineRotation = spineBone.localRotation;
                cachedChestRotation = chestBone.localRotation;
                cachedUpperChestRotation = upperChestBone.localRotation;
            }
        }
        void LateUpdate() // LateUpdate after AvatarMouseTracking
        {
            if (AvatarRotationController.isTurnedAround)
            {
                DoReversedSpine();
            }
        }
        void DoReversedSpine()
        {
            if (!spineBone || !spineDriver) return;

            //float targetW = IsAllowed("Spine") ? 1f : 0f;
            //spineTrackingWeight = Mathf.MoveTowards(spineTrackingWeight, targetW, Time.deltaTime * spineFadeSpeed);

            float normY = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
            float targetY = Mathf.Lerp(spineMinRotation, spineMaxRotation, normY);

            spineDriver.localRotation = Quaternion.Slerp(cachedDriverRotation, Quaternion.Euler(0f, targetY, 0f), Time.deltaTime * spineSmoothness);
            //spineDriver.localRotation = Quaternion.Slerp(spineDriver.localRotation, Quaternion.Euler(0f, -targetY, 0f), Time.deltaTime * spineSmoothness);

            var baseRot = cachedSpineRotation;
            var delta = spineDriver.localRotation * Quaternion.Inverse(spineInitRot);

            float applied = CurrentModel.AvatarMouseTracking.spineTrackingWeight * spineBlend;
            var offset = Quaternion.Slerp(Quaternion.identity, delta, applied);

            spineBone.localRotation = offset * baseRot;
            if (chestBone)
                chestBone.localRotation = Quaternion.Slerp(Quaternion.identity, delta, 0.8f * applied) * cachedChestRotation;
            if (upperChestBone)
                upperChestBone.localRotation = Quaternion.Slerp(Quaternion.identity, delta, 0.6f * applied) * cachedUpperChestRotation;
        }
    }
}
