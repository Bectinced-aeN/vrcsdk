using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName =  "New AnimatorPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/AnimatorPerformanceScanner"
    )]
    #endif
    public class AnimatorPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            int animatorCount = 0;

            List<Animator> animatorBuffer = new List<Animator>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, animatorBuffer);
            if(shouldIgnoreComponent != null)
            {
                animatorBuffer.RemoveAll(c => shouldIgnoreComponent(c));
            }

            animatorCount += animatorBuffer.Count;

            yield return null;

            List<Animation> animationBuffer = new List<Animation>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, animationBuffer);
            if(shouldIgnoreComponent != null)
            {
                animationBuffer.RemoveAll(c => shouldIgnoreComponent(c));
            }

            animatorCount += animationBuffer.Count;

            perfStats.animatorCount = animatorCount;

            yield return null;
        }
    }
}
