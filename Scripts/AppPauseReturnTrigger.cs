namespace CocodriloDog.App {

	using CocodriloDog.Core;
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	public class AppPauseReturnTrigger : MonoBehaviour {


		#region Unity Methods

		private void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus) {
				m_IsInBackground = true;
				OnAppPause();
			} else if (m_IsInBackground) {
				m_IsInBackground = false;
				OnReturnToApp();
			}
		}

		private void OnApplicationFocus(bool hasFocus) {
			if (!hasFocus) {
				m_IsInBackground = true;
				OnAppPause();
			} else if (m_IsInBackground) {
				m_IsInBackground = false;
				OnReturnToApp();
			}
		}

		#endregion


		#region Private Fields - Serialized

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnAppPause;

		[UnityEventGroup("Events")]
		[SerializeField]
		private UnityEvent m_OnReturnToApp;

		#endregion


		#region Private Fields - Non Serialized

		[NonSerialized]
		private bool m_IsInBackground;

		#endregion


		#region Private Methods

		private void OnAppPause() => m_OnAppPause.Invoke();

		private void OnReturnToApp() => m_OnReturnToApp.Invoke();

		#endregion


	}

}