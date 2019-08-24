using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName =  "New DynamicBonePerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/DynamicBonePerformanceScanner"
    )]
    #endif
    public class DynamicBonePerformanceScanner : AbstractPerformanceScanner
    {
        private static readonly object initLock = new object();

        private static Type _dynamicBoneType = null;
        private static FieldInfo _dynamicBoneRootFieldInfo = null;
        private static FieldInfo _dynamicBoneExclusionsFieldInfo = null;
        private static FieldInfo _dynamicBoneCollidersFieldInfo = null;
        private static FieldInfo _dynamicBoneEndLengthFieldInfo = null;
        private static FieldInfo _dynamicBoneEndOffsetFieldInfo = null;
        private static bool _searchedOptionalTypes = false;
        
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            lock (initLock)
            {
                if (!_searchedOptionalTypes)
                {
                    FindDynamicBoneTypes();
                    _searchedOptionalTypes = true;
                }
            }

            int totalSimulatedBoneCount = 0;
            int totalCollisionChecks = 0;

            if(_dynamicBoneType == null)
            {
                yield break;
            }

            List<Component> dynamicBones = avatarObject.GetComponentsInChildren(_dynamicBoneType, includeInactiveObjectsInStats).ToList();
            if (shouldIgnoreComponent != null)
            {
                dynamicBones.RemoveAll(c => shouldIgnoreComponent(c));
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

            perfStats.dynamicBoneComponentCount = dynamicBones.Count;
            perfStats.dynamicBoneSimulatedBoneCount = totalSimulatedBoneCount;
            perfStats.dynamicBoneColliderCount = colliders.Count;
            perfStats.dynamicBoneCollisionCheckCount = totalCollisionChecks;
        }

        private static void FindDynamicBoneTypes()
        {
            if (_dynamicBoneType != null)
            {
                return;
            }

            Type dyBoneType = ValidationUtils.GetTypeFromName("DynamicBone");
            if (dyBoneType == null)
            {
                return;
            }

            Type dyBoneColliderType = ValidationUtils.GetTypeFromName("DynamicBoneColliderBase") ?? ValidationUtils.GetTypeFromName("DynamicBoneCollider");
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
    }
}
