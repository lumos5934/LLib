using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LLib.Editor
{
    [FilePath("ProjectSettings/TestToolSetting.asset", FilePathAttribute.Location.ProjectFolder)]
    public class TestToolSettings : ScriptableSingleton<TestToolSettings>
    {
        internal int TitleFontSize = 25;
        internal Color TitleFontColor = new (1f, 0.9f, 0.9f);
        internal Color TitleFontShadowColor = new (0.15f, 0.1f, 0f, 0.8f);
        internal Color TitleUnderLineColor =  new (1, 1, 1, 0.15f);
        internal Color TitleUnderLineHighlightColor = new (1f, 0.85f, 0.4f, 0.6f);
        
        internal Color BottomBackgroundColor = new (0.2f, 0.2f, 0.2f, 1f);
        
        internal float CategoryRectWidth = 90;
        
        internal int ButtonFontSize = 12;
        internal float ButtonHeight = 25;
        
        internal Color ButtonNormalColor = new (1f, 0.80f, 0.6f, 0.6f);
        internal Color ButtonHighlightColor = new (1f, 0.85f, 0.4f, 0.9f);
        internal Color ButtonFontNormalColor = Color.white; 
        internal Color ButtonFontHoverColor = new (0.5f, 0.8f, 0.7f, 1);
        internal Color ContentsBackgroundColor = new (0.1f, 0.1f, 0.1f, 1f);
        
        internal List<BaseTestToolModule> Modules = new();



        #region GUIStyle


        public void InitStyles()
        {
            _titleStyle = null;
            _titleShadowStyle = null;
            _mainRectStyle = null;
            _categorySideRectStyle = null;
            _contentRectStyle = null;
            _btnStyle = null;
            _noticeStyle = null;
        }
        

        private GUIStyle _titleStyle;
        internal GUIStyle TitleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle(EditorStyles.boldLabel);
                }

                _titleStyle.fontStyle = FontStyle.Bold;
                _titleStyle.alignment = TextAnchor.MiddleCenter;
                _titleStyle.hover.background = null;
                _titleStyle.active.background = null;
                _titleStyle.fontSize = TitleFontSize;
                _titleStyle.normal.textColor = TitleFontColor;
                _titleStyle.hover.textColor = TitleFontColor;
                _titleStyle.active.textColor = TitleFontColor;
                _titleStyle.focused.textColor = TitleFontColor;
                
                return _titleStyle;
            }
        }
        
        private GUIStyle _titleShadowStyle;
        internal GUIStyle TitleShadowStyle
        {
            get
            {
                if (_titleShadowStyle == null)
                {
                    _titleShadowStyle = new GUIStyle(EditorStyles.boldLabel);
                }

                _titleShadowStyle.hover.background = null;
                _titleShadowStyle.active.background = null;
                _titleShadowStyle.fontStyle = FontStyle.Bold;
                _titleShadowStyle.alignment = TextAnchor.MiddleCenter;
                _titleShadowStyle.fontSize = TitleFontSize;
                _titleShadowStyle.normal.textColor = TitleFontShadowColor;
                _titleShadowStyle.hover.textColor = TitleFontShadowColor;
                _titleShadowStyle.active.textColor = TitleFontShadowColor;
                _titleShadowStyle.focused.textColor = TitleFontShadowColor;

                return _titleShadowStyle;
            }
        }

        private GUIStyle _mainRectStyle;
        internal GUIStyle MainRectStyle
        {
            get
            {
                if (_mainRectStyle == null)
                {
                    _mainRectStyle = new GUIStyle("box");
                }
                
                _mainRectStyle.padding = new RectOffset(0, 0, 0, 0);
                _mainRectStyle.margin = new RectOffset(0, 0, 0, 0);
                _mainRectStyle.stretchWidth = true;
                _mainRectStyle.stretchHeight = true;
            
                return _mainRectStyle;
            }
        }
        
        private GUIStyle _categorySideRectStyle;
        internal GUIStyle CategorySideRectStyle
        {
            get
            {
                if (_categorySideRectStyle == null)
                {
                    _categorySideRectStyle = new GUIStyle();
                }
                
                _categorySideRectStyle.padding = new RectOffset(1, 0, 0, 3);
                _categorySideRectStyle.margin = new RectOffset(0, 0, 0, 0);

                return _categorySideRectStyle;
            }
        }
        
        private GUIStyle _contentRectStyle;
        internal GUIStyle ContentRectStyle
        {
            get
            {
                if (_contentRectStyle == null)
                {
                    _contentRectStyle = new GUIStyle("box");
                }
                
                _contentRectStyle.margin = new RectOffset(0, 1, 1, 0);
              
                return _contentRectStyle;
            }
        }
        
        private GUIStyle _btnSelectedStyle;

        internal GUIStyle BtnSelectedStyle
        {
            get
            {
                if (_btnSelectedStyle == null)
                {
                    _btnSelectedStyle = new GUIStyle(); 
                }

                _btnSelectedStyle.fontStyle = FontStyle.Bold;
                _btnSelectedStyle.alignment = TextAnchor.MiddleCenter;
                _btnSelectedStyle.stretchWidth = true;
                _btnSelectedStyle.fixedHeight = ButtonHeight;
                _btnSelectedStyle.fontSize = ButtonFontSize;
        
                _btnSelectedStyle.normal.textColor = ButtonFontNormalColor;
                _btnSelectedStyle.hover.textColor = ButtonFontHoverColor;

                _btnSelectedStyle.margin = new RectOffset(0, 0, 0, 0);
                _btnSelectedStyle.padding = new RectOffset(0, 0, 0, 0);

                return _btnSelectedStyle;
            }
        }
        
        private GUIStyle _btnStyle;
        internal GUIStyle BtnStyle
        {
            get
            {
                if (_btnStyle == null)
                {
                    _btnStyle = new GUIStyle(EditorStyles.miniButton); 
                }

                _btnStyle.fontStyle = FontStyle.Bold;
                _btnStyle.alignment = TextAnchor.MiddleCenter;
                _btnStyle.stretchWidth = true;
                _btnStyle.fixedHeight = ButtonHeight;
                _btnStyle.fontSize = ButtonFontSize;
        
                _btnStyle.normal.textColor = ButtonFontNormalColor;
                _btnStyle.hover.textColor = ButtonFontHoverColor;

                _btnStyle.margin = new RectOffset(0, 0, 0, 0);
                _btnStyle.padding = new RectOffset(0, 0, 0, 0);

                return _btnStyle;
            }
        }
        
        private GUIStyle _noticeStyle;
        internal GUIStyle NoticeStyle
        {
            get
            {
                if (_noticeStyle == null)
                {
                    _noticeStyle = new GUIStyle();
                    
                    _noticeStyle.alignment = TextAnchor.MiddleCenter;
                    _noticeStyle.fontSize = 18;
                    _noticeStyle.fontStyle = FontStyle.Bold;
                    _noticeStyle.padding = new RectOffset(20, 20, 20, 20);
                    _noticeStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
                }
            
                return _noticeStyle;
            }
        }
        
        #endregion
    }
}