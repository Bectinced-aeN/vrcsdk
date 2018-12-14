using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRC.Core;

namespace VRCSDK2
{
    public class RuntimeWorldCreation : MonoBehaviour 
    {
        public VRC.Core.PipelineManager pipelineManager;
        
        public GameObject waitingPanel;
        public GameObject blueprintPanel;
        public GameObject errorPanel;
       
		public Text titleText;
        public InputField blueprintName;
        public InputField blueprintDescription;
        public InputField worldCapacity;
        public RawImage bpImage;
        public Image liveBpImage;
        public Toggle shouldUpdateImageToggle;
        public Toggle releasePublic;
		public Toggle contentNsfw;

        public Toggle contentSex;
        public Toggle contentViolence;
        public Toggle contentGore;
        public Toggle contentOther;

		public Toggle contentFeatured;
		public Toggle contentSDKExample;

        public UnityEngine.UI.Button uploadButton;

        public string uploadVrcPath;
        public string uploadPluginPath;
        public string uploadUnityPackagePath;

        public string cloudFrontAssetUrl;
        public string cloudFrontImageUrl;
        public string cloudFrontPluginUrl;
		public string cloudFrontUnityPackageUrl;

        private string abExtension;
        private CameraImageCapture imageCapture;
        private string assetBundleUrl;
        
        private bool isUpdate { get { return !string.IsNullOrEmpty(pipelineManager.blueprintId); } } 
        
        public bool isUploading = false;
        public float uploadProgress = 0f;
        public string uploadMessage;
        public string uploadTitle;

        private ApiWorld worldRecord;

        #if UNITY_EDITOR       
        void Start()
        {
            if(Application.isEditor && Application.isPlaying)
            {
                Application.runInBackground = true;
                UnityEngine.VR.VRSettings.enabled = false;

                PipelineManager[] pms = GameObject.FindObjectsOfType<VRC.Core.PipelineManager>();
				if(pms.Length > 0)
					pipelineManager = pms[0];

                imageCapture = GetComponent<CameraImageCapture>();
                imageCapture.shotCamera = GameObject.Find("VRCCam").GetComponent<Camera>();
                uploadButton.onClick.AddListener(SetupUpload);
               
                shouldUpdateImageToggle.onValueChanged.AddListener(ToggleUpdateImage);
                
                Login();
                SetupUI();
            }
        }
        
        void Update()
        {
            if(isUploading)
                UnityEditor.EditorUtility.DisplayProgressBar(uploadTitle, uploadMessage, uploadProgress);
        }

        void UserLoginFail(string error)
        {
            blueprintPanel.SetActive(false);
            errorPanel.SetActive(true);
        }

        void Login()
        {
            pipelineManager.user = APIUser.CachedLogin( UserLoggedInCallback, UserLoginFail, true);
        }

        void UserLoggedInCallback( APIUser user )
        {
            pipelineManager.user = user;

            if (isUpdate)
            {
                ApiWorld.Fetch(pipelineManager.blueprintId, 
                    delegate (ApiWorld world)
                    {
                        worldRecord = world;
                        SetupUI();
                    },
                    delegate (string message)
                    {
                        pipelineManager.blueprintId = "";
                        SetupUI();
                    });
            }
            else
            {
                SetupUI();
            }
        }

        void SetupUI()
        {
			if( APIUser.CurrentUser.developerType < APIUser.DeveloperType.Trusted )
			{
				contentFeatured.gameObject.SetActive(false);
				contentSDKExample.gameObject.SetActive(false);
			}
			else
			{
				contentFeatured.gameObject.SetActive(true);
				contentSDKExample.gameObject.SetActive(true);
			}

            if(APIUser.Exists(pipelineManager.user) && pipelineManager.user.isFresh)
            {
                waitingPanel.SetActive(false);
                blueprintPanel.SetActive(true);
                errorPanel.SetActive(false);

                if (isUpdate)
                {
                    // bp update
                    if (worldRecord.authorId == pipelineManager.user.id)
                    {
						titleText.text = "Update World";
                        blueprintName.text = worldRecord.name;
                        worldCapacity.text = worldRecord.capacity.ToString();
                        contentSex.isOn = worldRecord.tags.Contains("content_sex");
                        contentViolence.isOn = worldRecord.tags.Contains("content_violence");
                        contentGore.isOn = worldRecord.tags.Contains("content_gore");
                        contentOther.isOn = worldRecord.tags.Contains("content_other");

						if( APIUser.CurrentUser.developerType < APIUser.DeveloperType.Trusted )
                        {
                            releasePublic.isOn = false;
                            releasePublic.interactable = false;

							contentFeatured.isOn = contentSDKExample.isOn = false;
						}
                        else
                        {
							contentFeatured.isOn = worldRecord.tags.Contains("content_featured");
							contentSDKExample.isOn = worldRecord.tags.Contains("content_sdk_example");
                            releasePublic.isOn = worldRecord.releaseStatus == "public";
                        }
                        blueprintDescription.text = worldRecord.description;
                        shouldUpdateImageToggle.interactable = true;
                        shouldUpdateImageToggle.isOn = false;
                        liveBpImage.enabled = false;
                        bpImage.enabled = true;

                        ImageDownloader.DownloadImage(worldRecord.imageUrl, delegate(Texture2D obj) {
                            bpImage.texture = obj;
                        });
                    }
                    else // user does not own world id associated with descriptor
                    {
                        blueprintPanel.SetActive(false);
                        errorPanel.SetActive(true);
                    }
                }
                else
                {
					titleText.text = "New World";
                    shouldUpdateImageToggle.interactable = false;
                    shouldUpdateImageToggle.isOn = true;
                    liveBpImage.enabled = true;
                    bpImage.enabled = false;
                }
            }
            else
            {
                waitingPanel.SetActive(true);
                blueprintPanel.SetActive(false);
                errorPanel.SetActive(false);

				if (APIUser.CurrentUser.developerType < APIUser.DeveloperType.Trusted)
                {
                    releasePublic.isOn = false;
                    releasePublic.interactable = false;
                }
                else
                {
                    releasePublic.isOn = false;
                }
            }
        }
        
        public void SetupUpload()
        {
            uploadTitle = "Preparing For Upload";
            isUploading = true;
           
            string abPath = UnityEditor.EditorPrefs.GetString("currentBuildingAssetBundlePath");
            abExtension = System.IO.Path.GetExtension(abPath);

            string pluginPath = "";
            if(APIUser.CurrentUser.developerType >= APIUser.DeveloperType.Trusted)
                pluginPath = UnityEditor.EditorPrefs.GetString("externalPluginPath");


			string unityPackagePath = UnityEditor.EditorPrefs.GetString("VRC_exportedUnityPackagePath");

            UnityEditor.EditorPrefs.SetBool("VRCSDK2_scene_changed", true );
            UnityEditor.EditorPrefs.SetInt("VRCSDK2_capacity", System.Convert.ToInt16( worldCapacity.text ));
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_sex", contentSex.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_violence", contentViolence.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_gore", contentGore.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_other", contentOther.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_release_public", releasePublic.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_featured", contentFeatured.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_sdk_example", contentSDKExample.isOn);

            string blueprintId = isUpdate ? worldRecord.id : "new_" + VRC.Tools.GetRandomDigits(6);
            int version = isUpdate ? worldRecord.version+1 : 1;
            PrepareVRCPathForS3(abPath, blueprintId, version);

			if(!string.IsNullOrEmpty(pluginPath) && System.IO.File.Exists(pluginPath))
            {
				Debug.Log("Found plugin path. Preparing to upload!");
                PreparePluginPathForS3(pluginPath, blueprintId, version);
            }
            else
            {
				Debug.Log("Did not find plugin path. No upload occuring!");
            }

			if(!string.IsNullOrEmpty(unityPackagePath) && System.IO.File.Exists(unityPackagePath))
			{
				Debug.Log("Found unity package path. Preparing to upload!");
				PrepareUnityPackageForS3(unityPackagePath, blueprintId, version);
			}

            StartCoroutine(Upload());
        }

		void PrepareUnityPackageForS3(string packagePath, string blueprintId, int version)
		{
			uploadUnityPackagePath = Application.temporaryCachePath + "/" + blueprintId + "_" + version.ToString() + ".unitypackage";
			uploadUnityPackagePath.Trim();
			uploadUnityPackagePath.Replace(' ', '_');

			if(System.IO.File.Exists(uploadUnityPackagePath))
				System.IO.File.Delete(uploadUnityPackagePath);

			System.IO.File.Copy(packagePath, uploadUnityPackagePath);
		}

        void PrepareVRCPathForS3(string abPath, string blueprintId, int version)
        {
            uploadVrcPath = Application.temporaryCachePath + "/" + blueprintId + "_" + version.ToString() + abExtension;
            uploadVrcPath.Trim();
            uploadVrcPath.Replace(' ', '_');
            
            if(System.IO.File.Exists(uploadVrcPath))
                System.IO.File.Delete(uploadVrcPath);
            
            System.IO.File.Copy(abPath, uploadVrcPath);
        }

        void PreparePluginPathForS3(string pluginPath, string blueprintId, int version)
        {
            uploadPluginPath = Application.temporaryCachePath + "/" + blueprintId + "_" + version.ToString() + ".dll";
            uploadPluginPath.Trim();
            uploadPluginPath.Replace(' ', '_');
            
            if(System.IO.File.Exists(uploadPluginPath))
                System.IO.File.Delete(uploadPluginPath);
            
            System.IO.File.Copy(pluginPath, uploadPluginPath);
        }
        
        IEnumerator Upload()
        {
            bool caughtInvalidInput = false;
            if(!ValidateNameInput(blueprintName))
                caughtInvalidInput = true;

            if(!caughtInvalidInput)
            {
				if(!string.IsNullOrEmpty(uploadUnityPackagePath))
					yield return StartCoroutine(UploadUnityPackage());
				
                if(!string.IsNullOrEmpty(uploadPluginPath))
                    yield return StartCoroutine(UploadDLL());
                
                yield return StartCoroutine(UploadVRCFile());
                
                if(isUpdate)
                    yield return StartCoroutine(UpdateBlueprint());
                else
                    yield return StartCoroutine(CreateBlueprint());
                
                OnSDKPipelineComplete();
            }
        }

		IEnumerator UploadUnityPackage()
		{
			Debug.Log("Uploading Unity Package...");
			SetUploadProgress("Uploading Unity Package...","Future proofing your content!",  0.0f);
			bool doneUploading = false;

			string filePath = uploadUnityPackagePath;
			string s3FolderName = "unitypackages";
			var s3 = Uploader.UploadFile(filePath, s3FolderName, delegate(string obj) {
				string fileName = s3FolderName + "/" + System.IO.Path.GetFileName(filePath);
				cloudFrontUnityPackageUrl = "http://dbinj8iahsbec.cloudfront.net/" + fileName;
				doneUploading = true;
			});

            while (!doneUploading)
            {
                if (s3.webRequest != null && s3.webRequest.WwwRequest != null)
                    uploadProgress = s3.webRequest.WwwRequest.uploadProgress;
                yield return null;
            }
		}

        IEnumerator UploadDLL()
        {
            Debug.Log("Uploading Plugin...");
            SetUploadProgress("Uploading plugin...","Pushing those upload speeds!!",  0.0f);
            bool doneUploading = false;

            string filePath = uploadPluginPath;
            string s3FolderName = "plugins";
            var s3 = Uploader.UploadFile(filePath, s3FolderName, delegate(string obj) {
                string fileName = s3FolderName + "/" + System.IO.Path.GetFileName(filePath);
                cloudFrontPluginUrl = "http://dbinj8iahsbec.cloudfront.net/" + fileName;
                doneUploading = true;
            });

            while (!doneUploading)
            {
                if (s3.webRequest != null && s3.webRequest.WwwRequest != null)
                    uploadProgress = s3.webRequest.WwwRequest.uploadProgress;
                yield return null;
            }
        }

        IEnumerator UploadVRCFile()
        {
            Debug.Log("Uploading VRC File...");
            SetUploadProgress("Uploading asset...","Pushing those upload speeds!!",  0.0f);
            bool doneUploading = false;
            
            string filePath = uploadVrcPath;
            string s3FolderName = "Worlds";
            var s3 = Uploader.UploadFile(filePath, s3FolderName, delegate(string obj) {
                string fileName = s3FolderName + "/" + System.IO.Path.GetFileName(filePath);
                cloudFrontAssetUrl = "http://dbinj8iahsbec.cloudfront.net/" + fileName;
                doneUploading = true;
            });

            while (!doneUploading)
            {
                if (s3.webRequest != null && s3.webRequest.WwwRequest != null)
                    uploadProgress = s3.webRequest.WwwRequest.uploadProgress;
                yield return null;
            }
        }

        IEnumerator UploadImage()
        {
            Debug.Log("Uploading Image...");

            bool doneUploading = false;
            SetUploadProgress("Uploading Image...","That's a nice looking preview image ;)", 0.0f);
            string imagePath = imageCapture.TakePicture();
            var s3 = Uploader.UploadFile(imagePath, "images", delegate(string imageUrl)
            {
                cloudFrontImageUrl = imageUrl;
                doneUploading = true;
                VRC.Core.Logger.Log("Successfully uploaded image.", DebugLevel.All);
            });

            while (!doneUploading)
            {
                if (s3.webRequest != null && s3.webRequest.WwwRequest != null )
                    uploadProgress = s3.webRequest.WwwRequest.uploadProgress;
                yield return null;
            }
        }

        List<string> BuildTags()
        {
            var tags = new List<string>();
            if (contentSex.isOn)
                tags.Add("content_sex");
            if (contentViolence.isOn)
                tags.Add("content_violence");
            if (contentGore.isOn)
                tags.Add("content_gore");
            if (contentOther.isOn)
                tags.Add("content_other");

            if(APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
            {
                if(contentFeatured.isOn)
                    tags.Add("content_featured");
                if(contentSDKExample.isOn)
                    tags.Add("content_sdk_example");
            }

            return tags;
        }
        
        IEnumerator CreateBlueprint()
        {
            yield return StartCoroutine(UploadImage());

            SetUploadProgress("Saving Blueprint to user", "Almost finished!!", 0.0f);
            ApiWorld world = ScriptableObject.CreateInstance<ApiWorld>();
            world.Init(
                pipelineManager.user,
                blueprintName.text,
                cloudFrontImageUrl,
                cloudFrontAssetUrl,
                blueprintDescription.text,
                (releasePublic.isOn) ? ("public") : ("private"),
                System.Convert.ToInt16( worldCapacity.text ),
                BuildTags(), 
                0, 
                cloudFrontPluginUrl,
				cloudFrontUnityPackageUrl
                );

            if(APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
                world.isCurated = contentFeatured.isOn || contentSDKExample.isOn;
            else
                world.isCurated = false;

            bool doneUploading = false;
            world.SaveAndAddToUser(delegate(ApiModel model)
            {
                ApiWorld savedBP = (ApiWorld)model;
                pipelineManager.blueprintId = savedBP.id;
                pipelineManager.assetBundleUnityVersion = Application.unityVersion;
                UnityEditor.EditorPrefs.SetString("blueprintID-" + pipelineManager.GetInstanceID().ToString(), savedBP.id);
                Debug.Log("Setting blueprintID on pipeline manager and editor prefs");
                doneUploading = true;
            });

            while(!doneUploading)
                yield return null;
        }
        
        IEnumerator UpdateBlueprint()
        {
            bool doneUploading = false;

            worldRecord.name = blueprintName.text;
            worldRecord.description = blueprintDescription.text;
            worldRecord.capacity = System.Convert.ToInt16(worldCapacity.text);
            worldRecord.assetUrl = cloudFrontAssetUrl;
            worldRecord.pluginUrl = cloudFrontPluginUrl;
            worldRecord.tags = BuildTags();
            worldRecord.releaseStatus = (releasePublic.isOn) ? ("public") : ("private");
			worldRecord.unityPackageUrl = cloudFrontUnityPackageUrl;
            worldRecord.isCurated = contentFeatured.isOn || contentSDKExample.isOn;

            if (shouldUpdateImageToggle.isOn)
            {
                yield return StartCoroutine(UploadImage());
                worldRecord.imageUrl = cloudFrontImageUrl;
                SetUploadProgress("Saving Blueprint", "Almost finished!!", 0.0f);
                worldRecord.Save(delegate (ApiModel model)
                {
                    doneUploading = true;
                });
            }
            else
            {
                SetUploadProgress("Saving Blueprint", "Almost finished!!", 0.0f);
                worldRecord.Save(delegate (ApiModel model)
                {
                    doneUploading = true;
                });
            }

            while (!doneUploading)
                yield return null;
        }
        
        void OnSDKPipelineComplete()
        {
            VRC.Core.Logger.Log("OnSDKPipelineComplete", DebugLevel.All);
            isUploading = false;
            pipelineManager.completedSDKPipeline = true;
            UnityEditor.EditorApplication.isPaused = false;
            UnityEditor.EditorApplication.isPlaying = false;
            UnityEditor.EditorUtility.ClearProgressBar();
            UnityEditor.EditorUtility.DisplayDialog("VRChat SDK", "Update Complete! Launch VRChat to see your upl content.", "Okay");
        }
        
        void ToggleUpdateImage(bool isOn)
        {
            if(isOn)
            {
                bpImage.enabled = false;
                liveBpImage.enabled = true;
            }
            else
            {
                bpImage.enabled = true;
                liveBpImage.enabled = false;
                ImageDownloader.DownloadImage(worldRecord.imageUrl, delegate(Texture2D obj) {
                    bpImage.texture = obj;
                });
            }
        }
        
        void SetUploadProgress(string title, string message, float progress)
        {
            uploadTitle = title;
            uploadMessage = message;
            uploadProgress = progress;
        }
        
        public static string GetGenericFileName(string ext)
        {
            System.DateTime D = System.DateTime.Now;
            string day = D.Day.ToString( "00" );
            string month = D.Month.ToString( "00" );
            string year = D.Year.ToString();
            string version = year + "_" + month + "_" + day;
            return version + "." + ext;
        }

        bool ValidateURLInput(InputField urlInput)
        {
            bool isValid = true;
            if(!string.IsNullOrEmpty(urlInput.text) && !VRC.Tools.IsValidURL(urlInput.text))
            {
                isUploading = false;
                UnityEditor.EditorUtility.DisplayDialog("Invalid Input", "Inputted plugin url is invalid. Please enter a valid plugin url or leave it empty", "OK");
                isValid = false;
            }
            return isValid;
        }

        bool ValidateNameInput(InputField nameInput)
        {
            bool isValid = true;
            if(string.IsNullOrEmpty(nameInput.text))
            {
                isUploading = false;
                UnityEditor.EditorUtility.DisplayDialog("Invalid Input", "Cannot leave the name field empty.", "OK");
                isValid = false;
            }  
            return isValid;
        }

        void OnDestroy()
        {
            UnityEditor.EditorUtility.ClearProgressBar();
            UnityEditor.EditorPrefs.DeleteKey("currentBuildingAssetBundlePath");
            UnityEditor.EditorPrefs.DeleteKey("externalPluginPath");
        }
        #endif
    }
}


