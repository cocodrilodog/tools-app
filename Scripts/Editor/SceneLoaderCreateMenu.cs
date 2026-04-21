namespace CocodriloDog.App {

	using CocodriloDog.Core;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Creates a SceneLoader instance in the project panel.
	/// </summary>
	public class SceneLoaderCreateMenu {


		#region Private Static Methods

		[MenuItem("Assets/Create/Cocodrilo Dog/App/Scene Loader", false)]
		private static void CreateSceneLoader() {
			PrefabCreateMenuUtility.CreatePrefab("Packages/com.cocodrilodog.app/Prefabs/SceneLoader.prefab");
		}

		[MenuItem("Assets/Create/Cocodrilo Dog/App/Scene Loader", true)]
		private static bool ValidateCreateSceneLoader() {
			return true; // Always valid since we can fall back to the root Assets folder
		}

		#endregion


	}

}