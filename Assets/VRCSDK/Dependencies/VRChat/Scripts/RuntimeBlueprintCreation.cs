using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VRC.Core;

namespace VRCSDK2
{
    public class RuntimeBlueprintCreation : MonoBehaviour 
    {
        public VRC.Core.PipelineManager pipelineManager;
        
        public GameObject waitingPanel;
        public GameObject blueprintPanel;
        public GameObject errorPanel;
        
		public Text titleText;
        public InputField blueprintName;
        public InputField blueprintDescription;
        public RawImage bpImage;
        public Image liveBpImage;
        public Toggle shouldUpdateImageToggle;
        public Toggle contentSex;
        public Toggle contentViolence;
        public Toggle contentGore;
        public Toggle contentOther;
		public Toggle developerAvatar;

        public UnityEngine.UI.Button uploadButton;

        public string uploadVrcPath;
        public string uploadPluginPath;
        public string uploadUnityPackagePath;

        public string cloudFrontAssetUrl;
        public string cloudFrontImageUrl;
        public string cloudFrontPluginUrl;
        public string cloudFrontUnityPackageUrl;

        private ApiAvatar apiAvatar;
        private string abExtension;
        private CameraImageCapture imageCapture;
        private string assetBundleUrl;
        
        private bool isUpdate { get { return !string.IsNullOrEmpty(pipelineManager.blueprintId); } } 
        
        public bool isUploading = false;
        public float uploadProgress = 0f;
        public string uploadMessage;
        public string uploadTitle;

        #if UNITY_EDITOR       
        void Start()
        {
            if(Application.isEditor && Application.isPlaying)
            {
                Application.runInBackground = true;
                UnityEngine.VR.VRSettings.enabled = false;

				PipelineSaver ps = GameObject.FindObjectOfType<PipelineSaver>();
				pipelineManager = ps.gameObject.GetComponent<PipelineManager>();

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

        void LoginErrorCallback(string obj)
        {
            VRC.Core.Logger.LogError("Could not fetch fresh user - " + obj, DebugLevel.Always);    
        }

        void Login()
        {
            System.Action cachedLogin = delegate
            {
                pipelineManager.user = APIUser.CachedLogin(
                    delegate(APIUser user) 
                    {
                        pipelineManager.user = user;
                        if (isUpdate)
                        {
                            ApiAvatar.Fetch(pipelineManager.blueprintId,
                                delegate (ApiAvatar avatar)
                                {
                                    apiAvatar = avatar;
                                    SetupUI();
                                }, 
                                delegate(string message) 
                                {
                                    pipelineManager.blueprintId = "";
                                    SetupUI();
                                });
                        }
                        else
                        {
                            SetupUI();
                        }
                    }, LoginErrorCallback, true);
            };

            cachedLogin();
        }
        
        void SetupUI()
        {
            if(APIUser.Exists(pipelineManager.user) && pipelineManager.user.isFresh)
            {
                waitingPanel.SetActive(false);
                blueprintPanel.SetActive(true);
                errorPanel.SetActive(false);

                if (isUpdate)
                {
                    // bp update
                    if (apiAvatar.authorId == pipelineManager.user.id)
                    {
						titleText.text= "Update Avatar";
                        // apiAvatar = pipelineManager.user.GetBlueprint(pipelineManager.blueprintId) as ApiAvatar;
                        blueprintName.text = apiAvatar.name;
                        contentSex.isOn = apiAvatar.tags.Contains("content_sex");
                        contentViolence.isOn = apiAvatar.tags.Contains("content_violence");
                        contentGore.isOn = apiAvatar.tags.Contains("content_gore");
                        contentOther.isOn = apiAvatar.tags.Contains("content_other");
						developerAvatar.isOn = apiAvatar.tags.Contains("developer");
                        blueprintDescription.text = apiAvatar.description;
                        shouldUpdateImageToggle.interactable = true;
                        shouldUpdateImageToggle.isOn = false;
                        liveBpImage.enabled = false;
                        bpImage.enabled = true;

                        ImageDownloader.DownloadImage(apiAvatar.imageUrl, delegate(Texture2D obj) {
                            bpImage.texture = obj;
                        });
                    }
                    else // user does not own apiAvatar id associated with descriptor
                    {
                        blueprintPanel.SetActive(false);
                        errorPanel.SetActive(true);
                    }
                }
                else
                {
					titleText.text = "New Avatar";
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
            }

			if(APIUser.CurrentUser.developerType == APIUser.DeveloperType.Internal)
				developerAvatar.gameObject.SetActive(true);
			else
				developerAvatar.gameObject.SetActive(false);
        }
        
        public void SetupUpload()
        {
            uploadTitle = "Preparing For Upload";
            isUploading = true;
           
            string abPath = UnityEditor.EditorPrefs.GetString("currentBuildingAssetBundlePath");
            abExtension = System.IO.Path.GetExtension(abPath);

            string pluginPath = UnityEditor.EditorPrefs.GetString("externalPluginPath");

			string unityPackagePath = UnityEditor.EditorPrefs.GetString("VRC_exportedUnityPackagePath");

            UnityEditor.EditorPrefs.SetBool("VRCSDK2_scene_changed", true );
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_sex", contentSex.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_violence", contentViolence.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_gore", contentGore.isOn);
            UnityEditor.EditorPrefs.SetBool("VRCSDK2_content_other", contentOther.isOn);

            string avatarId = isUpdate ? apiAvatar.id : "new_" + VRC.Tools.GetRandomDigits(6);
            int version = isUpdate ? apiAvatar.version+1 : 1;
            PrepareVRCPathForS3(abPath, avatarId, version);

			if(!string.IsNullOrEmpty(pluginPath) && System.IO.File.Exists(pluginPath))
            {
				Debug.Log("Found plugin path. Preparing to upload!");
                PreparePluginPathForS3(pluginPath, avatarId, version);
            }
            else
            {
				Debug.Log("Did not find plugin path. No upload occuring!");
            }

			if(!string.IsNullOrEmpty(unityPackagePath) && System.IO.File.Exists(unityPackagePath))
			{
				Debug.Log("Found unity package path. Preparing to upload!");
				PrepareUnityPackageForS3(unityPackagePath, avatarId, version);
			}

            StartCoroutine(Upload());
        }

		void PrepareUnityPackageForS3(string packagePath, string avatarId, int version)
		{
			uploadUnityPackagePath = Application.temporaryCachePath + "/" + avatarId + "_" + version.ToString() + ".unitypackage";
			uploadUnityPackagePath.Trim();
			uploadUnityPackagePath.Replace(' ', '_');

			if(System.IO.File.Exists(uploadUnityPackagePath))
				System.IO.File.Delete(uploadUnityPackagePath);

			System.IO.File.Copy(packagePath, uploadUnityPackagePath);
		}

        void PrepareVRCPathForS3(string abPath, string avatarId, int version)
        {
            uploadVrcPath = Application.temporaryCachePath + "/" + avatarId + "_" + version.ToString() + abExtension;
            uploadVrcPath.Trim();
            uploadVrcPath.Replace(' ', '_');
            
            if(System.IO.File.Exists(uploadVrcPath))
                System.IO.File.Delete(uploadVrcPath);
            
            System.IO.File.Copy(abPath, uploadVrcPath);
        }

        void PreparePluginPathForS3(string pluginPath, string avatarId, int version)
        {
            uploadPluginPath = Application.temporaryCachePath + "/" + avatarId + "_" + version.ToString() + ".dll";
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
			SetUploadProgress("Uploading Unity Package...","Future proofing your content!",  0.05f);
			bool doneUploading = false;

			string filePath = uploadUnityPackagePath;
			string s3FolderName = "unitypackages";
			Uploader.UploadFile(filePath, s3FolderName, delegate(string obj) {
				string fileName = s3FolderName + "/" + System.IO.Path.GetFileName(filePath);
				cloudFrontUnityPackageUrl = "http://dbinj8iahsbec.cloudfront.net/" + fileName;
				doneUploading = true;
			});

			while(!doneUploading)
				yield return null;
		}

        IEnumerator UploadDLL()
        {
            Debug.Log("Uploading Plugin...");
            SetUploadProgress("Uploading plugin...","Pushing those upload speeds!!",  0.05f);
            bool doneUploading = false;

            string filePath = uploadPluginPath;
            string s3FolderName = "plugins";
            Uploader.UploadFile(filePath, s3FolderName, delegate(string obj) {
                string fileName = s3FolderName + "/" + System.IO.Path.GetFileName(filePath);
                cloudFrontPluginUrl = "http://dbinj8iahsbec.cloudfront.net/" + fileName;
                doneUploading = true;
            });

            while(!doneUploading)
                yield return null;
        }

        IEnumerator UploadVRCFile()
        {
            Debug.Log("Uploading VRC File...");
            SetUploadProgress("Uploading asset...","Pushing those upload speeds!!",  0.1f);
            bool doneUploading = false;
            
            string filePath = uploadVrcPath;
            string s3FolderName = "avatars";
            Uploader.UploadFile(filePath, s3FolderName, delegate(string obj) {
                string fileName = s3FolderName + "/" + System.IO.Path.GetFileName(filePath);
                cloudFrontAssetUrl = "http://dbinj8iahsbec.cloudfront.net/" + fileName;
                doneUploading = true;
            });

            while(!doneUploading)
                yield return null;
        }

        IEnumerator UploadImage()
        {
            Debug.Log("Uploading Image...");

            bool doneUploading = false;
            SetUploadProgress("Uploading Image...","That's a nice looking preview image ;)", 0.3f);
            string imagePath = imageCapture.TakePicture();
            Uploader.UploadFile(imagePath, "images", delegate(string imageUrl)
            {
                cloudFrontImageUrl = imageUrl;
                doneUploading = true;
                VRC.Core.Logger.Log("Successfully uploaded image.", DebugLevel.All);
            });

            while(!doneUploading)
                yield return null;
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
                if (developerAvatar.isOn)
                    tags.Add("developer");
            }

            return tags;
        }
        
        IEnumerator CreateBlueprint()
        {
            yield return StartCoroutine(UploadImage());

            ApiAvatar apiAvatar = ScriptableObject.CreateInstance<ApiAvatar>();
            apiAvatar.Init(
                pipelineManager.user,
                blueprintName.text,
                cloudFrontImageUrl,
                cloudFrontAssetUrl,
                blueprintDescription.text,
                BuildTags(),
                cloudFrontUnityPackageUrl
                );

            bool doneUploading = false;

            apiAvatar.Save(delegate(ApiModel model)
            {
                ApiAvatar savedBP = (ApiAvatar)model;
                pipelineManager.blueprintId = savedBP.id;
                pipelineManager.assetBundleUnityVersion = Application.unityVersion;
                UnityEditor.EditorPrefs.SetString("blueprintID-" + pipelineManager.GetInstanceID().ToString(), savedBP.id);
                doneUploading = true;
            });

            while (!doneUploading)
                yield return null;
        }
        
        IEnumerator UpdateBlueprint()
        {
            bool doneUploading = false;

            apiAvatar.name = blueprintName.text;
            apiAvatar.description = blueprintDescription.text;
            apiAvatar.assetUrl = cloudFrontAssetUrl;
            apiAvatar.tags = BuildTags();
            apiAvatar.unityPackageUrl = cloudFrontUnityPackageUrl;

            if (shouldUpdateImageToggle.isOn)
            {
                yield return StartCoroutine(UploadImage());
                apiAvatar.imageUrl = cloudFrontImageUrl;
                SetUploadProgress("Saving Avatar", "Almost finished!!", 0.8f);
                apiAvatar.Save(delegate(ApiModel model) 
                {
                    doneUploading = true;
                });
            }
            else
            {
                SetUploadProgress("Saving Avatar", "Almost finished!!", 0.8f);
                apiAvatar.Save(delegate(ApiModel model) 
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
                ImageDownloader.DownloadImage(apiAvatar.imageUrl, delegate(Texture2D obj) {
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


