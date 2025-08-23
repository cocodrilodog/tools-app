namespace CocodriloDog.App {

    using UnityEngine.AddressableAssets;
    using UnityEngine;
	using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif

	[Serializable]
    public class AssetReferenceScene : AssetReference {


		#region Constructor

		public AssetReferenceScene(string guid) : base(guid) { }

		#endregion


		#region Public Methods

#if UNITY_EDITOR
		public override bool ValidateAsset(UnityEngine.Object obj) => obj is SceneAsset;
        
		public override bool ValidateAsset(string path) => path.EndsWith(".unity");
#endif

		#endregion

	}

}