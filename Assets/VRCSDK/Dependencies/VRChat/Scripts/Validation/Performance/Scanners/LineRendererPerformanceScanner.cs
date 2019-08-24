using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName = "New LineRendererPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/LineRendererPerformanceScanner"
    )]
    #endif
    public class LineRendererPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            List<LineRenderer> lineRenderers = new List<LineRenderer>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, lineRenderers);
            if(shouldIgnoreComponent != null)
            {
                lineRenderers.RemoveAll(c => shouldIgnoreComponent(c));
            }

            int numLineRenderers = lineRenderers.Count;

            perfStats.lineRendererCount += numLineRenderers;
            perfStats.materialCount += numLineRenderers;

            yield break;
        }
    }
}
