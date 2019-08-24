using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRCSDK2.Validation.Performance.Stats;

namespace VRCSDK2.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName = "New ParticlePerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/ParticlePerformanceScanner"
    )]
    #endif
    public class ParticlePerformanceScanner : AbstractPerformanceScanner
    {
        [SerializeField]
        private bool includeInactiveObjectsInStats = true;

        public override IEnumerator RunPerformanceScan(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            List<ParticleSystem> particleSystems = new List<ParticleSystem>(16);
            avatarObject.GetComponentsInChildren(includeInactiveObjectsInStats, particleSystems);
            if(shouldIgnoreComponent != null)
            {
                particleSystems.RemoveAll(c => shouldIgnoreComponent(c));
            }

            yield return AnalyzeParticleSystemRenderers(particleSystems, perfStats);
        }

        private static IEnumerator AnalyzeParticleSystemRenderers(IEnumerable<ParticleSystem> particleSystems, AvatarPerformanceStats perfStats)
        {
            int particleSystemCount = 0;
            ulong particleTotalCount = 0;
            ulong particleTotalMaxMeshPolyCount = 0;
            bool particleTrailsEnabled = false;
            bool particleCollisionEnabled = false;
            int materialSlots = 0;

            foreach(ParticleSystem particleSystem in particleSystems)
            {
                int particleCount = particleSystem.main.maxParticles;
                if(particleCount <= 0)
                {
                    continue;
                }

                particleSystemCount++;
                particleTotalCount += (uint)particleCount;

                ParticleSystemRenderer particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
                if(particleSystemRenderer == null)
                {
                    continue;
                }

                materialSlots++;

                // mesh particles
                if(particleSystemRenderer.renderMode == ParticleSystemRenderMode.Mesh && particleSystemRenderer.meshCount > 0)
                {
                    uint highestPolyCount = 0;

                    Mesh[] meshes = new Mesh[particleSystemRenderer.meshCount];
                    int particleRendererMeshCount = particleSystemRenderer.GetMeshes(meshes);
                    for(int meshIndex = 0; meshIndex < particleRendererMeshCount; meshIndex++)
                    {
                        Mesh mesh = meshes[meshIndex];
                        if(mesh == null)
                        {
                            continue;
                        }

                        uint polyCount = MeshUtils.GetMeshTriangleCount(mesh);
                        if(polyCount > highestPolyCount)
                        {
                            highestPolyCount = polyCount;
                        }
                    }

                    ulong maxMeshParticlePolyCount = (uint)particleCount * highestPolyCount;
                    particleTotalMaxMeshPolyCount += maxMeshParticlePolyCount;
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

            perfStats.particleSystemCount = particleSystemCount;
            perfStats.particleTotalCount = particleTotalCount > int.MaxValue ? int.MaxValue : (int)particleTotalCount;
            perfStats.particleMaxMeshPolyCount = particleTotalMaxMeshPolyCount > int.MaxValue ? int.MaxValue : (int)particleTotalMaxMeshPolyCount;
            perfStats.particleTrailsEnabled = particleTrailsEnabled;
            perfStats.particleCollisionEnabled = particleCollisionEnabled;
            perfStats.materialCount += materialSlots;

            yield break;
        }
    }
}
