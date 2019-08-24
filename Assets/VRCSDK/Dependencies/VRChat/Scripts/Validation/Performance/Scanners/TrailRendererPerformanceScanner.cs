using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName = "New TrailRendererPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/TrailRendererPerformanceScanner"
    )]
    #endif
    public class TrailRendererPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            List<TrailRenderer> trailRenderers = new List<TrailRenderer>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, trailRenderers);
            if(shouldIgnoreComponent != null)
            {
                trailRenderers.RemoveAll(c => shouldIgnoreComponent(c));
            }

            int numTrailRenderers = trailRenderers.Count;

            perfStats.trailRendererCount += numTrailRenderers;
            perfStats.materialCount += numTrailRenderers;

            yield break;
        }
    }
}
