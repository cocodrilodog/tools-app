namespace CocodriloDog.App {

	using CocodriloDog.Core;
	using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
	using UnityEngine.SceneManagement;

	public class AddressableSceneLoaderInvoker : MonoBehaviour {


		#region Public Methods

		[Button(index:6, disableInEditMode:true)]
		public void LoadScene() => m_SceneLoader.Value.LoadScene(m_SceneReference, m_LoadSceneMode, m_AutoActivate, m_HideUIMode);

		#endregion


		#region Private Fields

		[Tooltip("A reference to the AddressableSceneLoader that this trigger will use.")]
		[SerializeField]
		private ScriptableReferenceField<AddressableSceneLoader> m_SceneLoader;

		[Tooltip("An addressable scene reference of the scene to load.")]
		[SerializeField]
		private AssetReferenceScene m_SceneReference;

		[Tooltip("The load scene mode.")]
		[SerializeField]
		private LoadSceneMode m_LoadSceneMode;

		[Tooltip("Activate the scene upon loading")]
		[SerializeField]
		private bool m_AutoActivate = true;

		[Tooltip("Behaviour for hiding the UI")]
		[SerializeField]
		private AddressableSceneLoader.HideUIMode m_HideUIMode = AddressableSceneLoader.HideUIMode.OnActivateScene;

		[Tooltip("Load the scene on Start")]
		[SerializeField]
		private bool m_LoadSceneOnStart;

		#endregion


	}

}