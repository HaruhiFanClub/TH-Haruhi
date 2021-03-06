using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UiText), true)]
    [CanEditMultipleObjects]
    public class UiTextEditor : TextEditor
    {
        private UiText text;
        private Texture2D t;
        protected override void OnEnable()
        {
            base.OnEnable();
            t = new Texture2D(30, 20);
            text = target as UiText;


            //var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/res/ui/fonts/default.ttf");
            //text.font = font;
        }

        private void NewColorBtn(Color color)
        {
            int width = 30;
            float a = color.a;
            float a_w = (int)(a * width);
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < 5; k++)
                {
                    //伪装成alpha条
                    if (i <= a_w)
                    {
                        t.SetPixel(i, k, Color.white);
                    }

                    if (i > a_w)
                    {
                        t.SetPixel(i, k, Color.black);
                    }
                }

                for (int j = 5; j < 20; j++)
                {
                    t.SetPixel(i, j, color);
                }
            }
            t.Apply();

            if (GUILayout.Button(t, GUILayout.MaxWidth(30))) ClickColor();
        }

        public override void OnInspectorGUI()
        {

            text = target as UiText;
            serializedObject.Update();


            if (GUILayout.Button("AddOutLine"))
            {
                text.AddOutLine();
                DoDirty();
            }
            if (GUILayout.Button("RemoveOutLine"))
            {
                text.RemoveOutLine();
                DoDirty();
            }
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }

        public void ClickColor()
        {
            text.color = t.GetPixel(0, 5);
            DoDirty();
        }

        private void DoDirty()
        {
            if (text == null) return;
            var prefabStage = PrefabStageUtility.GetPrefabStage(text.gameObject);
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }
    }

}
