using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UiTextButton), true)]
    [CanEditMultipleObjects]
    public class UiTextButtonEditor : ButtonEditor
    {
        private UiTextButton _button;
        private bool _isEnable;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            _button = target as UiTextButton;
            if (_button == null) return;

            serializedObject.Update();

            _isEnable = EditorGUILayout.Toggle("isEnableButton", _button.IsEnable);
            if (_button.IsEnable != _isEnable)
            {
                _button.IsEnable = _isEnable;
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
