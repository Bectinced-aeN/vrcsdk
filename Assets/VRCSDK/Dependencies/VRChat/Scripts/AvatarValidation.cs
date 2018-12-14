using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace VRCSDK2
{
    public static class AvatarValidation
    {
        public static readonly string[] ComponentTypeWhiteList = new string[] {
            "UnityEngine.Transform",
            "UnityEngine.Animator",
            "VRC.Core.PipelineManager",
#if !VRC_CLIENT
            "VRC.Core.PipelineSaver",
#endif
            "VRCSDK2.VRC_AvatarDescriptor",
            "VRCSDK2.VRC_AvatarVariations",
            "NetworkMetadata",
            "RootMotion.FinalIK.IKExecutionOrder",
            "RootMotion.FinalIK.VRIK",
            "RootMotion.FinalIK.FullBodyBipedIK",
            "RootMotion.FinalIK.LimbIK",
            "RootMotion.FinalIK.AimIK",
            "RootMotion.FinalIK.BipedIK",
            "RootMotion.FinalIK.GrounderIK",
            "RootMotion.FinalIK.GrounderFBBIK",
            "RootMotion.FinalIK.GrounderVRIK",
            "RootMotion.FinalIK.GrounderQuadruped",
            "RootMotion.FinalIK.TwistRelaxer",
            "RootMotion.FinalIK.ShoulderRotator",
            "RootMotion.FinalIK.FBBIKArmBending",
            "RootMotion.FinalIK.FBBIKHeadEffector",
            "RootMotion.FinalIK.FABRIK",
            "RootMotion.FinalIK.FABRIKChain",
            "RootMotion.FinalIK.FABRIKRoot",
            "RootMotion.FinalIK.CCDIK",
            "RootMotion.FinalIK.RotationLimit",
            "RootMotion.FinalIK.RotationLimitHinge",
            "RootMotion.FinalIK.RotationLimitPolygonal",
            "RootMotion.FinalIK.RotationLimitSpline",
            "UnityEngine.SkinnedMeshRenderer",
            "LimbIK", // our limbik based on Unity ik
            "AvatarAnimation",
            "LoadingAvatarTextureAnimation",
            "UnityEngine.MeshFilter",
            "UnityEngine.MeshRenderer",
            "UnityEngine.Animation",
            "UnityEngine.ParticleSystem",
            "UnityEngine.ParticleSystemRenderer",
            "DynamicBone",
            "DynamicBoneCollider",
            "UnityEngine.TrailRenderer",
            "UnityEngine.Cloth",
            "UnityEngine.Light",
            "UnityEngine.Collider",
            "UnityEngine.Rigidbody",
            "UnityEngine.Joint",
            "UnityEngine.Camera",
            "UnityEngine.FlareLayer",
            "UnityEngine.GUILayer",
            "UnityEngine.AudioSource",
            "ONSPAudioSource",
            "AvatarCustomAudioLimiter",
            "UnityEngine.EllipsoidParticleEmitter",
            "UnityEngine.ParticleRenderer",
            "UnityEngine.ParticleAnimator",
            "UnityEngine.MeshParticleEmitter",
            "UnityEngine.LineRenderer",
            "VRCSDK2.VRC_IKFollower",
            "VRC_IKFollowerInternal",
            "RealisticEyeMovements.EyeAndHeadAnimator",
            "AvatarAudioSourceFilter"
        };

        public static bool ps_limiter_enabled = false;
        public static int ps_max_particles = 50000;
        public static int ps_max_systems = 200;
        public static int ps_max_emission = 5000;
        public static int ps_max_total_emission = 40000;
        public static int ps_mesh_particle_divider = 50;
        public static int ps_mesh_particle_poly_limit = 50000;
        public static int ps_collision_penalty_high = 120;
        public static int ps_collision_penalty_med = 60;
        public static int ps_collision_penalty_low = 10;
        public static int ps_trails_penalty = 10;
        
        public static IEnumerable<Component> FindIllegalComponents(string Name, GameObject currentAvatar)
        {
            List<Component> bad = new List<Component>();
            IEnumerator seeker = FindIllegalComponentsEnumerator(Name, currentAvatar, (c) => bad.Add(c));
            while (seeker.MoveNext()) ;
            return bad;
        }

        public static void RemoveIllegalComponents(string Name, GameObject currentAvatar, bool retry = true)
        {
            IEnumerator remover = RemoveIllegalComponentsEnumerator(Name, currentAvatar, retry);
            while (remover.MoveNext()) ;
        }

        public static IEnumerator RemoveIllegalComponentsEnumerator(string Name, GameObject currentAvatar, bool retry = true)
        {
            bool foundBad = false;
            yield return VRCSDK2.AvatarValidation.FindIllegalComponentsEnumerator(Name, currentAvatar, (c) => {
                if (c != null)
                {
                    Debug.LogWarningFormat("Removed component of type {0}", c.GetType().Name);
                    RemoveDependancies(c);

#if VRC_CLIENT
                    Object.DestroyImmediate(c, true);
#else
                    Object.DestroyImmediate(c, false);
#endif
                    foundBad = true;
                }
            });

            if (retry && foundBad)
                yield return RemoveIllegalComponentsEnumerator(Name, currentAvatar, false);
        }

        public static List<AudioSource> EnforceAudioSourceLimits(GameObject currentAvatar)
        {
            List<AudioSource> found = new List<AudioSource>();
            IEnumerator enforcer = EnforceAudioSourceLimitsEnumerator(currentAvatar, (a) => found.Add(a));
            while (enforcer.MoveNext()) ;
            return found;
        }

        public static IEnumerator EnforceAudioSourceLimitsEnumerator(GameObject currentAvatar, System.Action<AudioSource> onFound)
        {
            if (currentAvatar == null)
                yield break;

            Queue<GameObject> children = new Queue<GameObject>();
            if( currentAvatar != null )
                children.Enqueue(currentAvatar.gameObject);
            while (children.Count > 0)
            {
                GameObject child = children.Dequeue();
                if (child == null)
                    continue;

                int childCount = child.transform.childCount;
                for (int idx = 0; idx < child.transform.childCount; ++idx)
                    children.Enqueue(child.transform.GetChild(idx).gameObject);

#if VRC_CLIENT
                if (child.GetComponent<USpeaker>() != null)
                    continue;
#endif

                AudioSource[] sources = child.transform.GetComponents<AudioSource>();
                if (sources != null && sources.Length > 0)
                {
                    AudioSource au = sources[0];
                    if (au == null)
                        continue;

#if VRC_CLIENT
                    au.outputAudioMixerGroup = VRCAudioManager.GetAvatarGroup();
#endif

                    if (au.volume > 0.9f)
                        au.volume = 0.9f;

#if VRC_CLIENT
                    // someone mucked with the sdk forced settings, shame on them!
                    if (au.spatialize == false)
                        au.volume = 0;
#else
                    au.spatialize = true;
#endif

                    au.priority = Mathf.Clamp(au.priority, 200, 255);
                    au.bypassEffects = false;
                    au.bypassListenerEffects = false;
                    au.spatialBlend = 1f;
                    au.spread = 0;

                    au.minDistance = Mathf.Clamp(au.minDistance, 0, 2);
                    au.maxDistance = Mathf.Clamp(au.maxDistance, 0, 30);

                    float range = au.maxDistance - au.minDistance;
                    float min = au.minDistance;
                    float max = au.maxDistance;
                    float mult = 50.0f/range;

                    // setup a custom rolloff curve
                    Keyframe[] keys = new Keyframe[7];
                    keys[0] = new Keyframe(0, 1);
                    keys[1] = new Keyframe(min, 1, 0, -0.4f * mult);
                    keys[2] = new Keyframe(min + 0.022f * range, 0.668f, -0.2f * mult, -0.2f * mult);
                    keys[3] = new Keyframe(min + 0.078f * range, 0.359f, -0.05f * mult, -0.05f * mult);
                    keys[4] = new Keyframe(min + 0.292f * range, 0.102f, -0.01f * mult, -0.01f * mult);
                    keys[5] = new Keyframe(min + 0.625f * range, 0.025f, -0.002f * mult, -0.002f * mult);
                    keys[6] = new Keyframe(max, 0);
                    AnimationCurve curve = new AnimationCurve(keys);

                    au.rolloffMode = AudioRolloffMode.Custom;
                    au.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);

                    // if we have an onsp component, also configure that
                    ONSPAudioSource oa = au.GetComponent<ONSPAudioSource>();
                    if (oa)
                    {
                        if (oa.Gain > 10f) oa.Gain = 10f;
#if VRC_CLIENT
                        // someone mucked with the sdk forced settings, shame on them!
                        if (oa.enabled == false || oa.EnableSpatialization == false)
                        {
                            oa.Gain = 0f;
                            au.volume = 0f;
                        }
#else
                        oa.enabled = true;
                        oa.EnableSpatialization = true;
#endif
                        oa.UseInvSqr = true; // This is the ENABLED value for OCULUS ATTENUATION
                        oa.EnableRfl = false;
                        if (oa.Near > 2f) oa.Near = 2f;
                        if (oa.Far > 30f) oa.Far = 30f;
                        oa.VolumetricRadius = 0f;
                    }

                    onFound(au);

                    if (sources.Length > 1)
                    {
                        Debug.LogError("Disabling extra AudioSources on GameObject("+ child.name +"). Only one is allowed per GameObject.");
                        for (int i=1; i<sources.Length; i++)
                        {
                            if (sources[i] == null)
                                continue;

#if VRC_CLIENT
                            sources[i].enabled = false;
                            sources[i].clip = null;
#else
                            Object.DestroyImmediate(sources[i]);
#endif
                        }
                    }
                }

                yield return null;
            }
        }

        public static void EnforceRealtimeParticleSystemLimits(Dictionary<ParticleSystem, int> particleSystems, bool includeDisabled = false, bool stopSystems = true)
        {
            if (!ps_limiter_enabled)
                return;

            float totalEmission = 0;
            ParticleSystem ps = null;
            int max = 0;
            int em_penalty = 1;
            ParticleSystem.EmissionModule em;
            float emission = 0;
            ParticleSystem.Burst[] bursts;

            foreach (KeyValuePair<ParticleSystem, int> kp in particleSystems)
            {
                if (!kp.Key.isPlaying && !includeDisabled)
                    continue;
                ps = kp.Key;
                max = kp.Value;
                em_penalty = 1;
                if (ps.collision.enabled)
                {
                    switch (ps.collision.quality)
                    {
                        case ParticleSystemCollisionQuality.High:
                            max = max / ps_collision_penalty_high;
                            em_penalty += 3;
                            break;
                        case ParticleSystemCollisionQuality.Medium:
                            max = max / ps_collision_penalty_med;
                            em_penalty += 2;
                            break;
                        case ParticleSystemCollisionQuality.Low:
                            max = max / ps_collision_penalty_low;
                            em_penalty += 2;
                            break;
                    }
                }
                if (ps.trails.enabled)
                {
                    max = max / ps_trails_penalty;
                    em_penalty += 3;
                }
                if (ps.emission.enabled)
                {
                    em = ps.emission;
                    emission = 0;
                    emission += GetCurveMax(em.rateOverTime);
                    emission += GetCurveMax(em.rateOverDistance);

                    bursts = new ParticleSystem.Burst[em.burstCount];
                    em.GetBursts(bursts);
                    for (int i = 0; i < bursts.Length; i++)
                    {
                        float adjMax = bursts[i].repeatInterval > 1 ? bursts[i].maxCount : bursts[i].maxCount * bursts[i].repeatInterval;
                        if (adjMax > ps_max_emission)
                            bursts[i].maxCount = (short)Mathf.Clamp(adjMax, 0, ps_max_emission);
                    }
                    em.SetBursts(bursts);

                    emission *= em_penalty;
                    totalEmission += emission;
                    if ((emission > ps_max_emission || totalEmission > ps_max_total_emission) && stopSystems)
                    {
                        kp.Key.Stop();
                        // Debug.LogWarning("Particle system named " + kp.Key.gameObject.name + " breached particle emission limits, it has been stopped");
                    }
                }
                if (ps.main.maxParticles > Mathf.Clamp(max, 1, kp.Value))
                {
                    ParticleSystem.MainModule psm = ps.main;
                    psm.maxParticles = Mathf.Clamp(psm.maxParticles, 1, max);
                    if (stopSystems)
                        kp.Key.Stop();
                    Debug.LogWarning("Particle system named " + kp.Key.gameObject.name + " breached particle limits, it has been limited");
                }
            }
        }

        public static void RemoveCameras(GameObject currentAvatar, bool localPlayer, bool friend)
        {
            if (!localPlayer && currentAvatar != null)
            {
                foreach (Camera camera in currentAvatar.GetComponentsInChildren<Camera>(true))
                {
                    if (camera == null || camera.gameObject == null)
                        continue;

                    Debug.LogWarning("Removing camera from " + camera.gameObject.name);

                    if (friend && camera.targetTexture != null)
                    {
                        camera.enabled = false;
                    }
                    else
                    {
                        RemoveDependancies(camera);

                        camera.enabled = false;
                        if (camera.targetTexture != null)
                            camera.targetTexture = new RenderTexture(16, 16, 24);
                        MonoBehaviour.DestroyImmediate(camera);
                    }
                }
            }
        }

        public static void RemoveExtraAnimationComponents(GameObject currentAvatar)
        {
            if (currentAvatar == null)
                return;

            // remove Animator comps
            {
                Animator mainAnimator = currentAvatar.GetComponent<Animator>();
                bool removeMainAnimator = false;
                if (mainAnimator != null)
                {
                    if (!mainAnimator.isHuman || mainAnimator.avatar == null || !mainAnimator.avatar.isValid)
                    {
                        removeMainAnimator = true;
                    }
                }

                foreach (Animator anim in currentAvatar.GetComponentsInChildren<Animator>(true))
                {
                    if (anim == null || anim.gameObject == null)
                        continue;

                    // exclude the main avatar animator
                    if (anim == mainAnimator)
                    {
                        if (!removeMainAnimator)
                        {
                            continue;
                        }
                    }

                    Debug.LogWarning("Removing Animator comp from " + anim.gameObject.name);

                    RemoveDependancies(anim);

                    anim.enabled = false;
                    MonoBehaviour.DestroyImmediate(anim);
                } 
            }

            // remove legacy Animation comps
            {
                foreach (Animation anim in currentAvatar.GetComponentsInChildren<Animation>(true))
                {
                    if (anim == null || anim.gameObject == null)
                        continue;

                    Debug.LogWarning("Removing Animation comp from " + anim.gameObject.name);

                    RemoveDependancies(anim);

                    anim.enabled = false;
                    MonoBehaviour.DestroyImmediate(anim);
                }
            }
        }

        public static void RemoveLightComponents(GameObject currentAvatar)
        {
            if (currentAvatar == null)
                return;

            foreach (Light light in currentAvatar.GetComponentsInChildren<Light>(true))
            {
                if (light == null || light.gameObject == null)
                    continue;

                Debug.LogWarning("Removing Light comp from " + light.gameObject.name);

                RemoveDependancies(light);

                light.enabled = false;
                MonoBehaviour.DestroyImmediate(light);
            }
        }

        public static void RemoveLineRendererComponents(GameObject currentAvatar)
        {
            if (currentAvatar == null)
                return;

            foreach (var comp in currentAvatar.GetComponentsInChildren<LineRenderer>(true))
            {
                if (comp == null || comp.gameObject == null)
                    continue;

                Debug.LogWarning("Removing LineRenderer comp from " + comp.gameObject.name);

                RemoveDependancies(comp);

                comp.enabled = false;
                MonoBehaviour.DestroyImmediate(comp);
            }
        }

        public static void RemoveTrailRendererComponents(GameObject currentAvatar) 
        {
            if (currentAvatar == null)
                return;

            foreach (var comp in currentAvatar.GetComponentsInChildren<TrailRenderer>(true))
            {
                if (comp == null || comp.gameObject == null)
                    continue;

                Debug.LogWarning("Removing TrailRenderer comp from " + comp.gameObject.name);

                RemoveDependancies(comp);

                comp.enabled = false;
                MonoBehaviour.DestroyImmediate(comp);
            }
        }

        static Color32 GetTrustLevelColor(VRC.Core.APIUser user)
        {
#if VRC_CLIENT
            Color32 color = new Color32(255,255,255,255);
            if (user==null) return color;

            if (user == VRC.Core.APIUser.CurrentUser)
            {
                color = VRCInputManager.showSocialRank ? VRCPlayer.GetColorForSocialRank(user) : VRCPlayer.GetDefaultNameplateColor(user, user.hasVIPAccess);
            }
            else
            {
                color = VRCPlayer.ShouldShowSocialRank(user) ? VRCPlayer.GetColorForSocialRank(user) : VRCPlayer.GetDefaultNameplateColor(user, user.hasVIPAccess);
            }
            return color;
#else
            // we are in sdk, this is not meaningful anyway
            return (Color32)Color.grey;
#endif
		}

        static Material CreateFallbackMaterial(Material mtl, VRC.Core.APIUser user)
        {
#if VRC_CLIENT
            Material newMtl;
            Color trustCol = (Color)GetTrustLevelColor(user);

            if (mtl != null)
            {
                var safeShader = VRC.Core.AssetManagement.GetSafeShader(mtl.shader.name);
                if (safeShader == null)
                {
                    newMtl = VRC.Core.AssetManagement.CreateSafeFallbackMaterial(mtl, trustCol * 0.8f + new Color(0.2f, 0.2f, 0.2f));
                    newMtl.name = "FB_"+mtl.shader.name;
                }
                else
                {
                    //Debug.Log("<color=cyan>*** using safe internal fallback for shader:"+ safeShader.name + "</color>");
                    newMtl = new Material(safeShader);
                    if (safeShader.name=="Standard" || safeShader.name=="Standard (Specular setup)")
                        VRC.Core.AssetManagement.SetupBlendMode(newMtl);
                    newMtl.CopyPropertiesFromMaterial(mtl);
                    newMtl.name = "INT_"+mtl.shader.name;
                }
            }
            else
            {
                newMtl = VRC.Core.AssetManagement.CreateMatCap(trustCol * 0.8f + new Color(0.2f, 0.2f, 0.2f));
                newMtl.name = "FB?_";
            }

            return newMtl;
#else
            // we are in sdk, this is not meaningful anyway
            return new Material(Shader.Find("Standard"));
#endif
        }

        public static void SetupShaderReplace(VRC.Core.APIUser user, GameObject currentAvatar, out HashSet<Renderer> avatarRenderers, out Dictionary<Material, Material> materialSwaps)
        {
            materialSwaps = new Dictionary<Material, Material>();
            avatarRenderers = new HashSet<Renderer>(currentAvatar.GetComponentsInChildren<SkinnedMeshRenderer>(true));
            avatarRenderers.UnionWith(currentAvatar.GetComponentsInChildren<MeshRenderer>(true));

            // find any material with custom shader and add to set
            // we don't actually replace the running shaders here
            foreach (Renderer r in avatarRenderers)
            {
                for (int i = 0; i < r.sharedMaterials.Length; ++i)
                {
                    if (r.sharedMaterials[i] == null || r.sharedMaterials[i].shader == null)
                        continue;
                    Material newMtl = CreateFallbackMaterial(r.sharedMaterials[i], user);
                    materialSwaps[newMtl] = r.sharedMaterials[i];
                }
            }
        }

        public static void ReplaceShaders(VRC.Core.APIUser user, HashSet<Renderer> avatarRenderers, ref Dictionary<Material, Material> materialSwaps, bool debug=true)
        {
            foreach (Renderer r in avatarRenderers)
            {
                Material[] materials = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < r.sharedMaterials.Length; ++i)
                {
                    if (r.sharedMaterials[i] == null)
                        materials[i] = CreateFallbackMaterial(null, user);
                    else if (materialSwaps.ContainsKey(r.sharedMaterials[i]))
                    {
                        // material is in our swap list, so its already a fallback
                        materials[i] = r.sharedMaterials[i];
                        if (debug)
                            Debug.Log("<color=cyan>*** using fallback :'" + materials[i].shader.name + "' </color>");
                    }
                    else
                    {
                        // material is not in our safe list, create a fallback and store it in dictionary
                        Material newMtl = CreateFallbackMaterial(r.sharedMaterials[i], user);
                        materialSwaps[newMtl] = materials[i];
                        materials[i] = newMtl;
                        if (debug)
                            Debug.Log("<color=cyan>*** new fallback :'" + materials[i].shader.name + "' </color>");
                    }
                }
                r.sharedMaterials = materials;
            }
        }

        public static void ReplaceShadersRealtime(VRC.Core.APIUser user, HashSet<Renderer> avatarRenderers, ref Dictionary<Material, Material> materialSwaps)
        {
            ReplaceShaders(user, avatarRenderers, ref materialSwaps, false);
        }

        public static void RestoreShaders(VRC.Core.APIUser user, HashSet<Renderer> avatarRenderers, Dictionary<Material, Material> materialSwaps)
        {
            foreach (Renderer r in avatarRenderers)
            {
                Material[] materials = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < r.sharedMaterials.Length; ++i)
                {
                    if (r.sharedMaterials[i] == null)
                    {
                        // create a temporary shader while loading
                        materials[i] = CreateFallbackMaterial(null, user);
                    }
                    else if (materialSwaps.ContainsKey(r.sharedMaterials[i]))
                        materials[i] = materialSwaps[r.sharedMaterials[i]];
                    else
                        materials[i] = r.sharedMaterials[i];
                }
                r.sharedMaterials = materials;
            }
        }

        public static void SetupParticleLimits()
        {
            ps_limiter_enabled = VRC.Core.RemoteConfig.GetBool("ps_limiter_enabled", ps_limiter_enabled);
            ps_max_particles = VRC.Core.RemoteConfig.GetInt("ps_max_particles", ps_max_particles);
            ps_max_systems = VRC.Core.RemoteConfig.GetInt("ps_max_systems", ps_max_systems);
            ps_max_emission = VRC.Core.RemoteConfig.GetInt("ps_max_emission", ps_max_emission);
            ps_max_total_emission = VRC.Core.RemoteConfig.GetInt("ps_max_total_emission", ps_max_total_emission);
            ps_mesh_particle_divider = VRC.Core.RemoteConfig.GetInt("ps_mesh_particle_divider", ps_mesh_particle_divider);
            ps_mesh_particle_poly_limit = VRC.Core.RemoteConfig.GetInt("ps_mesh_particle_poly_limit", ps_mesh_particle_poly_limit);
            ps_collision_penalty_high = VRC.Core.RemoteConfig.GetInt("ps_collision_penalty_high", ps_collision_penalty_high);
            ps_collision_penalty_med = VRC.Core.RemoteConfig.GetInt("ps_collision_penalty_med", ps_collision_penalty_med);
            ps_collision_penalty_low = VRC.Core.RemoteConfig.GetInt("ps_collision_penalty_low", ps_collision_penalty_low);
            ps_trails_penalty = VRC.Core.RemoteConfig.GetInt("ps_trails_penalty", ps_trails_penalty);

            ps_limiter_enabled = VRC.Core.LocalConfig.GetList("betas").Contains("particle_system_limiter") || ps_limiter_enabled;
            ps_max_particles = VRC.Core.LocalConfig.GetInt("ps_max_particles", ps_max_particles);
            ps_max_systems = VRC.Core.LocalConfig.GetInt("ps_max_systems", ps_max_systems);
            ps_max_emission = VRC.Core.LocalConfig.GetInt("ps_max_emission", ps_max_emission);
            ps_max_total_emission = VRC.Core.LocalConfig.GetInt("ps_max_total_emission", ps_max_total_emission);
            ps_mesh_particle_divider = VRC.Core.LocalConfig.GetInt("ps_mesh_particle_divider", ps_mesh_particle_divider);
            ps_mesh_particle_poly_limit = VRC.Core.LocalConfig.GetInt("ps_mesh_particle_poly_limit", ps_mesh_particle_poly_limit);
            ps_collision_penalty_high = VRC.Core.LocalConfig.GetInt("ps_collision_penalty_high", ps_collision_penalty_high);
            ps_collision_penalty_med = VRC.Core.LocalConfig.GetInt("ps_collision_penalty_med", ps_collision_penalty_med);
            ps_collision_penalty_low = VRC.Core.LocalConfig.GetInt("ps_collision_penalty_low", ps_collision_penalty_low);
            ps_trails_penalty = VRC.Core.LocalConfig.GetInt("ps_trails_penalty", ps_trails_penalty);
        }

        public static Dictionary<ParticleSystem, int> EnforceParticleSystemLimits(GameObject currentAvatar)
        {
            Dictionary<ParticleSystem, int> particleSystems = new Dictionary<ParticleSystem, int>();
            
            foreach(ParticleSystem ps in currentAvatar.transform.GetComponentsInChildren<ParticleSystem>(true))
            {
                int realtime_max = ps_max_particles;

                if (ps_limiter_enabled)
                {
                    if (particleSystems.Count > ps_max_systems)
                    {
                        Debug.LogError("Too many particle systems, #" + particleSystems.Count + " named " + ps.gameObject.name + " deleted");
                        Object.DestroyImmediate(ps);
                    }
                    else
                    {
                        var main = ps.main;
                        var collision = ps.collision;
                        var emission = ps.emission;


                        if (ps.GetComponent<ParticleSystemRenderer>())
                        {
                            ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
                            if (renderer.renderMode == ParticleSystemRenderMode.Mesh)
                            {
                                Mesh[] meshes = new Mesh[0];
                                int heighestPoly = 0;
                                renderer.GetMeshes(meshes);
                                if (meshes.Length == 0 && renderer.mesh != null)
                                {
                                    meshes = new Mesh[] { renderer.mesh };
                                }
                                // Debug.Log(meshes.Length + " meshes possible emmited meshes from " + ps.gameObject.name);
                                foreach (Mesh m in meshes)
                                {
                                    if (m.isReadable)
                                    {
                                        if (m.triangles.Length / 3 > heighestPoly)
                                        {
                                            heighestPoly = m.triangles.Length / 3;
                                        }
                                    }
                                    else
                                    {
                                        if (1000 > heighestPoly)
                                        {
                                            heighestPoly = 1000;
                                        }
                                    }
                                }
                                if (heighestPoly > 0)
                                {
                                    heighestPoly = Mathf.Clamp(heighestPoly / ps_mesh_particle_divider, 1, heighestPoly);
                                    if (heighestPoly < realtime_max)
                                    {
                                        realtime_max = realtime_max / heighestPoly;
                                    }
                                    else
                                    {
                                        realtime_max = 1;
                                    }
                                    if (heighestPoly > ps_mesh_particle_poly_limit)
                                    {
                                        Debug.LogError("Particle system named " + ps.gameObject.name + " breached polygon limits, it has been deleted");
                                        Object.DestroyImmediate(ps);
                                        continue;
                                    }
                                }
                            }
                        }


                        ParticleSystem.MinMaxCurve rate = emission.rateOverTime;

                        if (rate.mode == ParticleSystemCurveMode.Constant)
                        {
                            rate.constant = Mathf.Clamp(rate.constant, 0, ps_max_emission);
                        }
                        else if (rate.mode == ParticleSystemCurveMode.TwoConstants)
                        {
                            rate.constantMax = Mathf.Clamp(rate.constantMax, 0, ps_max_emission);
                        }
                        else
                        {
                            rate.curveMultiplier = Mathf.Clamp(rate.curveMultiplier, 0, ps_max_emission);
                        }

                        emission.rateOverTime = rate;
                        rate = emission.rateOverDistance;

                        if (rate.mode == ParticleSystemCurveMode.Constant)
                        {
                            rate.constant = Mathf.Clamp(rate.constant, 0, ps_max_emission);
                        }
                        else if (rate.mode == ParticleSystemCurveMode.TwoConstants)
                        {
                            rate.constantMax = Mathf.Clamp(rate.constantMax, 0, ps_max_emission);
                        }
                        else
                        {
                            rate.curveMultiplier = Mathf.Clamp(rate.curveMultiplier, 0, ps_max_emission);
                        }

                        emission.rateOverDistance = rate;

                        //Disable collision with PlayerLocal layer
                        collision.collidesWith &= ~(1 << 10);
                    } 
                }

                particleSystems.Add(ps, realtime_max);
            }

            EnforceRealtimeParticleSystemLimits(particleSystems, true, false);

            return particleSystems;
        }

        public static bool ClearLegacyAnimations(GameObject currentAvatar)
        {
            bool hasLegacyAnims = false;
			foreach(var ani in currentAvatar.GetComponentsInChildren<Animation>(true))
            {
                if(ani.clip != null)
                    if(ani.clip.legacy)
                    {
                        Debug.LogWarningFormat("Legacy animation found named '{0}' on '{1}', removing", ani.clip.name, ani.gameObject.name);
                        ani.clip = null;
                        hasLegacyAnims = true;
                    }
                foreach(AnimationState anistate in ani)
                    if(anistate.clip.legacy)
                    {
                        Debug.LogWarningFormat("Legacy animation found named '{0}' on '{1}', removing", anistate.clip.name, ani.gameObject.name);
                        ani.RemoveClip(anistate.clip);
                        hasLegacyAnims = true;
                    }
            }
            return hasLegacyAnims;
        }

        private static IEnumerator FindIllegalComponentsEnumerator(string Name, GameObject currentAvatar, System.Action<Component> onFound)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            System.Type[] foundTypes = WhiteListedTypes;
            HashSet<System.Type> typesInUse = new HashSet<System.Type>();
            List<Component> componentsInUse = new List<Component>();
            Queue<GameObject> children = new Queue<GameObject>();
            children.Enqueue(currentAvatar.gameObject);
            while (children.Count > 0)
            {
                GameObject child = children.Dequeue();
                int childCount = child.transform.childCount;
                for (int idx = 0; idx < child.transform.childCount; ++idx)
                    children.Enqueue(child.transform.GetChild(idx).gameObject);
                foreach (Component c in child.transform.GetComponents<Component>())
                {
                    if (c == null)
                        continue;

                    if (typesInUse.Contains(c.GetType()) == false)
                        typesInUse.Add(c.GetType());

                    if (!foundTypes.Any(allowedType => c.GetType() == allowedType || c.GetType().IsSubclassOf(allowedType)))
                    {
                        onFound(c);
                        yield return null;
                    }

                    if (watch.ElapsedMilliseconds > 1)
                    {
                        yield return null;
                        watch.Reset();
                    }
                }
            }
        }

        private static System.Type[] _foundTypes = null;
        private static System.Type[] WhiteListedTypes
        {
            get
            {
                if (_foundTypes != null)
                    return _foundTypes;

                Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

                _foundTypes = ComponentTypeWhiteList.Select((name) =>
                {
                    foreach (Assembly a in assemblies)
                    {
                        System.Type found = a.GetType(name);
                        if (found != null)
                            return found;
                    }

                //This is really verbose for some SDK scenes, eg.
                //If they don't have FinalIK installed
#if VRC_CLIENT && UNITY_EDITOR
                Debug.LogError("Could not find type " + name);
#endif
                return null;
                }).Where(t => t != null).ToArray();

                return _foundTypes;
            }
        }

        private static void RemoveDependancies(Component component)
        {
            if (component == null)
                return;

            Component[] components = component.GetComponents<Component>();
            if (components == null || components.Length == 0)
                return;

            System.Type compType = component.GetType();
            foreach (var c in components)
            {
                if (c == null)
                    continue;

                bool deleteMe = false;
                object[] requires = c.GetType().GetCustomAttributes(typeof(RequireComponent), true);
                if (requires == null || requires.Length == 0)
                    continue;

                foreach (var r in requires)
                {
                    RequireComponent rc = r as RequireComponent;
                    if (rc == null)
                        continue;

                    if (rc.m_Type0 == compType ||
                        rc.m_Type1 == compType ||
                        rc.m_Type2 == compType)
                    {
                        deleteMe = true;
                    }
                }

                if (deleteMe)
                {
                    Debug.LogErrorFormat("Deleting component dependency {0} found on {1}", c.GetType().Name, component.gameObject.name);

                    RemoveDependancies(c);

#if VRC_CLIENT
                    Object.DestroyImmediate(c, true);
#else
                        Object.DestroyImmediate(c,false);
#endif
                }
            }
        }

        private static float GetCurveMax(ParticleSystem.MinMaxCurve minMaxCurve)
        {
            switch(minMaxCurve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return minMaxCurve.constant;
                case ParticleSystemCurveMode.TwoConstants:
                    return minMaxCurve.constantMax;
                default:
                    return minMaxCurve.curveMultiplier;
            }
        }

        public static bool AreAnyParticleSystemsPlaying(Dictionary<ParticleSystem, int> particleSystems)
        {
            foreach (KeyValuePair<ParticleSystem, int> kp in particleSystems)
            {
                if (kp.Key.isPlaying)
                    return true;
            }

            return false;
        }

        public static void StopAllParticleSystems(Dictionary<ParticleSystem, int> particleSystems)
        {
            foreach (KeyValuePair<ParticleSystem, int> kp in particleSystems)
            {
                if (kp.Key.isPlaying)
                {
                    kp.Key.Stop();
                }
            }
        }
    }
}
