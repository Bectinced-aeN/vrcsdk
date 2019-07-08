using UnityEngine;

namespace VRCSDK2
{
    public class AvatarPerformanceStatsLevel : ScriptableObject
    {
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
        public int DynamicBoneCollisionCheckCount;
        public int ClothCount;
        public int ClothMaxVertices;
        public int PhysicsColliderCount;
        public int PhysicsRigidbodyCount;
        public int AudioSourceCount;
    } 
}
