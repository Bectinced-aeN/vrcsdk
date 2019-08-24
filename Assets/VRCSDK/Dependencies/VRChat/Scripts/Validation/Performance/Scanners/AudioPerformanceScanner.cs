using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName =  "New AudioPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/AudioPerformanceScanner"
    )]
    #endif
    public class AudioPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            List<AudioSource> audioSourceBuffer = new List<AudioSource>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, audioSourceBuffer);
            if(shouldIgnoreComponent != null)
            {
                audioSourceBuffer.RemoveAll(c => shouldIgnoreComponent(c));
            }

            perfStats.audioSourceCount = audioSourceBuffer.Count;

            yield break;
        }
    }
}
