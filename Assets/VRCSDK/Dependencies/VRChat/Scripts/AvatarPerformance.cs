using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VRCSDK2
{
    public enum PerformanceRating
    {
        None = 0,
        VeryGood = 1,
        Good = 2,
        Medium = 3,
        Bad = 4,
        VeryBad = 5
    }

    public enum AvatarPerformanceCategory
    {
        None,

        Overall,

        PolyCount,
        AABB,
        SkinnedMeshCount,
        MeshCount,
        MaterialCount,
        DynamicBoneComponentCount,
        DynamicBoneSimulatedBoneCount,
        DynamicBoneColliderCount,
        DynamicBoneCollisionCheckCount,
        AnimatorCount,
        BoneCount,
        LightCount,
        ParticleSystemCount,
        ParticleTotalCount,
        ParticleTotalActiveMeshPolyCount,
        ParticleTrailsEnabled,
        ParticleCollisionEnabled,
        TrailRendererCount,
        LineRendererCount,
        ClothCount,
        ClothMaxVertices,
        PhysicsColliderCount,
        PhysicsRigidbodyCount,
        AudioSourceCount,

        AvatarPerformanceCategoryCount
    }

    public enum PerformanceInfoDisplayLevel
    {
        None,

        Verbose,
        Info,
        Warning,
        Error
    }

    public class AvatarPerformanceStats
    {
        #region Performance Stats Properties
        public string AvatarName;

        public int PolyCount;
        public Bounds AABB;
        public int SkinnedMeshCount;
        public int MeshCount;
        public int MaterialCount;
        public int AnimatorCount;
        public int BoneCount;
        public int LightCount;
        public int ParticleSystemCount;
        public int ParticleTotalCount;
        public int ParticleTotalActiveMeshPolyCount;
        public bool ParticleTrailsEnabled;
        public bool ParticleCollisionEnabled;
        public int TrailRendererCount;
        public int LineRendererCount;
        public int DynamicBoneComponentCount;
        public int DynamicBoneSimulatedBoneCount;
        public int DynamicBoneColliderCount;
        public int DynamicBoneCollisionCheckCount;          // number of collider simulated bones excluding the root multiplied by the number of colliders
        public int ClothCount;
        public int ClothMaxVertices;
        public int PhysicsColliderCount;
        public int PhysicsRigidbodyCount;
        public int AudioSourceCount;

        #endregion

        #region Performance Rating Stats

        private static AvatarPerformanceStatsLevelSet _performanceStatsLevelSet = null;

        public static AvatarPerformanceStatsLevel VeryGoodPerformanceStatLimits
        {
            get { return _performanceStatsLevelSet.VeryGood; }
        }

        public static AvatarPerformanceStatsLevel GoodPerformanceStatLimits
        {
            get { return _performanceStatsLevelSet.Good; }
        }

        public static AvatarPerformanceStatsLevel MediumPerformanceStatLimits
        {
            get { return _performanceStatsLevelSet.Medium; }
        }

        public static AvatarPerformanceStatsLevel BadPerformanceStatLimits
        {
            get { return _performanceStatsLevelSet.Bad; }
        }

        public static void Initialize()
        {
            if (_performanceStatsLevelSet != null)
                return;

            _performanceStatsLevelSet = Resources.Load<AvatarPerformanceStatsLevelSet>(GetPlatformPerformanceStatLevels());
        }

        public static IEnumerator InitializeAsync()
        {
            if (_performanceStatsLevelSet != null)
                yield break;

            ResourceRequest req = Resources.LoadAsync<AvatarPerformanceStatsLevelSet>(GetPlatformPerformanceStatLevels());
            while (!req.isDone)
                yield return null;

            _performanceStatsLevelSet = req.asset as AvatarPerformanceStatsLevelSet;
        }

        private static string GetPlatformPerformanceStatLevels()
        {
            if (VRC.ValidationHelpers.IsStandalonePlatform())
                return "PerformanceStatsLevels/Windows/AvatarPerformanceStatLevels_Windows";
            else
                return "PerformanceStatsLevels/Quest/AvatarPerformanceStatLevels_Quest";
        }

        #endregion

        #region Private Fields

        private readonly PerformanceRating[] _performanceRatingCache;

        #endregion

        #region Constructors

        public AvatarPerformanceStats()
        {
            _performanceRatingCache = new PerformanceRating[(int)AvatarPerformanceCategory.AvatarPerformanceCategoryCount];
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            AvatarName = null;
            PolyCount = 0;
            AABB = new Bounds(Vector3.zero, Vector3.zero);
            SkinnedMeshCount = 0;
            MeshCount = 0;
            MaterialCount = 0;
            AnimatorCount = 0;
            BoneCount = 0;
            LightCount = 0;
            ParticleSystemCount = 0;
            ParticleTotalCount = 0;
            ParticleTotalActiveMeshPolyCount = 0;
            ParticleTrailsEnabled = false;
            ParticleCollisionEnabled = false;
            TrailRendererCount = 0;
            LineRendererCount = 0;
            DynamicBoneComponentCount = 0;
            DynamicBoneSimulatedBoneCount = 0;
            DynamicBoneColliderCount = 0;
            DynamicBoneCollisionCheckCount = 0;
            ClothCount = 0;
            ClothMaxVertices = 0;
            PhysicsColliderCount = 0;
            PhysicsRigidbodyCount = 0;
            AudioSourceCount = 0;
        }

        public PerformanceRating GetPerformanceRatingForCategory(AvatarPerformanceCategory perfCategory)
        {
            if (_performanceRatingCache[(int)perfCategory] == PerformanceRating.None)
            {
                _performanceRatingCache[(int)perfCategory] = CalculatePerformanceRatingForCategory(perfCategory);
            }

            return _performanceRatingCache[(int)perfCategory];
        }

        public void CalculateAllPerformanceRatings()
        {
            for (int i = 0; i < _performanceRatingCache.Length; i++)
            {
                _performanceRatingCache[i] = PerformanceRating.None;
            }

            foreach (AvatarPerformanceCategory perfCategory in Enum.GetValues(typeof(AvatarPerformanceCategory)))
            {
                if (perfCategory == AvatarPerformanceCategory.None ||
                    perfCategory == AvatarPerformanceCategory.AvatarPerformanceCategoryCount)
                {
                    continue;
                }

                if (_performanceRatingCache[(int)perfCategory] == PerformanceRating.None)
                {
                    _performanceRatingCache[(int)perfCategory] = CalculatePerformanceRatingForCategory(perfCategory);
                }
            }
        }

        public void GetSDKPerformanceInfoText(out string text, out PerformanceInfoDisplayLevel displayLevel, AvatarPerformanceCategory perfCategory, PerformanceRating rating)
        {
            text = "";
            displayLevel = PerformanceInfoDisplayLevel.None;

            switch (perfCategory)
            {
                case AvatarPerformanceCategory.Overall:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Info;
                            text = string.Format("Overall Performance: {0}", GetPerformanceRatingDisplayName(rating));
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Overall Performance: {0} - This avatar may not perform well on many systems. See additional warnings for suggestions on how to improve performance. Click 'Avatar Optimization Tips' below for more information.", GetPerformanceRatingDisplayName(rating));
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Overall Performance: {0} - This avatar does not meet minimum performance requirements for VRChat. " +
                                (VRC.ValidationHelpers.IsMobilePlatform() ? "It will be blocked by default on VRChat for Quest, and will not show unless a user chooses to show your avatar." : "It may be blocked by users depending on their Performance settings.") +
                                " See additional warnings for suggestions on how to improve performance. Click 'Avatar Optimization Tips' below for more information.", GetPerformanceRatingDisplayName(rating));
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.PolyCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            displayLevel = PerformanceInfoDisplayLevel.Info;
                            text = string.Format("Polygons: {0}", PolyCount);
                            break;
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Info;
                            text = string.Format("Polygons: {0} (Recommended: {1})", PolyCount, VeryGoodPerformanceStatLimits.PolyCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Polygons: {0} - Please try to reduce your avatar poly count to less than {1} (Recommended: {2})", PolyCount, GoodPerformanceStatLimits.PolyCount, VeryGoodPerformanceStatLimits.PolyCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Polygons: {0} - This avatar has too many polygons. " +
                                (VRC.ValidationHelpers.IsMobilePlatform() ? "It will be blocked by default on VRChat for Quest, and will not show unless a user chooses to show your avatar." : "It may be blocked by users depending on their Performance settings.") +
                                " It should have less than {1}. VRChat recommends having less than {2}.", PolyCount, BadPerformanceStatLimits.PolyCount, VeryGoodPerformanceStatLimits.PolyCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.AABB:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Bounding box (AABB) size: {0}", AABB.size.ToString());
                            break;
                        case PerformanceRating.Good:
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Bounding box (AABB) size: {0} (Recommended: {1})", AABB.size.ToString(), VeryGoodPerformanceStatLimits.AABB.size.ToString());
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("This avatar's bounding box (AABB) is too large on at least one axis. Current size: {0}, Maximum size: {1}", AABB.size.ToString(), BadPerformanceStatLimits.AABB.size.ToString());
                            break;
                    }

                    break;
                case AvatarPerformanceCategory.SkinnedMeshCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Skinned Mesh Renderers: {0}", SkinnedMeshCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Skinned Mesh Renderers: {0} (Recommended: {1}) - Combine multiple skinned meshes for optimal performance.", SkinnedMeshCount, VeryGoodPerformanceStatLimits.SkinnedMeshCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Skinned Mesh Renderers: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many skinned meshes. Combine multiple skinned meshes for optimal performance.", SkinnedMeshCount, BadPerformanceStatLimits.SkinnedMeshCount, VeryGoodPerformanceStatLimits.SkinnedMeshCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.MeshCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Mesh Renderers: {0}", MeshCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Mesh Renderers: {0} (Recommended: {1}) - Combine multiple meshes for optimal performance.", MeshCount, VeryGoodPerformanceStatLimits.MeshCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Mesh Renderers: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many meshes. Combine multiple meshes for optimal performance.", MeshCount, BadPerformanceStatLimits.MeshCount, VeryGoodPerformanceStatLimits.MeshCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.MaterialCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Material Slots: {0}", MaterialCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Material Slots: {0} (Recommended: {1}) - Combine materials and atlas textures for optimal performance.", MaterialCount, VeryGoodPerformanceStatLimits.MaterialCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Material Slots: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many materials. Combine materials and atlas textures for optimal performance.", MaterialCount, BadPerformanceStatLimits.MaterialCount, VeryGoodPerformanceStatLimits.MaterialCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.AnimatorCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Animator Count: {0}", AnimatorCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Animator Count: {0} (Recommended: {1}) - Avoid using extra Animators for optimal performance.", AnimatorCount, VeryGoodPerformanceStatLimits.AnimatorCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Animator Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many Animators. Avoid using extra Animators for optimal performance.", AnimatorCount, BadPerformanceStatLimits.AnimatorCount, VeryGoodPerformanceStatLimits.AnimatorCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.BoneCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Bones: {0}", BoneCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Bones: {0} (Recommended: {1}) - Reduce number of bones for optimal performance.", BoneCount, VeryGoodPerformanceStatLimits.BoneCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Bones: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many bones. Reduce number of bones for optimal performance.", BoneCount, BadPerformanceStatLimits.BoneCount, VeryGoodPerformanceStatLimits.BoneCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.LightCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Lights: {0}", LightCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Lights: {0} (Recommended: {1}) - Avoid use of dynamic lights for optimal performance.", LightCount, VeryGoodPerformanceStatLimits.LightCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Lights: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many dynamic lights. Avoid use of dynamic lights for optimal performance.", LightCount, BadPerformanceStatLimits.LightCount, VeryGoodPerformanceStatLimits.LightCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.ParticleSystemCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Particle Systems: {0}", ParticleSystemCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Particle Systems: {0} (Recommended: {1}) - Reduce number of particle systems for better performance.", ParticleSystemCount, VeryGoodPerformanceStatLimits.ParticleSystemCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Particle Systems: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many particle systems. Reduce number of particle systems for better performance.", ParticleSystemCount, BadPerformanceStatLimits.ParticleSystemCount, VeryGoodPerformanceStatLimits.ParticleSystemCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.ParticleTotalCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Total Combined Max Particle Count: {0}", ParticleTotalCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Total Combined Max Particle Count: {0} (Recommended: {1}) - Reduce 'Max Particles' across all particle systems for better performance.", ParticleTotalCount, VeryGoodPerformanceStatLimits.ParticleTotalCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Total Combined Max Particle Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar uses too many particles. Reduce 'Max Particles' across all particle systems for better performance.", ParticleTotalCount, BadPerformanceStatLimits.ParticleTotalCount, VeryGoodPerformanceStatLimits.ParticleTotalCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.ParticleTotalActiveMeshPolyCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Mesh Particle Active Total Poly Count: {0}", ParticleTotalActiveMeshPolyCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Mesh Particle Active Total Poly Count: {0} (Recommended: {1}) - Reduce number of polygons in particle meshes, and reduce 'Max Particles' for better performance.", ParticleTotalActiveMeshPolyCount, VeryGoodPerformanceStatLimits.ParticleTotalActiveMeshPolyCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Mesh Particle Active Total Poly Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar uses too many mesh particle polygons. Reduce number of polygons in particle meshes, and reduce 'Max Particles' for better performance.", ParticleTotalActiveMeshPolyCount, BadPerformanceStatLimits.ParticleTotalCount, VeryGoodPerformanceStatLimits.ParticleTotalActiveMeshPolyCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.ParticleTrailsEnabled:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Particle Trails Enabled: {0}", ParticleTrailsEnabled);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Particle Trails Enabled: {0} (Recommended: {1}) - Avoid particle trails for better performance.", ParticleTrailsEnabled, VeryGoodPerformanceStatLimits.ParticleTrailsEnabled);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.ParticleCollisionEnabled:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Particle Collision Enabled: {0}", ParticleCollisionEnabled);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Particle Collision Enabled: {0} (Recommended: {1}) - Avoid particle collision for better performance.", ParticleCollisionEnabled, VeryGoodPerformanceStatLimits.ParticleCollisionEnabled);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.TrailRendererCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Trail Renderers: {0}", TrailRendererCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Trail Renderers: {0} (Recommended: {1}) - Reduce number of TrailRenderers for better performance.", TrailRendererCount, VeryGoodPerformanceStatLimits.TrailRendererCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Trail Renderers: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many TrailRenderers. Reduce number of TrailRenderers for better performance.", TrailRendererCount, BadPerformanceStatLimits.TrailRendererCount, VeryGoodPerformanceStatLimits.TrailRendererCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.LineRendererCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Line Renderers: {0}", LineRendererCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Line Renderers: {0} (Recommended: {1}) - Reduce number of LineRenderers for better performance.", LineRendererCount, VeryGoodPerformanceStatLimits.LineRendererCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Line Renderers: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many LineRenderers. Reduce number of LineRenderers for better performance.", LineRendererCount, BadPerformanceStatLimits.LineRendererCount, VeryGoodPerformanceStatLimits.LineRendererCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.DynamicBoneComponentCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Dynamic Bone Components: {0}", DynamicBoneComponentCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Components: {0} (Recommended: {1}) - Reduce number of DynamicBone components for better performance.", DynamicBoneComponentCount, VeryGoodPerformanceStatLimits.DynamicBoneComponentCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Components: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many DynamicBone components. Reduce number of DynamicBone components for better performance.", DynamicBoneComponentCount, BadPerformanceStatLimits.DynamicBoneComponentCount, VeryGoodPerformanceStatLimits.DynamicBoneComponentCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.DynamicBoneSimulatedBoneCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Dynamic Bone Simulated Bone Count: {0}", DynamicBoneSimulatedBoneCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Simulated Bone Count: {0} (Recommended: {1}) - Reduce number of transforms in hierarchy under DynamicBone components, or set EndLength or EndOffset to zero to reduce the number of simulated bones.", DynamicBoneSimulatedBoneCount, VeryGoodPerformanceStatLimits.DynamicBoneSimulatedBoneCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Simulated Bone Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many bones simulated by DynamicBone. Reduce number of transforms in hierarchy under DynamicBone components, or set EndLength or EndOffset to zero to reduce the number of simulated bones.", DynamicBoneSimulatedBoneCount, BadPerformanceStatLimits.DynamicBoneSimulatedBoneCount, VeryGoodPerformanceStatLimits.DynamicBoneSimulatedBoneCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.DynamicBoneColliderCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Dynamic Bone Collider Count: {0}", DynamicBoneColliderCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Collider Count: {0} (Recommended: {1}) - Avoid use of DynamicBoneColliders for better performance.", DynamicBoneColliderCount, VeryGoodPerformanceStatLimits.DynamicBoneColliderCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Collider Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many DynamicBoneColliders. Avoid use of DynamicBoneColliders for better performance.", DynamicBoneColliderCount, BadPerformanceStatLimits.DynamicBoneColliderCount, VeryGoodPerformanceStatLimits.DynamicBoneColliderCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.DynamicBoneCollisionCheckCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Dynamic Bone Collision Check Count: {0}", DynamicBoneCollisionCheckCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Collision Check Count: {0} (Recommended: {1}) - Avoid use of DynamicBoneColliders for better performance.", DynamicBoneCollisionCheckCount, VeryGoodPerformanceStatLimits.DynamicBoneCollisionCheckCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Dynamic Bone Collision Check Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many DynamicBoneColliders. Avoid use of DynamicBoneColliders for better performance.", DynamicBoneCollisionCheckCount, BadPerformanceStatLimits.DynamicBoneCollisionCheckCount, VeryGoodPerformanceStatLimits.DynamicBoneCollisionCheckCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.ClothCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Cloth Component Count: {0}", ClothCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Cloth Component Count: {0} (Recommended: {1}) - Avoid use of cloth for optimal performance.", ClothCount, VeryGoodPerformanceStatLimits.ClothCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Cloth Component Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many Cloth components. Avoid use of cloth for optimal performance.", ClothCount, BadPerformanceStatLimits.ClothCount, VeryGoodPerformanceStatLimits.ClothCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.ClothMaxVertices:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Cloth Total Vertex Count: {0}", ClothMaxVertices);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Cloth Total Vertex Count: {0} (Recommended: {1}) - Reduce number of vertices in cloth meshes for improved performance.", ClothMaxVertices, VeryGoodPerformanceStatLimits.ClothMaxVertices);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Cloth Total Vertex Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many vertices in cloth meshes. Reduce number of vertices in cloth meshes for improved performance.", ClothMaxVertices, BadPerformanceStatLimits.ClothMaxVertices, VeryGoodPerformanceStatLimits.ClothMaxVertices);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.PhysicsColliderCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Physics Collider Count: {0}", PhysicsColliderCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Physics Collider Count: {0} (Recommended: {1}) - Avoid use of colliders for optimal performance.", PhysicsColliderCount, VeryGoodPerformanceStatLimits.PhysicsColliderCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Physics Collider Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many colliders. Avoid use of colliders for optimal performance.", PhysicsColliderCount, BadPerformanceStatLimits.PhysicsColliderCount, VeryGoodPerformanceStatLimits.PhysicsColliderCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.PhysicsRigidbodyCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Physics Rigidbody Count: {0}", PhysicsRigidbodyCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Physics Rigidbody Count: {0} (Recommended: {1}) - Avoid use of rigidbodies for optimal performance.", PhysicsRigidbodyCount, VeryGoodPerformanceStatLimits.PhysicsRigidbodyCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Physics Rigidbody Count: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many rigidbodies. Avoid use of rigidbodies for optimal performance.", PhysicsRigidbodyCount, BadPerformanceStatLimits.PhysicsRigidbodyCount, VeryGoodPerformanceStatLimits.PhysicsRigidbodyCount);
                            break;
                    }
                    break;
                case AvatarPerformanceCategory.AudioSourceCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                        case PerformanceRating.Good:
                            displayLevel = PerformanceInfoDisplayLevel.Verbose;
                            text = string.Format("Audio Sources: {0}", AudioSourceCount);
                            break;
                        case PerformanceRating.Medium:
                        case PerformanceRating.Bad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Audio Sources: {0} (Recommended: {1}) - Reduce number of audio sources for better performance.", AudioSourceCount, VeryGoodPerformanceStatLimits.AudioSourceCount);
                            break;
                        case PerformanceRating.VeryBad:
                            displayLevel = PerformanceInfoDisplayLevel.Warning;
                            text = string.Format("Audio Sources: {0} (Maximum: {1}, Recommended: {2}) - This avatar has too many audio sources. Reduce number of audio sources for better performance.", AudioSourceCount, BadPerformanceStatLimits.AudioSourceCount, VeryGoodPerformanceStatLimits.AudioSourceCount);
                            break;
                    }
                    break;
                default:
                    text = "";
                    displayLevel = PerformanceInfoDisplayLevel.None;
                    break;
            }
        }

        public string GetStatTextForCategory(AvatarPerformanceCategory perfCategory, PerformanceRating rating)
        {
            switch (perfCategory)
            {
                case AvatarPerformanceCategory.Overall:
                    return string.Format("Overall Performance: {0}", GetPerformanceRatingDisplayName(rating));
                case AvatarPerformanceCategory.PolyCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Polygons: {0}", PolyCount);
                        default:
                        {
                            if(PolyCount == int.MaxValue)
                            {
                                return "Polygons: Mesh Read/Write Disabled";
                            }
                            return string.Format("Polygons: {0} ({1})", PolyCount, VeryGoodPerformanceStatLimits.PolyCount);
                        }
                    }
                case AvatarPerformanceCategory.AABB:
                    switch (rating)
                    {
                        case PerformanceRating.VeryBad:
                            return string.Format("Bounds Size: {0} ({1})", AABB.size.ToString(), BadPerformanceStatLimits.AABB.size.ToString());
                        default:
                            return string.Format("Bounds Size: {0}", AABB.size.ToString());
                    }

                case AvatarPerformanceCategory.SkinnedMeshCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Skinned Meshes: {0}", SkinnedMeshCount);
                        default:
                            return string.Format("Skinned Meshes: {0} ({1})", SkinnedMeshCount, VeryGoodPerformanceStatLimits.SkinnedMeshCount);
                    }
                case AvatarPerformanceCategory.MeshCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Meshes: {0}", MeshCount);
                        default:
                            return string.Format("Meshes: {0} ({1})", MeshCount, VeryGoodPerformanceStatLimits.MeshCount);

                    }
                case AvatarPerformanceCategory.MaterialCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Material Slots: {0}", MaterialCount);
                        default:
                            return string.Format("Material Slots: {0} ({1})", MaterialCount, VeryGoodPerformanceStatLimits.MaterialCount);
                    }
                case AvatarPerformanceCategory.AnimatorCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Animators: {0}", AnimatorCount);
                        default:
                            return string.Format("Animators: {0} ({1})", AnimatorCount, VeryGoodPerformanceStatLimits.AnimatorCount);
                    }
                case AvatarPerformanceCategory.BoneCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Bones: {0}", BoneCount);
                        default:
                            return string.Format("Bones: {0} ({1})", BoneCount, VeryGoodPerformanceStatLimits.BoneCount);
                    }
                case AvatarPerformanceCategory.LightCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Lights: {0}", LightCount);
                        default:
                            return string.Format("Lights: {0} ({1})", LightCount, VeryGoodPerformanceStatLimits.LightCount);
                    }
                case AvatarPerformanceCategory.ParticleSystemCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Particle Systems: {0}", ParticleSystemCount);
                        default:
                            return string.Format("Particle Systems: {0} ({1})", ParticleSystemCount, VeryGoodPerformanceStatLimits.ParticleSystemCount);
                    }
                case AvatarPerformanceCategory.ParticleTotalCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Total Particles Active: {0}", ParticleTotalCount);
                        default:
                            return string.Format("Total Particles Active: {0} ({1})", ParticleTotalCount, VeryGoodPerformanceStatLimits.ParticleTotalCount);
                    }
                case AvatarPerformanceCategory.ParticleTotalActiveMeshPolyCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Mesh Particle Active Polygons: {0}", ParticleTotalActiveMeshPolyCount);
                        default:
                            return string.Format("Mesh Particle Active Polygons: {0} ({1})", ParticleTotalActiveMeshPolyCount, VeryGoodPerformanceStatLimits.ParticleTotalActiveMeshPolyCount);
                    }
                case AvatarPerformanceCategory.ParticleTrailsEnabled:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Particle Trails Enabled: {0}", ParticleTrailsEnabled);
                        default:
                            return string.Format("Particle Trails Enabled: {0} ({1})", ParticleTrailsEnabled, VeryGoodPerformanceStatLimits.ParticleTrailsEnabled);
                    }
                case AvatarPerformanceCategory.ParticleCollisionEnabled:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Particle Collision Enabled: {0}", ParticleCollisionEnabled);
                        default:
                            return string.Format("Particle Collision Enabled: {0} ({1})", ParticleCollisionEnabled, VeryGoodPerformanceStatLimits.ParticleCollisionEnabled);
                    }
                case AvatarPerformanceCategory.TrailRendererCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Trail Renderers: {0}", TrailRendererCount);
                        default:
                            return string.Format("Trail Renderers: {0} ({1})", TrailRendererCount, VeryGoodPerformanceStatLimits.TrailRendererCount);
                    }
                case AvatarPerformanceCategory.LineRendererCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Line Renderers: {0}", LineRendererCount);
                        default:
                            return string.Format("Line Renderers: {0} ({1})", LineRendererCount, VeryGoodPerformanceStatLimits.LineRendererCount);
                    }
                case AvatarPerformanceCategory.DynamicBoneComponentCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Dynamic Bone Components: {0}", DynamicBoneComponentCount);
                        default:
                            return string.Format("Dynamic Bone Components: {0} ({1})", DynamicBoneComponentCount, VeryGoodPerformanceStatLimits.DynamicBoneComponentCount);
                    }
                case AvatarPerformanceCategory.DynamicBoneSimulatedBoneCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Dynamic Bone Transforms: {0}", DynamicBoneSimulatedBoneCount);
                        default:
                            return string.Format("Dynamic Bone Transforms: {0} ({1})", DynamicBoneSimulatedBoneCount, VeryGoodPerformanceStatLimits.DynamicBoneSimulatedBoneCount);
                    }
                case AvatarPerformanceCategory.DynamicBoneColliderCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Dynamic Bone Colliders: {0}", DynamicBoneColliderCount);
                        default:
                            return string.Format("Dynamic Bone Colliders: {0} ({1})", DynamicBoneColliderCount, VeryGoodPerformanceStatLimits.DynamicBoneColliderCount);
                    }
                case AvatarPerformanceCategory.DynamicBoneCollisionCheckCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Dynamic Bone Collision Check Count: {0}", DynamicBoneCollisionCheckCount);
                        default:
                            return string.Format("Dynamic Bone Collision Check Count: {0} ({1})", DynamicBoneCollisionCheckCount, VeryGoodPerformanceStatLimits.DynamicBoneCollisionCheckCount);
                    }
                case AvatarPerformanceCategory.ClothCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Cloths: {0}", ClothCount);
                        default:
                            return string.Format("Cloths: {0} ({1})", ClothCount, VeryGoodPerformanceStatLimits.ClothCount);
                    }
                case AvatarPerformanceCategory.ClothMaxVertices:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Total Cloth Vertices: {0}", ClothMaxVertices);
                        default:
                            return string.Format("Total Cloth Vertices: {0} ({1})", ClothMaxVertices, VeryGoodPerformanceStatLimits.ClothMaxVertices);
                    }
                case AvatarPerformanceCategory.PhysicsColliderCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Physics Colliders: {0}", PhysicsColliderCount);
                        default:
                            return string.Format("Physics Colliders: {0} ({1})", PhysicsColliderCount, VeryGoodPerformanceStatLimits.PhysicsColliderCount);
                    }
                case AvatarPerformanceCategory.PhysicsRigidbodyCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Physics Rigidbodies: {0}", PhysicsRigidbodyCount);
                        default:
                            return string.Format("Physics Rigidbodies: {0} ({1})", PhysicsRigidbodyCount, VeryGoodPerformanceStatLimits.PhysicsRigidbodyCount);
                    }
                case AvatarPerformanceCategory.AudioSourceCount:
                    switch (rating)
                    {
                        case PerformanceRating.VeryGood:
                            return string.Format("Audio Sources: {0}", AudioSourceCount);
                        default:
                            return string.Format("Audio Sources: {0} ({1})", AudioSourceCount, VeryGoodPerformanceStatLimits.AudioSourceCount);
                    }
                default:
                    return "#AvatarPerformanceCategory - " + perfCategory;
            }
        }

        public static string GetPerformanceRatingDisplayName(PerformanceRating rating)
        {
            switch (rating)
            {
                case PerformanceRating.VeryGood:
                    return "Excellent";
                case PerformanceRating.Good:
                    return "Good";
                case PerformanceRating.Medium:
                    return "Medium";
                case PerformanceRating.Bad:
                    return "Poor";
                case PerformanceRating.VeryBad:
                    return "Very Poor";
                default:
                    return "None";
            }
        }

        #endregion

        #region Private Methods

        private delegate int ComparePerformanceStatsDelegate(AvatarPerformanceStats stats, AvatarPerformanceStatsLevel statsLevel);

        private PerformanceRating CalculatePerformanceRatingForCategory(AvatarPerformanceCategory perfCategory)
        {
            switch (perfCategory)
            {
                case AvatarPerformanceCategory.Overall:
                    {
                        PerformanceRating maxRating = PerformanceRating.None;

                        foreach (AvatarPerformanceCategory category in Enum.GetValues(typeof(AvatarPerformanceCategory)))
                        {
                            if (category == AvatarPerformanceCategory.None ||
                                category == AvatarPerformanceCategory.Overall ||
                                category == AvatarPerformanceCategory.AvatarPerformanceCategoryCount)
                            {
                                continue;
                            }

                            PerformanceRating rating = GetPerformanceRatingForCategory(category);
                            
                            // Bounds don't affect the Overall rating beyond medium.
                            if(category == AvatarPerformanceCategory.AABB)
                            {
                                rating = rating > PerformanceRating.Medium ? PerformanceRating.Medium : rating;
                            }

                            if (rating > maxRating)
                            {
                                maxRating = rating;
                            }
                        }

                        return maxRating;
                    }
                case AvatarPerformanceCategory.PolyCount:
                    return CalculatePerformanceRating((x, y) => x.PolyCount - y.PolyCount);
                case AvatarPerformanceCategory.AABB:
                {
                    return CalculatePerformanceRating((x, y) => 
                        ApproxLessOrEqual(y.AABB.extents.x, 0.0f) ||    // -1 extents means "no AABB limit"
                        (
                            ApproxLessOrEqual(x.AABB.extents.x, y.AABB.extents.x) &&
                            ApproxLessOrEqual(x.AABB.extents.y, y.AABB.extents.y) &&
                            ApproxLessOrEqual(x.AABB.extents.z, y.AABB.extents.z)) ? -1 : 1
                        );
                }
                case AvatarPerformanceCategory.SkinnedMeshCount:
                    return CalculatePerformanceRating((x, y) => x.SkinnedMeshCount - y.SkinnedMeshCount);
                case AvatarPerformanceCategory.MeshCount:
                    return CalculatePerformanceRating((x, y) => x.MeshCount - y.MeshCount);
                case AvatarPerformanceCategory.MaterialCount:
                    return CalculatePerformanceRating((x, y) => x.MaterialCount - y.MaterialCount);
                case AvatarPerformanceCategory.AnimatorCount:
                    return CalculatePerformanceRating((x, y) => x.AnimatorCount - y.AnimatorCount);
                case AvatarPerformanceCategory.BoneCount:
                    return CalculatePerformanceRating((x, y) => x.BoneCount - y.BoneCount);
                case AvatarPerformanceCategory.LightCount:
                    return CalculatePerformanceRating((x, y) => x.LightCount - y.LightCount);
                case AvatarPerformanceCategory.ParticleSystemCount:
                    return CalculatePerformanceRating((x, y) => x.ParticleSystemCount - y.ParticleSystemCount);
                case AvatarPerformanceCategory.ParticleTotalCount:
                    return CalculatePerformanceRating((x, y) => x.ParticleTotalCount - y.ParticleTotalCount);
                case AvatarPerformanceCategory.ParticleTotalActiveMeshPolyCount:
                    return CalculatePerformanceRating((x, y) => x.ParticleTotalActiveMeshPolyCount - y.ParticleTotalActiveMeshPolyCount);
                case AvatarPerformanceCategory.ParticleTrailsEnabled:
                    return CalculatePerformanceRating((x, y) =>
                    {
                        if (x.ParticleTrailsEnabled == y.ParticleTrailsEnabled)
                        {
                            return 0;
                        }

                        return x.ParticleTrailsEnabled ? 1 : -1;
                    });
                case AvatarPerformanceCategory.ParticleCollisionEnabled:
                    return CalculatePerformanceRating((x, y) =>
                    {
                        if (x.ParticleCollisionEnabled == y.ParticleCollisionEnabled)
                        {
                            return 0;
                        }

                        return x.ParticleCollisionEnabled ? 1 : -1;
                    });
                case AvatarPerformanceCategory.TrailRendererCount:
                    return CalculatePerformanceRating((x, y) => x.TrailRendererCount - y.TrailRendererCount);
                case AvatarPerformanceCategory.LineRendererCount:
                    return CalculatePerformanceRating((x, y) => x.LineRendererCount - y.LineRendererCount);
                case AvatarPerformanceCategory.DynamicBoneComponentCount:
                    return CalculatePerformanceRating((x, y) => x.DynamicBoneComponentCount - y.DynamicBoneComponentCount);
                case AvatarPerformanceCategory.DynamicBoneSimulatedBoneCount:
                    return CalculatePerformanceRating((x, y) => x.DynamicBoneSimulatedBoneCount - y.DynamicBoneSimulatedBoneCount);
                case AvatarPerformanceCategory.DynamicBoneColliderCount:
                    return CalculatePerformanceRating((x, y) => x.DynamicBoneColliderCount - y.DynamicBoneColliderCount);
                case AvatarPerformanceCategory.DynamicBoneCollisionCheckCount:
                    return CalculatePerformanceRating((x, y) => x.DynamicBoneCollisionCheckCount - y.DynamicBoneCollisionCheckCount);
                case AvatarPerformanceCategory.ClothCount:
                    return CalculatePerformanceRating((x, y) => x.ClothCount - y.ClothCount);
                case AvatarPerformanceCategory.ClothMaxVertices:
                    return CalculatePerformanceRating((x, y) => x.ClothMaxVertices - y.ClothMaxVertices);
                case AvatarPerformanceCategory.PhysicsColliderCount:
                    return CalculatePerformanceRating((x, y) => x.PhysicsColliderCount - y.PhysicsColliderCount);
                case AvatarPerformanceCategory.PhysicsRigidbodyCount:
                    return CalculatePerformanceRating((x, y) => x.PhysicsRigidbodyCount - y.PhysicsRigidbodyCount);
                case AvatarPerformanceCategory.AudioSourceCount:
                    return CalculatePerformanceRating((x, y) => x.AudioSourceCount - y.AudioSourceCount);
                default:
                    return PerformanceRating.None;
            }
        }

        private PerformanceRating CalculatePerformanceRating(ComparePerformanceStatsDelegate compareFn)
        {
            if (compareFn(this, VeryGoodPerformanceStatLimits) <= 0)
            {
                return PerformanceRating.VeryGood;
            }

            if (compareFn(this, GoodPerformanceStatLimits) <= 0)
            {
                return PerformanceRating.Good;
            }

            if (compareFn(this, MediumPerformanceStatLimits) <= 0)
            {
                return PerformanceRating.Medium;
            }

            if (compareFn(this, BadPerformanceStatLimits) <= 0)
            {
                return PerformanceRating.Bad;
            }

            return PerformanceRating.VeryBad;
        }

        private static bool ApproxLessOrEqual(float x1, float x2)
        {
            float r = x1 - x2;
            return r < 0.0f || Mathf.Approximately(r, 0.0f);
        }
        
        #endregion

        #region Overrides

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("AvatarName: {0}\n", AvatarName);
            sb.AppendFormat("Overall Performance: {0}\n", GetPerformanceRatingForCategory(AvatarPerformanceCategory.Overall));
            sb.AppendFormat("PolyCount: {0}\n", PolyCount);
            sb.AppendFormat("Bounds: {0}\n", AABB.ToString());
            sb.AppendFormat("SkinnedMeshCount: {0}\n", SkinnedMeshCount);
            sb.AppendFormat("MeshCount: {0}\n", MeshCount);
            sb.AppendFormat("MaterialCount: {0}\n", MaterialCount);
            sb.AppendFormat("AnimatorCount: {0}\n", AnimatorCount);
            sb.AppendFormat("BoneCount: {0}\n", BoneCount);
            sb.AppendFormat("LightCount: {0}\n", LightCount);
            sb.AppendFormat("ParticleSystemCount: {0}\n", ParticleSystemCount);
            sb.AppendFormat("ParticleTotalCount: {0}\n", ParticleTotalCount);
            sb.AppendFormat("ParticleTotalActiveMeshPolyCount: {0}\n", ParticleTotalActiveMeshPolyCount);
            sb.AppendFormat("ParticleTrailsEnabled: {0}\n", ParticleTrailsEnabled);
            sb.AppendFormat("ParticleCollisionEnabled: {0}\n", ParticleCollisionEnabled);
            sb.AppendFormat("TrailRendererCount: {0}\n", TrailRendererCount);
            sb.AppendFormat("LineRendererCount: {0}\n", LineRendererCount);
            sb.AppendFormat("DynamicBoneComponentCount: {0}\n", DynamicBoneComponentCount);
            sb.AppendFormat("DynamicBoneSimulatedBoneCount: {0}\n", DynamicBoneSimulatedBoneCount);
            sb.AppendFormat("DynamicBoneColliderCount: {0}\n", DynamicBoneColliderCount);
            sb.AppendFormat("DynamicBoneCollisionCheckCount: {0}\n", DynamicBoneCollisionCheckCount);
            sb.AppendFormat("ClothCount: {0}\n", ClothCount);
            sb.AppendFormat("ClothMaxVertices: {0}\n", ClothMaxVertices);
            sb.AppendFormat("PhysicsColliderCount: {0}\n", PhysicsColliderCount);
            sb.AppendFormat("PhysicsRigidbodyCount: {0}\n", PhysicsRigidbodyCount);

            return sb.ToString();
        }

        #endregion
    }

    public static class AvatarPerformance
    {
        #region Public Constants

        public const int DEFAULT_DYNAMIC_BONE_MAX_SIMULATED_BONE_LIMIT = 32;
        public const int DEFAULT_DYNAMIC_BONE_MAX_COLLIDER_CHECK_LIMIT = 8;

        #if UNITY_ANDROID || UNITY_IOS
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_DEFAULT = PerformanceRating.Medium;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MIN = PerformanceRating.Medium;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MAX = PerformanceRating.Bad;
        #else
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_DEFAULT = PerformanceRating.VeryBad;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MIN = PerformanceRating.Medium;
        internal const PerformanceRating AVATAR_PERFORMANCE_RATING_MINIMUM_TO_DISPLAY_MAX = PerformanceRating.VeryBad;
        #endif

        #endregion

        #region Private Fields

        private static readonly object initLock = new object();

        private static Type _dynamicBoneType = null;
        private static FieldInfo _dynamicBoneRootFieldInfo = null;
        private static FieldInfo _dynamicBoneExclusionsFieldInfo = null;
        private static FieldInfo _dynamicBoneCollidersFieldInfo = null;
        private static FieldInfo _dynamicBoneEndLengthFieldInfo = null;
        private static FieldInfo _dynamicBoneEndOffsetFieldInfo = null;
        private static bool _searchedOptionalTypes = false;
        private static readonly List<int> _triangleBuffer = new List<int>();
        private static readonly bool INCLUDE_INACTIVE_OBJECTS_IN_STATS = true;

        #endregion

        #region Public Delegates

        public delegate bool IgnoreDelegate(Component component);
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
            lock (initLock)
            {
                if (!_searchedOptionalTypes)
                {
                    FindOptionalTypes();
                    _searchedOptionalTypes = true;
                }
            }

            perfStats.Reset();
            perfStats.AvatarName = avatarName;

            yield return AnalyzeGraphics(avatarObject, perfStats);
            yield return AnalyzeAnimators(avatarObject, perfStats);

            AnalyzeLights(avatarObject, perfStats);
            yield return null;

            AnalyzeDynamicBone(avatarObject, perfStats);
            yield return null;

            AnalyzeCloth(avatarObject, perfStats);
            yield return null;

            yield return AnalyzePhysics(avatarObject, perfStats);

            AnalyzeAudioSources(avatarObject, perfStats);
            yield return null;

            // cache performance ratings
            perfStats.CalculateAllPerformanceRatings();
        }

        #endregion

        #region Private Methods

        private static bool ShouldIgnorePhysicsComponent(Component component)
        {
            if (ShouldIgnoreComponent != null && ShouldIgnoreComponent(component))
            {
                return true;
            }

            if (component.GetComponent<VRC_Station>() != null)
            {
                return true;
            }

            return false;
        }

        private static uint CalculateRendererPolyCount(Renderer renderer)
        {
            Mesh sharedMesh = null;
            SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
            if(skinnedMeshRenderer != null)
            {
                sharedMesh = skinnedMeshRenderer.sharedMesh;
            }

            if(sharedMesh == null)
            {
                MeshRenderer meshRenderer = renderer as MeshRenderer;
                if(meshRenderer != null)
                {
                    MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        sharedMesh = meshFilter.sharedMesh;
                    }
                }
            }

            if(sharedMesh == null)
            {
                return 0;
            }

            return GetMeshPolyCount(sharedMesh);
        }

        private static uint GetMeshPolyCount(Mesh sourceMesh)
        {
            // We can't get the Triangle Count if the mesh isn't readable so just return a huge number.
            // The SDK Control Panel should show a warning in this case.
            if (!sourceMesh.isReadable)
            {
                return uint.MaxValue;
            }

            uint count = 0;
            for(int i = 0; i < sourceMesh.subMeshCount; i++)
            {
                sourceMesh.GetTriangles(_triangleBuffer, i);
                count += (uint)_triangleBuffer.Count / 3;
                _triangleBuffer.Clear();
            }

            return count;
        }
        private static IEnumerator AnalyzeGraphics(GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            List<Renderer> rendererBuffer = new List<Renderer>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, rendererBuffer);
            if(ShouldIgnoreComponent != null)
            {
                rendererBuffer.RemoveAll(c => ShouldIgnoreComponent(c));
            }

            AnalyzeGeometry(avatarObject, rendererBuffer, perfStats);
            yield return null;

            // renderers
            AnalyzeRenderers(rendererBuffer, perfStats);
            yield return null;
        }

        private static IEnumerator AnalyzeAnimators(GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            int animatorCount = 0;

            List<Animator> animatorBuffer = new List<Animator>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, animatorBuffer);
            if(ShouldIgnoreComponent != null)
            {
                animatorBuffer.RemoveAll(c => ShouldIgnoreComponent(c));
            }

            animatorCount += animatorBuffer.Count;

            yield return null;

            List<Animation> animationBuffer = new List<Animation>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, animationBuffer);
            if(ShouldIgnoreComponent != null)
            {
                animationBuffer.RemoveAll(c => ShouldIgnoreComponent(c));
            }

            animatorCount += animationBuffer.Count;

            perfStats.AnimatorCount = animatorCount;

            yield return null;
        }

        private static IEnumerator AnalyzePhysics(GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            List<Collider> colliderBuffer = new List<Collider>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, colliderBuffer);
            colliderBuffer.RemoveAll(ShouldIgnorePhysicsComponent);
            perfStats.PhysicsColliderCount = colliderBuffer.Count;

            yield return null;

            List<Rigidbody> rigidbodyBuffer = new List<Rigidbody>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, rigidbodyBuffer);
            rigidbodyBuffer.RemoveAll(ShouldIgnorePhysicsComponent);
            perfStats.PhysicsRigidbodyCount = rigidbodyBuffer.Count;

            yield return null;
        }

        private static void AnalyzeLights(GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            List<Light> lightBuffer = new List<Light>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, lightBuffer);
            if(ShouldIgnoreComponent != null)
            {
                lightBuffer.RemoveAll(c => ShouldIgnoreComponent(c));
            }

            perfStats.LightCount = lightBuffer.Count;
        }

        private static void AnalyzeCloth(GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            int totalClothVertices = 0;

            List<Cloth> clothBuffer = new List<Cloth>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, clothBuffer);
            if(ShouldIgnoreComponent != null)
            {
                clothBuffer.RemoveAll(c => ShouldIgnoreComponent(c));
            }

            perfStats.ClothCount = clothBuffer.Count;

            foreach(Cloth cloth in clothBuffer)
            {
                if(cloth == null)
                {
                    continue;
                }

                Vector3[] clothVertices = cloth.vertices;
                if(clothVertices == null)
                {
                    continue;
                }

                totalClothVertices += clothVertices.Length;
            }

            perfStats.ClothMaxVertices = totalClothVertices;
        }

        private static void AnalyzeAudioSources(GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            List<AudioSource> audioSourceBuffer = new List<AudioSource>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, audioSourceBuffer);
            if(ShouldIgnoreComponent != null)
            {
                audioSourceBuffer.RemoveAll(c => ShouldIgnoreComponent(c));
            }

            perfStats.AudioSourceCount = audioSourceBuffer.Count;
        }

        private static void AnalyzeGeometry(GameObject avatarObject, IEnumerable<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            ulong polyCount = 0;
            Bounds bounds = new Bounds(avatarObject.transform.position, Vector3.zero);

            List<Renderer> rendererIgnoreBuffer = new List<Renderer>(16);

            List<LODGroup> lodBuffer = new List<LODGroup>(16);
            avatarObject.GetComponentsInChildren(INCLUDE_INACTIVE_OBJECTS_IN_STATS, lodBuffer);
            try
            {
                foreach (LODGroup lodGroup in lodBuffer)
                {
                    LOD[] lodLevels = lodGroup.GetLODs();

                    ulong highestLodPolyCount = 0;
                    foreach (LOD lod in lodLevels)
                    {
                        uint thisLodPolyCount = 0;
                        foreach (Renderer renderer in lod.renderers)
                        {
                            rendererIgnoreBuffer.Add(renderer);
                            checked
                            {
                                thisLodPolyCount += CalculateRendererPolyCount(renderer);
                            }
                        }
                        if (thisLodPolyCount > highestLodPolyCount)
                        {
                            highestLodPolyCount = thisLodPolyCount;
                        }
                    }

                    checked
                    {
                        polyCount += highestLodPolyCount;
                    }
                }
            }
            catch(OverflowException e)
            {
                polyCount = uint.MaxValue;
            }

            foreach (Renderer renderer in renderers)
            {
                if ((renderer as ParticleSystemRenderer) == null)
                {
                    bounds.Encapsulate(renderer.bounds);
                }

                if(rendererIgnoreBuffer.Contains(renderer))
                {
                    continue;
                }

                polyCount += CalculateRendererPolyCount(renderer);
            }

            bounds.center -= avatarObject.transform.position;

            rendererIgnoreBuffer.Clear();
            lodBuffer.Clear();

            perfStats.PolyCount = polyCount > int.MaxValue ? int.MaxValue : (int)polyCount;
            perfStats.AABB = bounds;
        }

        private static void AnalyzeSkinnedMeshRenderers(IEnumerable<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            int count = 0;
            int materialSlots = 0;
            int skinnedBoneCount = 0;

            HashSet<Transform> transformIgnoreBuffer = new HashSet<Transform>();
            foreach(Renderer renderer in renderers)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
                if(skinnedMeshRenderer == null)
                {
                    continue;
                }

                count++;

                Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
                if(sharedMesh != null)
                {
                    materialSlots += sharedMesh.subMeshCount;
                }

                // bone count
                Transform[] bones = skinnedMeshRenderer.bones;
                foreach (Transform bone in bones)
                {
                    if (bone == null || transformIgnoreBuffer.Contains(bone))
                    {
                        continue;
                    }

                    transformIgnoreBuffer.Add(bone);
                    skinnedBoneCount++;
                }
            }
            transformIgnoreBuffer.Clear();

            perfStats.SkinnedMeshCount += count;
            perfStats.MaterialCount += materialSlots;
            perfStats.BoneCount += skinnedBoneCount;
        }

        private static void AnalyzeMeshRenderers(IEnumerable<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            int count = 0;
            int materialSlots = 0;
            foreach(Renderer renderer in renderers)
            {
                MeshRenderer meshRenderer = renderer as MeshRenderer;
                if(meshRenderer == null)
                {
                    continue;
                }

                count++;

                MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
                Mesh sharedMesh = meshFilter.sharedMesh;
                if(sharedMesh != null)
                {
                    materialSlots += sharedMesh.subMeshCount;
                }
            }

            perfStats.MeshCount += count;
            perfStats.MaterialCount += materialSlots;
        }

        private static void AnalyzeTrailRenderers(IEnumerable<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            int numTrailRenderers = renderers.Count(o => o is TrailRenderer);

            perfStats.TrailRendererCount += numTrailRenderers;
            perfStats.MaterialCount += numTrailRenderers;
        }

        private static void AnalyzeLineRenderers(IEnumerable<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            int numLineRenderers = renderers.Count(o => o is LineRenderer);

            perfStats.LineRendererCount += numLineRenderers;
            perfStats.MaterialCount += numLineRenderers;
        }

        private static void AnalyzeParticleSystemRenderers(IEnumerable<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            int particleSystemCount = 0;
            ulong particleTotalCount = 0;
            ulong particleTotalActiveMeshPolyCount = 0;
            bool particleTrailsEnabled = false;
            bool particleCollisionEnabled = false;
            int materialSlots = 0;

            foreach (Renderer renderer in renderers)
            {
                ParticleSystemRenderer particleRenderer = renderer as ParticleSystemRenderer;
                if(particleRenderer == null)
                {
                    continue;
                }

                ParticleSystem particleSystem = particleRenderer.GetComponent<ParticleSystem>();

                int particleCount = particleSystem.main.maxParticles;
                if (particleCount <= 0)
                {
                    continue;
                }

                particleSystemCount++;
                materialSlots++;
                particleTotalCount += (uint)particleCount;

                // mesh particles
                if (particleRenderer.renderMode == ParticleSystemRenderMode.Mesh && particleRenderer.meshCount > 0)
                {
                    uint highestPolyCount = 0;

                    Mesh[] meshes = new Mesh[particleRenderer.meshCount];
                    int particleRendererMeshCount = particleRenderer.GetMeshes(meshes);
                    for (int meshIndex = 0; meshIndex < particleRendererMeshCount; meshIndex++)
                    {
                        Mesh mesh = meshes[meshIndex];
                        if(mesh == null)
                        {
                            continue;
                        }

                        uint polyCount = GetMeshPolyCount(mesh);
                        if (polyCount > highestPolyCount)
                        {
                            highestPolyCount = polyCount;
                        }
                    }

                    ulong maxActivePolyCount = (uint)particleCount * highestPolyCount;
                    particleTotalActiveMeshPolyCount += maxActivePolyCount;
                }

                if(particleSystem.trails.enabled)
                {
                    particleTrailsEnabled = true;
                    materialSlots++;
                }

                if(particleSystem.collision.enabled)
                {
                    particleCollisionEnabled = true;
                }
            }

            perfStats.ParticleSystemCount = particleSystemCount;
            perfStats.ParticleTotalCount = particleTotalCount > int.MaxValue ? int.MaxValue : (int)particleTotalCount;
            perfStats.ParticleTotalActiveMeshPolyCount = particleTotalActiveMeshPolyCount > int.MaxValue ? int.MaxValue : (int)particleTotalActiveMeshPolyCount;
            perfStats.ParticleTrailsEnabled = particleTrailsEnabled;
            perfStats.ParticleCollisionEnabled = particleCollisionEnabled;
            perfStats.MaterialCount += materialSlots;
        }

        private static void AnalyzeRenderers(List<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            AnalyzeSkinnedMeshRenderers(renderers, perfStats);
            AnalyzeMeshRenderers(renderers, perfStats);
            AnalyzeTrailRenderers(renderers, perfStats);
            AnalyzeLineRenderers(renderers, perfStats);
            AnalyzeParticleSystemRenderers(renderers, perfStats);
        }

        private static void AnalyzeDynamicBone(GameObject avatarObject, AvatarPerformanceStats perfStats)
        {
            int totalSimulatedBoneCount = 0;
            int totalCollisionChecks = 0;

            if(_dynamicBoneType == null)
            {
                return;
            }

            List<Component> dynamicBones = avatarObject.GetComponentsInChildren(_dynamicBoneType, INCLUDE_INACTIVE_OBJECTS_IN_STATS).ToList();
            if (ShouldIgnoreComponent != null)
            {
                dynamicBones.RemoveAll(c => ShouldIgnoreComponent(c));
            }

            List<object> colliders = new List<object>();
            foreach(Component dynamicBone in dynamicBones)
            {
                int simulatedBones = 0;

                // Add extra bones to the end of each chain if end bones are being used.
                float endLength = (float)_dynamicBoneEndLengthFieldInfo.GetValue(dynamicBone);
                Vector3 endOffset = (Vector3)_dynamicBoneEndOffsetFieldInfo.GetValue(dynamicBone);
                bool hasEndBones = endLength > 0 || endOffset != Vector3.zero;

                Transform root = (Transform)_dynamicBoneRootFieldInfo.GetValue(dynamicBone);
                if (root != null)
                {
                    List<Transform> exclusions = (List<Transform>)_dynamicBoneExclusionsFieldInfo.GetValue(dynamicBone);
                    
                    // Calculate number of simulated bones for the hierarchy
                    simulatedBones = CountTransformsRecursively(root, exclusions, hasEndBones);
                    totalSimulatedBoneCount += simulatedBones;
                }

                int colliderListEntryCount = 0;
                IList colliderList = (IList)_dynamicBoneCollidersFieldInfo.GetValue(dynamicBone);
                if (colliderList != null)
                {
                    foreach(object collider in colliderList)
                    {
                        colliderListEntryCount += 1;
                        if(collider != null && !colliders.Contains(collider))
                        {
                            colliders.Add(collider);
                        }
                    }
                }

                // The root bone is skipped in collision checks.
                totalCollisionChecks += (simulatedBones - 1) * colliderListEntryCount;
            }

            perfStats.DynamicBoneComponentCount = dynamicBones.Count;
            perfStats.DynamicBoneSimulatedBoneCount = totalSimulatedBoneCount;
            perfStats.DynamicBoneColliderCount = colliders.Count;
            perfStats.DynamicBoneCollisionCheckCount = totalCollisionChecks;
        }

        private static void FindOptionalTypes()
        {
            FindDynamicBoneTypes();
        }

        private static void FindDynamicBoneTypes()
        {
            if (_dynamicBoneType != null)
            {
                return;
            }

            Type dyBoneType = Validation.GetTypeFromName("DynamicBone");
            if (dyBoneType == null)
            {
                return;
            }

            Type dyBoneColliderType = Validation.GetTypeFromName("DynamicBoneColliderBase") ?? Validation.GetTypeFromName("DynamicBoneCollider");
            if (dyBoneColliderType == null)
            {
                return;
            }

            FieldInfo rootFieldInfo = dyBoneType.GetField("m_Root", BindingFlags.Public | BindingFlags.Instance);
            if (rootFieldInfo == null || rootFieldInfo.FieldType != typeof(Transform))
            {
                return;
            }

            FieldInfo exclusionsFieldInfo = dyBoneType.GetField("m_Exclusions", BindingFlags.Public | BindingFlags.Instance);
            if (exclusionsFieldInfo == null || exclusionsFieldInfo.FieldType != typeof(List<Transform>))
            {
                return;
            }

            FieldInfo collidersFieldInfo = dyBoneType.GetField("m_Colliders", BindingFlags.Public | BindingFlags.Instance);
            if (collidersFieldInfo == null || collidersFieldInfo.FieldType.GetGenericTypeDefinition() != typeof(List<>) || collidersFieldInfo.FieldType.GetGenericArguments().Single() != dyBoneColliderType)
            {
                return;
            }

            FieldInfo endLengthFieldInfo = dyBoneType.GetField("m_EndLength", BindingFlags.Public | BindingFlags.Instance);
            if (endLengthFieldInfo == null || endLengthFieldInfo.FieldType != typeof(float))
            {
                return;
            }
            
            FieldInfo endOffsetFieldInfo = dyBoneType.GetField("m_EndOffset", BindingFlags.Public | BindingFlags.Instance);
            if (endOffsetFieldInfo == null || endOffsetFieldInfo.FieldType != typeof(Vector3))
            {
                return;
            }

            _dynamicBoneType = dyBoneType;
            _dynamicBoneRootFieldInfo = rootFieldInfo;
            _dynamicBoneExclusionsFieldInfo = exclusionsFieldInfo;
            _dynamicBoneCollidersFieldInfo = collidersFieldInfo;
            _dynamicBoneEndLengthFieldInfo = endLengthFieldInfo;
            _dynamicBoneEndOffsetFieldInfo = endOffsetFieldInfo;
        }

        // Like DynamicBone itself exclusions only apply to children of the current bone.
        // This means the root bone itself never excluded.
        private static int CountTransformsRecursively(Transform transform, List<Transform> exclusions, bool addEndBones)
        {
            if (transform == null)
            {
                return 0;
            }

            int count = 1;
            int childCount = transform.childCount;
            if(childCount > 0)
            {
                foreach(Transform child in transform)
                {
                    if(exclusions == null || !exclusions.Contains(child))
                    {
                        count += CountTransformsRecursively(child, exclusions, addEndBones);
                    }
                }
            }
            else
            {
                if(addEndBones)
                {
                    count++;
                }
            }

            return count;
        }

        #endregion
    }
}
