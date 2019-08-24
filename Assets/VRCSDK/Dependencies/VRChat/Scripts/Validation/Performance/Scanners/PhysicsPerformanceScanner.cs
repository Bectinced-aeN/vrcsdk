using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName = "New PhysicsPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/PhysicsPerformanceScanner"
    )]
    #endif
    public class PhysicsPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            List<Collider> colliderBuffer = new List<Collider>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, colliderBuffer);
            colliderBuffer.RemoveAll(
                o =>
                {
                    if(shouldIgnoreComponent != null && shouldIgnoreComponent(o))
                    {
                        return true;
                    }

                    if(o.GetComponent<VRC_Station>() != null)
                    {
                        return true;
                    }

                    return false;
                }
            );

            perfStats.physicsColliderCount = colliderBuffer.Count;

            yield return null;

            List<Rigidbody> rigidbodyBuffer = new List<Rigidbody>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, rigidbodyBuffer);
            rigidbodyBuffer.RemoveAll(
                o =>
                {
                    if(shouldIgnoreComponent != null && shouldIgnoreComponent(o))
                    {
                        return true;
                    }

                    if(o.GetComponent<VRC_Station>() != null)
                    {
                        return true;
                    }

                    return false;
                }
            );

            perfStats.physicsRigidbodyCount = rigidbodyBuffer.Count;

            yield return null;
        }
    }
}
