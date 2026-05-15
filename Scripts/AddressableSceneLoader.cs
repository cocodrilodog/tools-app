namespace CocodriloDog.App {

	using CocodriloDog.Core;
	using CocodriloDog.MotionKit;
	using System;
	using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
	using UnityEngine.Events;
	using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.ResourceManagement.ResourceProviders;
	using UnityEngine.SceneManagement;

	public class AddressableSceneLoader : MonoBehaviour {


        #region Small Types

        public enum HideUIMode {
            DontHide,
            OnActivateScene,
            OnActivateSceneButtonClick
        }

        #endregion


        #region Public Methods

		/// <summary>
		/// Load the addressable scene.
		/// </summary>
		/// <param name="sceneReference">The scene reference.</param>
		/// <param name="loadSceneMode">The load mode.</param>
		/// <param name="autoActivate">Will the scene auto activate?</param>
		/// <param name="hideUIMode">The hide UI mode.</param>
		/// <param name="onSceneHandleReady">a callback that receives the scene load handle when ready.</param>
        public void LoadScene(
			AssetReferenceScene sceneReference, 
			LoadSceneMode loadSceneMode, 
			bool autoActivate = true, 
			HideUIMode hideUIMode = HideUIMode.OnActivateScene,
			Action<AsyncOperationHandle<SceneInstance>> onSceneHandleReady = null
		) {
            m_UI.OnLoadProgress(0);
            EnableCanvases();
            ShowUI(true, () => {
				var handle = _LoadScene(sceneReference, loadSceneMode, autoActivate, hideUIMode);
				onSceneHandleReady?.Invoke(handle);
			});
        }

        /// <summary>
        /// Activates the scene.
        /// </summary>
        public void ActivateScene() {
            m_AsyncOperationHandle.Result.ActivateAsync();
		}

        /// <summary>
        /// Hides the loader UI.
        /// </summary>
        /// <param name="animated">Hiding will be animated.</param>
        public void HideUI(bool animated) => HideUI(animated, null);

        /// <summary>
        /// Hides the loader UI.
        /// </summary>
        /// <param name="animated">Hiding will be animated.</param>
        /// <param name="onComplete">An action to invoke on complete</param>
        public void HideUI(bool animated, Action onComplete) {
            if (animated) {
                m_UI.OnFadeOut(m_FadeOutTime);
                MotionKit.GetMotion(this, AlphaKey, v => m_UI.CanvasGroup.alpha = v)
                    .SetTimeMode(TimeMode.Unscaled)
                    .SetEasing(MotionKitEasing.QuadInOut)
                    .SetOnComplete(() => {
                        m_UI.gameObject.SetActive(false);
                        DisableCanvases();
                        onComplete?.Invoke();
                    })
                    .Play(1, 0, m_FadeOutTime);
            } else {
                m_UI.CanvasGroup.alpha = 0;
                m_UI.gameObject.SetActive(false);
            }
        }

        #endregion


        #region Unity Methods

        protected void Awake() {
            DontDestroyOnLoad(gameObject);
            DisableCanvases();
            HideUI(false);
        }

        private void OnEnable() => SubscribeToUI();

        // Initialize so that it is not created OnDestroy()
		private void Start() => _ = MotionKit.Instance;

        private void OnValidate() {
            m_FadeInTime = Mathf.Clamp(m_FadeInTime, 0, float.MaxValue);
            m_FadeOutTime = Mathf.Clamp(m_FadeOutTime, 0, float.MaxValue);
        }

        private void Reset() {
            m_FadeInTime = 0.2f;
            m_FadeOutTime = 0.2f;
        }

        private void OnDisable() => UnsubscribeFromUI();

        protected void OnDestroy() => MotionKit.ClearPlaybacks(this);

        #endregion


        #region Event Handlers

        private void ActivateSceneButton_onClick() {
			if (m_AsyncOperationHandle.IsValid()) {
                ActivateScene();
			}
            if (m_HideUIMode == HideUIMode.OnActivateSceneButtonClick) {
                m_HideUIMode = default;
                HideUI(true);
            }
        }

        #endregion


        #region Private Constants

        private const string AlphaKey = "Alpha";

        #endregion


        #region Private Fields - Non Serialized

        [Tooltip("The duration of the fade in effect.")]
        [SerializeField]
        public float m_FadeInTime = 0.2f;

        [Tooltip("The duration of the fade out effect.")]
        [SerializeField]
        public float m_FadeOutTime = 0.2f;

        [Tooltip("The UI")]
        [SerializeField]
        private AbstractSceneLoaderUI m_UI;

		[Space]

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnEnableCanvases;

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnDisableCanvases;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private AsyncOperationHandle<SceneInstance> m_AsyncOperationHandle;

        [NonSerialized]
        private Canvas[] m_Canvases;

        [NonSerialized]
        private HideUIMode m_HideUIMode;

        #endregion


        #region Private Properties

        private Canvas[] Canvases {
            get {
                if (m_Canvases == null) {
                    m_Canvases = GetComponentsInChildren<Canvas>();
                }
                return m_Canvases;
            }
        }

        #endregion


        #region Private Methods

        private AsyncOperationHandle<SceneInstance> _LoadScene(
			AssetReferenceScene sceneReference, 
			LoadSceneMode loadSceneMode, 
			bool autoActivate, 
			HideUIMode hideUIMode = HideUIMode.OnActivateScene
		) {

			m_HideUIMode = hideUIMode;
            m_UI.OnLoadStart();
            m_AsyncOperationHandle = sceneReference.LoadSceneAsync(loadSceneMode, autoActivate);

			StartCoroutine(TrackProgress());
            IEnumerator TrackProgress() {        

                while (!m_AsyncOperationHandle.IsDone) {
                    m_UI.OnLoadProgress(m_AsyncOperationHandle.PercentComplete);
                    yield return null;
				}

                m_UI.OnLoadProgress(1);
                m_UI.OnLoadComplete();

				while (!m_AsyncOperationHandle.Result.Scene.isLoaded) {
                    yield return null;
				}

				if (m_HideUIMode == HideUIMode.OnActivateScene) {
                    m_HideUIMode = default;
                    HideUI(true);
                }

            }

			return m_AsyncOperationHandle;

        }

        private void EnableCanvases() {
            foreach (Canvas canvas in Canvases) {
                canvas.enabled = true;
            }
			m_OnEnableCanvases.Invoke();
        }

        private void DisableCanvases() {
            foreach (Canvas canvas in Canvases) {
                canvas.enabled = false;
            }
			m_OnDisableCanvases.Invoke();
        }

        private void ShowUI(bool animated, Action onComplete = null) {
            m_UI.gameObject.SetActive(true);
            if (animated) {
                m_UI.OnFadeIn(m_FadeInTime);
                MotionKit.GetMotion(this, AlphaKey, v => m_UI.CanvasGroup.alpha = v)
                    .SetTimeMode(TimeMode.Unscaled)
                    .SetEasing(MotionKitEasing.QuadInOut)
                    .SetOnComplete(() => onComplete?.Invoke())
                    .Play(0, 1, m_FadeInTime);
            } else {
                m_UI.CanvasGroup.alpha = 1;
                onComplete?.Invoke();
            }
        }

        private void SubscribeToUI() {
            if (m_UI.ActivateSceneButton != null) {
                m_UI.ActivateSceneButton.onClick.AddListener(ActivateSceneButton_onClick);
            }
        }

        private void UnsubscribeFromUI() {
            if (m_UI.ActivateSceneButton != null) {
                m_UI.ActivateSceneButton.onClick.RemoveListener(ActivateSceneButton_onClick);
            }
        }

        #endregion


    }

}