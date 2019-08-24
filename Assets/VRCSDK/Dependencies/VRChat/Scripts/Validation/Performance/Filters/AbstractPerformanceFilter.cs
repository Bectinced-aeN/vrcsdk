using System.Collections;
using UnityEngine;
using VRCSDK2.Validation.Performance;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Filters
{
    public abstract class AbstractPerformanceFilter : ScriptableObject
    {
        public abstract IEnumerator ApplyPerformanceFilter(
            GameObject avatarObject,
            AvatarPerformanceStats perfStats,
            PerformanceRating ratingLimit,
            AvatarPerformance.IgnoreDelegate shouldIgnoreComponent,
            AvatarPerformance.FilterBlockCallback onBlock
        );
    }
}
