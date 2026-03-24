using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LumosLib.Editor
{
    public class TestTool : EditorWindow
    {
        #region Fields

        private List<ITestToolElement> _elements;
        private ITestToolElement _selectedElement;
        private Vector2 _scrollPos;
        
        private GUIStyle _titleStyle;
        private GUIStyle TitleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle(EditorStyles.boldLabel);
                    _titleStyle.fontStyle = FontStyle.Bold;
                    _titleStyle.alignment = TextAnchor.MiddleCenter;
                    _titleStyle.hover.background = null;
                    _titleStyle.active.background = null;
                }

                return _titleStyle;
            }
        }
        
        private GUIStyle _titleShadowStyle;
        private GUIStyle TitleShadowStyle
        {
            get
            {
                if (_titleShadowStyle == null)
                {
                    _titleShadowStyle = new GUIStyle(EditorStyles.boldLabel);
                    _titleShadowStyle.fontSize = 25;
                    _titleShadowStyle.fontStyle = FontStyle.Bold;
                    _titleShadowStyle.alignment = TextAnchor.MiddleCenter;
                    _titleShadowStyle.hover.background = null;
                    _titleShadowStyle.active.background = null;
                }

                return _titleShadowStyle;
            }
        }
        
        private GUIStyle _mainRectStyle;
        private GUIStyle MainRectStyle
        {
            get
            {
                if (_mainRectStyle == null)
                {
                    _mainRectStyle = new GUIStyle("box")
                    {
                        padding = new RectOffset(0, 0, 0, 0),
                        margin = new RectOffset(0, 0, 0, 0),
                        stretchWidth = true,
                        stretchHeight = true,
                    };
                }
            
                return _mainRectStyle;
            }
        }
        private GUIStyle _categorySideRectStyle;
        private GUIStyle CategorySideRectStyle
        {
            get
            {
                if (_categorySideRectStyle == null)
                {
                    _categorySideRectStyle = new GUIStyle()
                    {
                        padding = new RectOffset(2, 0, 0, 3),
                        margin = new RectOffset(0, 0, 0, 0)
                    };
                }

                return _categorySideRectStyle;
            }
        }
        private GUIStyle _contentRectStyle;
        private GUIStyle ContentRectStyle
        {
            get
            {
                if (_contentRectStyle == null)
                {
                    _contentRectStyle = new GUIStyle("box")
                    {
                        margin = new RectOffset(0, 1, 1, 0),
                    };
                }
              
                return _contentRectStyle;
            }
        }

        private GUIStyle _btnStyle;
        private GUIStyle BtnStyle
        {
            get
            {
                if (_btnStyle == null)
                {
                    _btnStyle = new GUIStyle(EditorStyles.miniButtonMid)
                    {
                        margin = new RectOffset(0, 0, 0, 0),
                        padding = new RectOffset(0,0,0,0),
                        overflow = new RectOffset(0,0,0,0),
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                    };
                }

                return _btnStyle;
            }
        }
        private GUIStyle _noticeStyle;
        private GUIStyle NoticeStyle
        {
            get
            {
                if (_noticeStyle == null)
                {
                    _noticeStyle = new GUIStyle()
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 18,
                        fontStyle = FontStyle.Bold,
                        padding = new RectOffset(20, 20, 20, 20),
                    };
                    
                    _noticeStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
                }
            
                return _noticeStyle;
            }
        }

        private LumosLibSettings Settings => LumosLibSettings.Instance;

        #endregion

        
        [MenuItem("Window/[ Lumos Lib ]/Test Tool", false, int.MinValue)]
        public static void Open()
        {
            var window = GetWindow<TestTool>();
            window.titleContent = new GUIContent("Test Tool");
            window.Show();
        }

        
        private void OnEnable()
        {
            _elements = TypeCache
                .GetTypesDerivedFrom<ITestToolElement>()
                .Where(t => !t.IsAbstract)
                .Select(t => Activator.CreateInstance(t) as ITestToolElement)
                .OrderBy(t => t.Priority)
                .ToList();
            
            _elements.ForEach(i => i.OnEnable(this));
            
            
            _titleStyle = null;
            _titleShadowStyle = null;
            _mainRectStyle = null;
            _categorySideRectStyle = null;
            _contentRectStyle = null;
            _btnStyle = null;
            _noticeStyle = null;
            
            
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
            
            
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            {
                DrawTop();
            
                GUI.backgroundColor = Color.black;
                Rect mainRect = EditorGUILayout.BeginVertical(MainRectStyle);
                {
                    GUI.backgroundColor = Color.white;
                
                   if (_selectedElement == null)
                   {
                       DrawCategory(mainRect);
                   }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            var categorySideRect = EditorGUILayout.BeginVertical(CategorySideRectStyle, GUILayout.Width(Settings.CategorySideWidth), GUILayout.ExpandHeight(true));
                            {
                                DrawCategory(categorySideRect);
                            
                                EditorGUILayout.EndVertical();
                            }

                            GUI.backgroundColor = Settings.ContentsBackgroundColor;
                            Rect contentsRect = EditorGUILayout.BeginVertical(ContentRectStyle,
                                GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                            {
                                GUI.backgroundColor = Color.white;

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

        #region TOP

        private void DrawTop()
        {
            TitleStyle.fontSize = Settings.TitleFontSize;
            
            Color titleColor = Settings.TitleFontColor;
            TitleStyle.normal.textColor = titleColor;
            TitleStyle.hover.textColor = titleColor;
            TitleStyle.active.textColor = titleColor;
            TitleStyle.focused.textColor = titleColor;
            
            
            TitleShadowStyle.fontSize = Settings.TitleFontSize;
            
            Color titleShadowColor = Settings.TitleFontShadowColor;
            TitleShadowStyle.normal.textColor = titleShadowColor;
            TitleShadowStyle.hover.textColor = titleShadowColor;
            TitleShadowStyle.active.textColor = titleShadowColor;
            TitleShadowStyle.focused.textColor = titleShadowColor;
            
            DrawTitle();
            DrawUnderline(GUILayoutUtility.GetLastRect());
        }
        

        private void DrawTitle()
        {
            EditorGUILayout.Space(20f);

            GUIContent content = EditorGUIUtility.TrTextContent("TEST TOOL");
            Rect rect = GUILayoutUtility.GetRect(content, TitleStyle);

            var shadowStyle = TitleShadowStyle;
            GUI.Label(new Rect(rect.x + 2.5f, rect.y + 2.5f, rect.width, rect.height), content, shadowStyle);
            GUI.Label(rect, content, TitleStyle);

            EditorGUILayout.Space(20f);
        }
        
        
        private void DrawUnderline(Rect rect)
        {
            float lineY = rect.yMax - 1.5f;
            
            var lineColor = Settings.TitleUnderLineColor;
            var lineColorShadowColor = lineColor * 0.4f;
            lineColor.a = lineColor.a;
            
            EditorGUI.DrawRect(new Rect(rect.x, lineY - 1f, rect.width, 1), lineColorShadowColor);
            EditorGUI.DrawRect(new Rect(rect.x, lineY, rect.width, 2), lineColor);

            var highLightColor = Settings.TitleUnderLineHighlightColor;
            var highLightShadowColor = highLightColor * 0.4f;
            highLightShadowColor.a = highLightColor.a;
          
            EditorGUI.DrawRect(new Rect(rect.center.x - 20, lineY - 1f, 40, 1), highLightShadowColor);
            EditorGUI.DrawRect(new Rect(rect.center.x - 20, lineY, 40, 2), Settings.TitleUnderLineHighlightColor);
        }

        #endregion

        #region CATEGORY

        private void DrawCategory(Rect mainRect)
        {
            DrawGrid(mainRect);
            EditorGUILayout.Space(2);
            DrawCategoryButtons();
        }
       
        private void DrawCategoryButtons()
        {
            BtnStyle.normal.textColor = Settings.ButtonFontNormalColor;
            BtnStyle.hover.textColor = Settings.ButtonFontHoverColor;
            BtnStyle.fixedHeight = Settings.ButtonHeight;
            BtnStyle.fontSize = Settings.ButtonFontSize;
            
            for (int i = 0; i < _elements.Count; i++)
            {
                var target = _elements[i];

                GUI.backgroundColor = (_selectedElement == target)
                    ? Settings.ButtonSelectedColor
                    : Settings.ButtonNormalColor;

                var label = target.Title.ToUpper();
                
                if (GUILayout.Button(label, BtnStyle))
                {
                    _selectedElement = _selectedElement == target ? null : target;
                }
                
                Rect rect = GUILayoutUtility.GetLastRect();
                
                if (_selectedElement == target)
                {
                    rect.width -= 2;
                    rect.height -= 2;
                    rect.center -= new Vector2(0f, -1f);

                    var outlineColor = Settings.ButtonHighlightColor;
                    
                    Handles.color = outlineColor;
                    // 테두리만 컬러
                    Handles.DrawSolidRectangleWithOutline(rect, 
                        new Color(0, 0, 0, 0), 
                        outlineColor);
                }
            }
            
            GUI.backgroundColor = Color.white;
        }
        
        private void DrawGrid(Rect rect)
        {
            Color gridColor = new Color(1, 1, 1, 0.03f); 
            Handles.BeginGUI();
    
            Handles.color = gridColor;
            
            for (float i = rect.y; i < rect.yMax; i += 15)
            {
                Handles.DrawLine(new Vector2(rect.x, i), new Vector2(rect.xMax, i));
            }

            for (float i = rect.x; i < rect.xMax; i += 15)
            {
                Handles.DrawLine(new Vector2(i, rect.y), new Vector2(i, rect.yMax));
            }

            Handles.EndGUI();
        }
        
        #endregion

        #region Contents

        private void DrawContents(Rect contentsRect)
        {
            if (_selectedElement != null)
            {
                if (_selectedElement.IsRunTimeOnly && 
                    !Application.isPlaying)
                {
                    GUILayout.FlexibleSpace();
                    string message = "⚠️ RUNTIME ONLY";
                    GUILayout.Label(message, NoticeStyle);
                    GUILayout.FlexibleSpace();
                }
                else
                {
                    _selectedElement.OnGUI();
                }
            }
            
            HandleElementMenu(contentsRect, _selectedElement);
        }

        private void HandleElementMenu(Rect rect, ITestToolElement element)
        {
            Event current = Event.current;
        
            if (current.type == EventType.ContextClick && rect.Contains(current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
            
                menu.AddItem(new GUIContent($"{element.GetType().Name} Script"), false, () => OpenElementScript(element));
            
                menu.ShowAsContext();
            
                current.Use();
            }
        }
        
        
        private void OpenElementScript(ITestToolElement element)
        {
            var type = element.GetType();
            
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

