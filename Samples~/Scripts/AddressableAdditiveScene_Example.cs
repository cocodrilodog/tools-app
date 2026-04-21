namespace CocodriloDog.App.Examples {

	using System;
	using UnityEngine;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	using UnityEngine.ResourceManagement.ResourceProviders;

	[AddComponentMenu("")]
	public class AddressableAdditiveScene_Example : MonoBehaviour {


		#region Public Methods

		public void LoadScene() {
			m_SceneLoaderInvoker.LoadScene(h => m_SceneHandle = h);
		}

		public void UnloadScene() {
			Addressables.UnloadSceneAsync(m_SceneHandle);
		}

		#endregion
		

		#region Private Fields

		[SerializeField]
		private AddressableSceneLoaderInvoker m_SceneLoaderInvoker;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private AsyncOperationHandle<SceneInstance> m_SceneHandle;

		#endregion


	}

}