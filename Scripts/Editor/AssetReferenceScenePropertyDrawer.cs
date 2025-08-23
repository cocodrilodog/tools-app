namespace CocodriloDog.App {

    using UnityEditor;
    using UnityEngine;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;

    [CustomPropertyDrawer(typeof(AssetReferenceScene))]
    public class AssetReferenceSceneDrawer : PropertyDrawer {


		#region Public Methods

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight;

		#endregion


		#region Unity Methods

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            // Addressables runtime serializes the GUID in "m_AssetGUID"
            var guidProp = property.FindPropertyRelative("m_AssetGUID");
            if (guidProp == null) {
                EditorGUI.HelpBox(position, "Invalid AssetReference serialization.", MessageType.Error);
                return;
            }

            // Resolve current value
            string guid = guidProp.stringValue;
            string path = string.IsNullOrEmpty(guid) ? null : AssetDatabase.GUIDToAssetPath(guid);
            SceneAsset current = string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

            // Layout: [Label][space][ObjectField]
            position = EditorGUI.IndentedRect(position);
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var space = 2;
            var fieldRect = new Rect(labelRect.xMax + space, position.y, position.width - EditorGUIUtility.labelWidth - space, position.height);

            EditorGUI.LabelField(labelRect, label);

            EditorGUI.BeginChangeCheck();
            var picked = (SceneAsset)EditorGUI.ObjectField(fieldRect, current, typeof(SceneAsset), false);
            if (EditorGUI.EndChangeCheck()) {
                if (picked == null) {
                    guidProp.stringValue = string.Empty;
                } else {
                    var pickedPath = AssetDatabase.GetAssetPath(picked);
                    if (!pickedPath.EndsWith(".unity")) {
                        ShowSceneOnlyDialog();
                    } else {
                        // Ensure it is in Addressables (Default Group if missing)
                        string pickedGuid = AssetDatabase.AssetPathToGUID(pickedPath);
                        EnsureAddressable(pickedGuid);
                        guidProp.stringValue = pickedGuid;
                    }
                }
            }

        }

		#endregion


		#region Private Static Methods

		private static void ShowSceneOnlyDialog() {
            EditorUtility.DisplayDialog("Scene Only",
                "This field accepts only Scene assets (.unity).",
                "OK");
        }

        // TODO: Review this
        private static void EnsureAddressable(string guid) {

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) {
                // If Addressables is not configured yet, show a friendly message.
                EditorUtility.DisplayDialog("Addressables not configured",
                    "AddressableAssetSettings could not be found. " +
                    "Open 'Window > Asset Management > Addressables > Groups' to create the default settings.",
                    "OK");
                return;
            }

            var entry = settings.FindAssetEntry(guid);
            if (entry == null) {
                // Add to Default Group (or create if missing)
                var group = settings.DefaultGroup ?? settings.CreateGroup(
                    "Default Local Group", false, false, false, settings.DefaultGroup.Schemas, typeof(AddressableAssetGroup));
                settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: true);
                AssetDatabase.SaveAssets();
            }

        }

		#endregion


	}

}
