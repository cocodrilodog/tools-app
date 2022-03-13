namespace CocodriloDog.App {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;

	public class DefaultSceneLoader : MonoBehaviour {


		#region Public Fields

		[SerializeField]
		public string DefaultSceneName;

		[SerializeField]
		public LoadSceneMode LoadSceneMode;

		[SerializeField]
		public bool AutoActivate = true;

		#endregion


		#region Unity Methods

		private void Start() {
#if CB_CODE
			StartCoroutine(WaitUntilReadSavegame());
			IEnumerator WaitUntilReadSavegame()
			{
				yield return new WaitUntil(() => CasualBrothers.Platforms.SavegameManager.Instance.IsReady);
				SceneLoader.LoadScene(DefaultSceneName, LoadSceneMode, AutoActivate);
			}
#else
			SceneLoader.LoadScene(DefaultSceneName, LoadSceneMode, AutoActivate);
#endif
		}

        #endregion


    }
}