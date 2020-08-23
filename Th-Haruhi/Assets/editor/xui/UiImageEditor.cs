using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UiImage), true)]
    [CanEditMultipleObjects]
    public class UiImageEditor : ImageEditor
    {
        private UiImage _image;
        private Texture2D t;
        protected override void OnEnable()
        {
            base.OnEnable();
            t = new Texture2D(30, 20);
        }
        public override void OnInspectorGUI()
        {
            _image = target as UiImage;
            serializedObject.Update();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        public void ClickColor()
        {
            _image.color = t.GetPixel(0, 5);
            DoDirty();
        }

        private void DoDirty()
        {
            if (_image == null) return;
            var prefabStage = PrefabStageUtility.GetPrefabStage(_image.gameObject);
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }
    }
}
