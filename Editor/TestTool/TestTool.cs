using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LLib.Editor
{
    public class TestTool : EditorWindow
    {
        private List<BaseTestToolModule> _modules;
        private BaseTestToolModule _selectedModule;
        private int _selectedIndex;
        private TestToolSettingsModule _settingsModule;
        private Vector2 _scrollPos;
        
        
        private TestToolSettings Settings => TestToolSettings.instance;
        
        
        private TestToolSettingsModule SettingsModule
        {
            get
            {
                if (_settingsModule == null)
                {
                    _settingsModule = CreateInstance<TestToolSettingsModule>();
                    _settingsModule.Title = "Settings";
                    _settingsModule.IsRunTimeOnly = false;
                }   
                
                return _settingsModule;
            }
        }



        [MenuItem("Window/[ LLib ]/Test Tool", false, int.MinValue)]
        public static void Open()
        {
            var window = GetWindow<TestTool>();
            window.titleContent = new GUIContent("Test Tool");
            window.Show();
        }

        
        private void OnEnable()
        {
            SettingsModule.Init();
            
            Settings.InitStyles();
           
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

            _modules = Settings.Modules;
            
            DrawTitle();
            DrawUnderline(GUILayoutUtility.GetLastRect());
            
            Rect mainRect = EditorGUILayout.BeginVertical(Settings.MainRectStyle);
            {
                EditorGUI.DrawRect(mainRect, Settings.BottomBackgroundColor);
                DrawGrid(mainRect);
             
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    if (_selectedModule == null)
                    {
                        EditorGUILayout.Space(2);
                        DrawCategoryButtons();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical(Settings.CategorySideRectStyle, GUILayout.Width(Settings.CategoryRectWidth), GUILayout.ExpandHeight(true));
                            {
                                EditorGUILayout.Space(2);
                                DrawCategoryButtons();
                            
                                EditorGUILayout.EndVertical();
                            }

                            Rect contentsRect = EditorGUILayout.BeginVertical(Settings.ContentRectStyle, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                            {
                                EditorGUI.DrawRect(contentsRect, Settings.ContentsBackgroundColor);

                                DrawContents(contentsRect);
                                EditorGUILayout.Space(20);
                                EditorGUILayout.EndVertical();
                            }
                        
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    
                    EditorGUILayout.EndScrollView();
                }
                   
                EditorGUILayout.EndVertical();
            }
            
         
        }

        #region TOP

        private void DrawTitle()
        {
            EditorGUILayout.Space(20f);

            GUIContent content = EditorGUIUtility.TrTextContent("TEST TOOL");
            Rect rect = GUILayoutUtility.GetRect(content, Settings.TitleStyle);

            
            GUI.Label(new Rect(rect.x + 2.5f, rect.y + 2.5f, rect.width, rect.height), content, Settings.TitleShadowStyle);
            GUI.Label(rect, content, Settings.TitleStyle);

            EditorGUILayout.Space(20f);
        }
        
        private void DrawUnderline(Rect rect)
        {
            float lineY = rect.yMax - 2.5f;
            
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
            for (int i = 0; i <= _modules.Count; i++)
            {
                BaseTestToolModule target = (i == _modules.Count) ? SettingsModule : _modules[i];
                if (target == null) 
                    continue;

                var label = target.Title.ToUpper();

                if (_selectedModule == target && _selectedIndex == i)
                {
                    Rect btnRect = GUILayoutUtility.GetRect(new GUIContent(label), Settings.BtnSelectedStyle);

                    EditorGUI.DrawRect(btnRect, Settings.ContentsBackgroundColor);
                
                    GUI.Label(btnRect, label, Settings.BtnSelectedStyle);

                    var outlineColor = Settings.ButtonHighlightColor;
                    float thickness = 2f;

                    EditorGUI.DrawRect(new Rect(btnRect.x, btnRect.y, btnRect.width, thickness), outlineColor);
                    EditorGUI.DrawRect(new Rect(btnRect.x, btnRect.yMax - thickness, btnRect.width, thickness), outlineColor);
                    EditorGUI.DrawRect(new Rect(btnRect.x, btnRect.y, thickness, btnRect.height), outlineColor);
                    EditorGUI.DrawRect(new Rect(btnRect.xMax - thickness, btnRect.y, thickness, btnRect.height), outlineColor);

                    if (GUI.Button(btnRect, "", GUIStyle.none))
                    {
                        _selectedModule = null;
                    }
                }
                else
                {
                    GUI.backgroundColor = Settings.ButtonNormalColor;
            
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
            Color gridColor = new Color(1, 1, 1, 0.03f); 

            for (float i = rect.y; i < rect.yMax; i += 15)
            {
                EditorGUI.DrawRect(new Rect(rect.x, i, rect.width, 1f), gridColor);
            }

            for (float i = rect.x; i < rect.xMax; i += 15)
            {
                EditorGUI.DrawRect(new Rect(i, rect.y, 1f, rect.height), gridColor);
            }
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
                    string message = "⚠️ RUNTIME ONLY";
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
            Event current = Event.current;
        
            if (current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
            
                menu.AddItem(new GUIContent($"{module.GetType().Name} Script"), false, () => OpenModuleScript(module));
            
                menu.ShowAsContext();
            
                current.Use();
            }
        }
        
        
        private void OpenModuleScript(BaseTestToolModule module)
        {
            var type = module.GetType();
            
            string[] guids = AssetDatabase.FindAssets($"{type.Name} t:MonoScript");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

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

