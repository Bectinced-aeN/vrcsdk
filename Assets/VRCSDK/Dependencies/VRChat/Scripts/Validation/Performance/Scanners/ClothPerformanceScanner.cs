using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName =  "New ClothPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/ClothPerformanceScanner"
    )]
    #endif
    public class ClothPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            int totalClothVertices = 0;

            List<Cloth> clothBuffer = new List<Cloth>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, clothBuffer);
            if(shouldIgnoreComponent != null)
            {
                clothBuffer.RemoveAll(c => shouldIgnoreComponent(c));
            }

            perfStats.clothCount = clothBuffer.Count;

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

            perfStats.clothMaxVertices = totalClothVertices;

            yield break;
        }
    }
}
