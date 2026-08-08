using UnityEngine;

namespace AvatarRotationController
{
    internal class SpineRotator
    {
        Transform spineBone => CurrentModel.AvatarMouseTracking.spineBone;
        Transform chestBone => CurrentModel.AvatarMouseTracking.chestBone;
        Transform spineDriver => CurrentModel.AvatarMouseTracking.spineDriver;
        Transform upperChestBone => CurrentModel.AvatarMouseTracking.upperChestBone;
        Quaternion spineInitRot => CurrentModel.AvatarMouseTracking.spineInitRot;
        float spineBlend => CurrentModel.AvatarMouseTracking.Inst.spineBlend;
        float spineSmoothness => CurrentModel.AvatarMouseTracking.Inst.spineSmoothness;
        float spineMinRotation => CurrentModel.AvatarMouseTracking.Inst.spineMinRotation;
        float spineMaxRotation => CurrentModel.AvatarMouseTracking.Inst.spineMaxRotation;

        Quaternion cachedDriverRotation;
        Quaternion cachedSpineRotation;
        Quaternion cachedChestRotation;
        Quaternion cachedUpperChestRotation;
        internal void OnPreLateUpdate()
        {
            if (AvatarRotationController.isTurnedAround)
            {
                cachedDriverRotation = spineDriver.localRotation;
                cachedSpineRotation = spineBone.localRotation;
                if (chestBone)
                    cachedChestRotation = chestBone.localRotation;
                if (upperChestBone)
                    cachedUpperChestRotation = upperChestBone.localRotation;
            }
        }
        internal void OnPostLateUpdate()
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
