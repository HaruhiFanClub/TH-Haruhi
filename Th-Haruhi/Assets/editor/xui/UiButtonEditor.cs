using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UiButton), true)]
    [CanEditMultipleObjects]
    public class UiButtonEditor : ButtonEditor
    {
        SerializedProperty _debugProperty;
        private UiButton _button;
        private bool _isEnable;
        private bool _bUseTween;
        private bool _useDisplayImage;

        protected override void OnEnable()
        {
            base.OnEnable();
            _debugProperty = serializedObject.FindProperty("isDebug");
        }

        public override void OnInspectorGUI()
        {
            _button = target as UiButton;
            if (_button == null) return;

            serializedObject.Update();


            EditorGUILayout.PropertyField(_debugProperty);

            
            //button style change

            _bUseTween = EditorGUILayout.Toggle("UseTween", _button.UseTween);
            if (_button.UseTween != _bUseTween)
            {
                _button.SetUseTween(_bUseTween);
                DoDirty();
            }

            _isEnable = EditorGUILayout.Toggle("isEnableButton", _button.isEnable);
            if (_button.isEnable != _isEnable)
            {
                _button.isEnable = _isEnable;
                DoDirty();
            }

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        private void DoDirty()
        {
            if (_button == null) return;
            var prefabStage = PrefabStageUtility.GetPrefabStage(_button.gameObject);
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }
    }
}
