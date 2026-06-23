using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LLib.Editor
{
    public class TestToolSettingsModule : BaseTestToolModule
    {
        private ReorderableList _reorderableList;

        private ReorderableList ReorderableList
        {
            get
            {
                if (_reorderableList == null)
                {
                    var modules = Settings.Modules;

                    _reorderableList = new ReorderableList(
                        modules,
                        typeof(BaseTestToolModule),
                        true,
                        false,
                        true,
                        true);

                    _reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        var previousValue = modules[index];

                        rect.y += 2.45f;

                        modules[index] = (BaseTestToolModule)EditorGUI.ObjectField(
                            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                            modules[index],
                            typeof(BaseTestToolModule),
                            false
                        );

                        if (previousValue != modules[index] && modules[index] != null) modules[index].Init();

                        Settings.Save();
                    };

                    _reorderableList.onAddCallback = list => modules.Add(null);
                }

                return _reorderableList;
            }
        }

        private TestToolSettings Settings => TestToolSettings.instance;


        public override void Init()
        {
        }


        public override void OnGUI()
        {
            DrawTop();
            EditorGUILayout.Space(20f);

            DrawBottom();
            EditorGUILayout.Space(20f);

            DrawCategory();
            EditorGUILayout.Space(20f);

            DrawContents();
            EditorGUILayout.Space(20f);

            DrawModules();
            EditorGUILayout.Space(20f);

            if (GUILayout.Button("SAVE", EditorStyles.miniButton)) Settings.Save();
        }


        private void DrawModules()
        {
            EditorGUILayout.LabelField("Modules", EditorStyles.boldLabel);
            ReorderableList.DoLayoutList();
        }


        private void DrawTop()
        {
            EditorGUILayout.LabelField("Top", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                Settings.TitleFontSize = EditorGUILayout.IntField("Font Size", Settings.TitleFontSize);
                Settings.TitleFontColor = EditorGUILayout.ColorField("Font Color", Settings.TitleFontColor);
                Settings.TitleFontShadowColor =
                    EditorGUILayout.ColorField("Font Shadow Color", Settings.TitleFontShadowColor);
                Settings.TitleUnderLineColor =
                    EditorGUILayout.ColorField("UnderLine Color", Settings.TitleUnderLineColor);
                Settings.TitleUnderLineHighlightColor = EditorGUILayout.ColorField("UnderLine Highlight Color",
                    Settings.TitleUnderLineHighlightColor);

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawBottom()
        {
            EditorGUILayout.LabelField("Bottom", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                Settings.BottomBackgroundColor =
                    EditorGUILayout.ColorField("Background Color", Settings.BottomBackgroundColor);

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawCategory()
        {
            EditorGUILayout.LabelField("Category", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                Settings.CategoryRectWidth = EditorGUILayout.FloatField("Width", Settings.CategoryRectWidth);
                EditorGUILayout.Space(5f);

                Settings.ButtonFontSize = EditorGUILayout.IntField("Font Size", Settings.ButtonFontSize);
                Settings.ButtonFontNormalColor =
                    EditorGUILayout.ColorField("Font Normal Color", Settings.ButtonFontNormalColor);
                Settings.ButtonFontHoverColor =
                    EditorGUILayout.ColorField("Font Hover Color", Settings.ButtonFontHoverColor);
                EditorGUILayout.Space(5f);
                Settings.ButtonHeight = EditorGUILayout.FloatField("Button Height", Settings.ButtonHeight);
                Settings.ButtonNormalColor =
                    EditorGUILayout.ColorField("Button Normal Color", Settings.ButtonNormalColor);
                Settings.ButtonHighlightColor =
                    EditorGUILayout.ColorField("Button Highlight Color", Settings.ButtonHighlightColor);

                EditorGUILayout.EndVertical();
            }
        }


        private void DrawContents()
        {
            EditorGUILayout.LabelField("Contents", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                Settings.ContentsBackgroundColor =
                    EditorGUILayout.ColorField("Background Color", Settings.ContentsBackgroundColor);

                EditorGUILayout.EndVertical();
            }
        }
    }
}