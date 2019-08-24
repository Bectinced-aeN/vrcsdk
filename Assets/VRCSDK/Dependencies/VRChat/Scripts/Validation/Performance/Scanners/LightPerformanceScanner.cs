using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName =  "New LightPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/LightPerformanceScanner"
    )]
    #endif
    public class LightPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            List<Light> lightBuffer = new List<Light>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, lightBuffer);
            if(shouldIgnoreComponent != null)
            {
                lightBuffer.RemoveAll(c => shouldIgnoreComponent(c));
            }

            perfStats.lightCount = lightBuffer.Count;
            yield break;
        }
    }
}
