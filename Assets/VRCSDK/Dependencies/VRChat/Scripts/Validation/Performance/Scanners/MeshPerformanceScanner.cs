using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName = "New MeshPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/MeshPerformanceScanner"
    )]
    #endif
    public class MeshPerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            List<Renderer> renderers = new List<Renderer>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, renderers);
            if(shouldIgnoreComponent != null)
            {
                renderers.RemoveAll(c => shouldIgnoreComponent(c));
            }

            AnalyzeGeometry(avatarObject, renderers, perfStats);
            AnalyzeMeshRenderers(renderers, perfStats);
            AnalyzeSkinnedMeshRenderers(renderers, perfStats);
            yield return null;
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
                    if(meshFilter != null)
                    {
                        sharedMesh = meshFilter.sharedMesh;
                    }
                }
            }

            if(sharedMesh == null)
            {
                return 0;
            }

            return MeshUtils.GetMeshTriangleCount(sharedMesh);
        }

        private static bool RendererHasMesh(Renderer renderer)
        {
            MeshRenderer meshRenderer = renderer as MeshRenderer;
            if(meshRenderer != null)
            {
                MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
                if(meshFilter == null)
                {
                    return false;
                }

                return meshFilter.sharedMesh != null;
            }
            
            SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
            if(skinnedMeshRenderer != null)
            {
                return skinnedMeshRenderer.sharedMesh != null;
            }

            return false;
        }

        private void AnalyzeGeometry(GameObject avatarObject, IEnumerable<Renderer> renderers, AvatarPerformanceStats perfStats)
        {
            ulong polyCount = 0;
            Bounds bounds = new Bounds(avatarObject.transform.position, Vector3.zero);
            List<Renderer> rendererIgnoreBuffer = new List<Renderer>(16);

            List<LODGroup> lodBuffer = new List<LODGroup>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, lodBuffer);
            try
            {
                foreach(LODGroup lodGroup in lodBuffer)
                {
                    LOD[] lodLevels = lodGroup.GetLODs();

                    ulong highestLodPolyCount = 0;
                    foreach(LOD lod in lodLevels)
                    {
                        uint thisLodPolyCount = 0;
                        foreach(Renderer renderer in lod.renderers)
                        {
                            rendererIgnoreBuffer.Add(renderer);
                            checked
                            {
                                thisLodPolyCount += CalculateRendererPolyCount(renderer);
                            }
                        }

                        if(thisLodPolyCount > highestLodPolyCount)
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

            foreach(Renderer renderer in renderers)
            {
                if(renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
                {
                    if(!RendererHasMesh(renderer))
                    {
                        continue;
                    }

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

            perfStats.polyCount = polyCount > int.MaxValue ? int.MaxValue : (int)polyCount;
            perfStats.aabb = bounds;
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
                foreach(Transform bone in bones)
                {
                    if(bone == null || transformIgnoreBuffer.Contains(bone))
                    {
                        continue;
                    }

                    transformIgnoreBuffer.Add(bone);
                    skinnedBoneCount++;
                }
            }

            transformIgnoreBuffer.Clear();

            perfStats.skinnedMeshCount += count;
            perfStats.materialCount += materialSlots;
            perfStats.boneCount += skinnedBoneCount;
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
                if(meshFilter == null)
                {
                    continue;
                }

                Mesh sharedMesh = meshFilter.sharedMesh;
                if(sharedMesh != null)
                {
                    materialSlots += sharedMesh.subMeshCount;
                }
            }

            perfStats.meshCount += count;
            perfStats.materialCount += materialSlots;
        }
    }
}
