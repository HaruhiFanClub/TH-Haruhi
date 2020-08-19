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
        private UiColorTemplate colorTemplate;
        private UiImage _image;
        private Texture2D t;
        protected override void OnEnable()
        {
            base.OnEnable();
            t = new Texture2D(30, 20);
            colorTemplate = UiFrameTemplate.GetColorTemplate();
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
            _image = target as UiImage;
            serializedObject.Update();

            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            GUILayout.Label("Desc Color");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < colorTemplate.descColorList.Count; i++)
            {
                Color c = colorTemplate.descColorList[i];
                NewColorBtn(c);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("Rarity Text Color");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < colorTemplate.rarityTextList.Count; i++)
            {
                Color c = colorTemplate.rarityTextList[i];
                NewColorBtn(c);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("Rarity Image Color");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < colorTemplate.rarityImageList.Count; i++)
            {
                Color c = colorTemplate.rarityImageList[i];
                NewColorBtn(c);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label("Important Color");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < colorTemplate.importantColorList.Count; i++)
            {
                Color c = colorTemplate.importantColorList[i];
                NewColorBtn(c);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            EditorGUILayout.Space();

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
