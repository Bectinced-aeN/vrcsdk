using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance
{
    public static class AvatarPerformance
    {
        #region Public Constants

        public const int DEFAULT_DYNAMIC_BONE_MAX_SIMULATED_BONE_LIMIT = 32;
        public const int DEFAULT_DYNAMIC_BONE_MAX_COLLIDER_CHECK_LIMIT = 8;

        #if UNITY_ANDROID || UNITY_IOS
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_DEFAULT = PerformanceRating.Medium;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MIN = PerformanceRating.Medium;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MAX = PerformanceRating.Poor;
        #else
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_DEFAULT = PerformanceRating.VeryPoor;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MIN = PerformanceRating.Medium;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MAX = PerformanceRating.VeryPoor;
        #endif

        #endregion

        #region Public Delegates

        public delegate bool IgnoreDelegate(Component component);

        public delegate void FilterBlockCallback();

        public static IgnoreDelegate ShouldIgnoreComponent { get; set; }

        #endregion

        #region Public Methods

        public static AvatarPerformanceStats CalculatePerformanceStats(string avatarName, GameObject avatarObject)
        {
            AvatarPerformanceStats stats = new AvatarPerformanceStats();

            Stack<IEnumerator> coroutines = new Stack<IEnumerator>();
            coroutines.Push(CalculatePerformanceStatsEnumerator(avatarName, avatarObject, stats));
            while(coroutines.Count > 0)
            {
                IEnumerator currentCoroutine = coroutines.Peek();
                if(currentCoroutine.MoveNext())
                {
                    IEnumerator nestedCoroutine = currentCoroutine.Current as IEnumerator;
                    if(nestedCoroutine != null)
                    {
                        coroutines.Push(nestedCoroutine);
                    }
                }
                else
                {
                    coroutines.Pop();
                }
            }

            return stats;
        }

        public static IEnumerator CalculatePerformanceStatsEnumerator(string avatarName, GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            PerformanceScannerSet performanceScannerSet;
            if(VRC.ValidationHelpers.IsStandalonePlatform())
            {
                performanceScannerSet = Resources.Load<PerformanceScannerSet>("Validation/Performance/ScannerSets/PerformanceScannerSet_Windows");
            }
            else
            {
                performanceScannerSet = Resources.Load<PerformanceScannerSet>("Validation/Performance/ScannerSets/PerformanceScannerSet_Quest");
            }

            perfStats.Reset();
            perfStats.avatarName = avatarName;

            if(performanceScannerSet != null)
            {
                yield return performanceScannerSet.RunPerformanceScan(avatarObject, perfStats, ShouldIgnoreComponentInternal);
            }

            // cache performance ratings
            perfStats.CalculateAllPerformanceRatings();
        }

        public static IEnumerator ApplyPerformanceFiltersEnumerator(GameObject avatarObject, AvatarPerformanceStats perfStats, PerformanceRating minPerfRating, FilterBlockCallback onBlock)
        {
            // Performance Filtering is disabled.
            if(minPerfRating == PerformanceRating.None)
            {
                yield break;
            }

            PerformanceFilterSet performanceFilterSet;
            if(VRC.ValidationHelpers.IsStandalonePlatform())
            {
                performanceFilterSet = Resources.Load<PerformanceFilterSet>("Validation/Performance/FilterSets/PerformanceFilterSet_Windows");
            }
            else
            {
                performanceFilterSet = Resources.Load<PerformanceFilterSet>("Validation/Performance/FilterSets/PerformanceFilterSet_Quest");
            }

            bool avatarBlocked = false;
            if(performanceFilterSet != null)
            {
                yield return performanceFilterSet.ApplyPerformanceFilters(
                    avatarObject,
                    perfStats,
                    minPerfRating,
                    ShouldIgnoreComponentInternal,
                    () => { avatarBlocked = true; }
                );
            }

            if(!avatarBlocked)
            {
                yield break;
            }

            VRC.Core.Logger.LogFormat(
                "Avatar hidden due to low performance rating: [{0}] {1} - minimum setting: {2}",
                perfStats.avatarName,
                perfStats.GetPerformanceRatingForCategory(AvatarPerformanceCategory.Overall),
                minPerfRating
            );

            onBlock();
        }

        #endregion

        #region Private Methods

        private static bool ShouldIgnoreComponentInternal(Component component)
        {
            if(Application.isEditor)
            {
                if(component == null)
                {
                    return false;
                }

                if(component.CompareTag("EditorOnly"))
                {
                    return true;
                }
            }

            if(ShouldIgnoreComponent != null)
            {
                return ShouldIgnoreComponent(component);
            }

            return false;
        }

        #endregion
    }
}
