using UnityEditor;
using UnityEngine;

namespace LLib.Editor
{
    public class TestTool : EditorWindow
    {
        private Vector2 _scrollPos;
        private int _selectedIndex;
        private BaseTestToolModule _selectedModule;
        private TestToolSettingsModule _settingsModule;


        private TestToolSettings Settings => TestToolSettings.instance;


        private TestToolSettingsModule SettingsModule
        {
            get
            {
                if (_settingsModule == null) _settingsModule = CreateInstance<TestToolSettingsModule>();

                _settingsModule.Title = "SETTINGS";
                _settingsModule.IsRunTimeOnly = false;

                return _settingsModule;
            }
        }


        private void OnEnable()
        {
            Settings.InitStyles();


            foreach (var module in Settings.Modules) module.Init();

            SettingsModule.Init();


            EditorApplication.update += Repaint;
        }


        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }


        private void OnGUI()
        {
            if (Settings == null)
                return;


            _scrollPos =
                EditorGUILayout.BeginScrollView(_scrollPos, false, false, GUIStyle.none, GUIStyle.none, GUIStyle.none);
            {
                DrawTitle();
                DrawUnderline(GUILayoutUtility.GetLastRect());

                var mainRect = EditorGUILayout.BeginVertical(Settings.MainRectStyle);
                {
                    EditorGUI.DrawRect(mainRect, Settings.BottomBackgroundColor);
                    DrawGrid(mainRect);

                    if (_selectedModule == null)
                    {
                        EditorGUILayout.Space(2);
                        DrawCategoryButtons();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical(Settings.CategorySideRectStyle,
                                GUILayout.Width(Settings.CategoryRectWidth), GUILayout.ExpandHeight(true));
                            {
                                EditorGUILayout.Space(2);
                                DrawCategoryButtons();

                                EditorGUILayout.EndVertical();
                            }

                            var contentsRect = EditorGUILayout.BeginVertical(Settings.ContentRectStyle,
                                GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                            {
                                EditorGUI.DrawRect(contentsRect, Settings.ContentsBackgroundColor);

                                DrawContents(contentsRect);
                                EditorGUILayout.EndVertical();
                            }

                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndScrollView();
            }
        }


        [MenuItem("Window/[ LLib ]/Test Tool", false, int.MinValue)]
        public static void Open()
        {
            var window = GetWindow<TestTool>();
            window.titleContent = new GUIContent("Test Tool");
            window.Show();
        }

        #region TOP

        private void DrawTitle()
        {
            EditorGUILayout.Space(20f);

            var content = EditorGUIUtility.TrTextContent("TEST TOOL");
            var rect = GUILayoutUtility.GetRect(content, Settings.TitleStyle);


            GUI.Label(new Rect(rect.x + 2.5f, rect.y + 2.5f, rect.width, rect.height), content,
                Settings.TitleShadowStyle);
            GUI.Label(rect, content, Settings.TitleStyle);

            EditorGUILayout.Space(20f);
        }

        private void DrawUnderline(Rect rect)
        {
            var lineY = rect.yMax - 2.5f;

            var lineColor = Settings.TitleUnderLineColor;
            var lineColorShadowColor = lineColor * 0.4f;

            EditorGUI.DrawRect(new Rect(rect.x, lineY - 1f, rect.width, 1), lineColorShadowColor);
            EditorGUI.DrawRect(new Rect(rect.x, lineY, rect.width, 2), lineColor);

            var highLightColor = Settings.TitleUnderLineHighlightColor;
            var highLightShadowColor = highLightColor * 0.4f;

            EditorGUI.DrawRect(new Rect(rect.center.x - 20, lineY - 1f, 40, 1), highLightShadowColor);
            EditorGUI.DrawRect(new Rect(rect.center.x - 20, lineY, 40, 2), Settings.TitleUnderLineHighlightColor);
        }

        #endregion

        #region CATEGORY

        private void DrawCategoryButtons()
        {
            for (var i = 0; i <= Settings.Modules.Count; i++)
            {
                var target = i == Settings.Modules.Count ? SettingsModule : Settings.Modules[i];
                if (target == null)
                    continue;

                var label = target.Title;

                if (_selectedModule == target && _selectedIndex == i)
                {
                    var btnRect = GUILayoutUtility.GetRect(new GUIContent(label), Settings.BtnSelectedStyle);

                    EditorGUI.DrawRect(btnRect, Settings.ContentsBackgroundColor);

                    GUI.Label(btnRect, label, Settings.BtnSelectedStyle);

                    var outlineColor = Settings.ButtonHighlightColor;
                    var thickness = 2f;

                    EditorGUI.DrawRect(new Rect(btnRect.x, btnRect.y, btnRect.width, thickness), outlineColor);
                    EditorGUI.DrawRect(new Rect(btnRect.x, btnRect.yMax - thickness, btnRect.width, thickness),
                        outlineColor);
                    EditorGUI.DrawRect(new Rect(btnRect.x, btnRect.y, thickness, btnRect.height), outlineColor);
                    EditorGUI.DrawRect(new Rect(btnRect.xMax - thickness, btnRect.y, thickness, btnRect.height),
                        outlineColor);

                    if (GUI.Button(btnRect, "", GUIStyle.none)) _selectedModule = null;
                }
                else
                {
                    GUI.backgroundColor = target.UseCustomButtonColor ? target.ButtonColor : Settings.ButtonNormalColor;

                    if (GUILayout.Button(label, Settings.BtnStyle))
                    {
                        _selectedModule = target;
                        _selectedIndex = i;
                    }

                    GUI.backgroundColor = Color.white;
                }
            }
        }

        private void DrawGrid(Rect rect)
        {
            var gridColor = new Color(1, 1, 1, 0.03f);

            for (var i = rect.y; i < rect.yMax; i += 15)
                EditorGUI.DrawRect(new Rect(rect.x, i, rect.width, 1f), gridColor);

            for (var i = rect.x; i < rect.xMax; i += 15)
                EditorGUI.DrawRect(new Rect(i, rect.y, 1f, rect.height), gridColor);
        }

        #endregion

        #region Contents

        private void DrawContents(Rect contentsRect)
        {
            if (_selectedModule != null)
            {
                if (_selectedModule.IsRunTimeOnly &&
                    !Application.isPlaying)
                {
                    GUILayout.FlexibleSpace();
                    var message = "⚠️ RUNTIME ONLY";
                    GUILayout.Label(message, Settings.NoticeStyle);
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    _selectedModule.OnGUI();
                }
            }

            HandleModuleMenu(contentsRect, _selectedModule);
        }

        private void HandleModuleMenu(Rect rect, BaseTestToolModule module)
        {
            var current = Event.current;

            if (current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent($"{module.GetType().Name} Script"), false, () => OpenModuleScript(module));

                menu.ShowAsContext();

                current.Use();
            }
        }


        private void OpenModuleScript(BaseTestToolModule module)
        {
            var type = module.GetType();

            var guids = AssetDatabase.FindAssets($"{type.Name} t:MonoScript");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script != null && script.GetClass() == type)
                {
                    AssetDatabase.OpenAsset(script);
                    return;
                }
            }
        }

        #endregion
    }
}