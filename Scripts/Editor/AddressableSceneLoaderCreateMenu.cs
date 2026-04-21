namespace CocodriloDog.App {

	using CocodriloDog.Core;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Creates an AddressableSceneLoader instance in the project panel.
	/// </summary>
	public static class AddressableSceneLoaderCreateMenu {


		#region Private Static Methods

		[MenuItem("Assets/Create/Cocodrilo Dog/App/Addresable Scene Loader", false)]
		private static void CreateAddressableSceneLoader() {
			PrefabCreateMenuUtility.CreatePrefab("Packages/com.cocodrilodog.app/Prefabs/AddressableSceneLoader.prefab");
		}

		[MenuItem("Assets/Create/Cocodrilo Dog/App/Addressable Scene Loader", true)]
		private static bool ValidateCreateAddressableSceneLoader() {
			return true; // Always valid since we can fall back to the root Assets folder
		}

		#endregion


	}

}