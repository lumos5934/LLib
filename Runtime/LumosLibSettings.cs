using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TriInspector;

[assembly: InternalsVisibleTo("LumosLib.Editor")]
namespace LumosLib
{
    [CreateAssetMenu(fileName = "LumosLibSettings", menuName = "[ LumosLib ]/Scriptable Objects/Settings", order = int.MinValue)]
    public class LumosLibSettings : ScriptableObject
    {
        public static LumosLibSettings Instance { get; set; }
        
        [Title("Preload")]
        [SerializeField] private bool _usePreInitialize;

        [PropertySpace(10f)] 
        [SerializeField] private List<GameObject> _preloadObjects;

        public bool UsePreInitialize => _usePreInitialize;
        public List<GameObject> PreloadObjects => _preloadObjects;


        /// <summary>
        /// TetTool Settings
        /// </summary>
        internal int TitleFontSize = 25;
        internal Color TitleFontColor = new (1f, 0.9f, 0.9f);
        internal Color TitleFontShadowColor = new (0.15f, 0.1f, 0f, 0.8f);
        internal Color TitleUnderLineColor =  new (1, 1, 1, 0.15f);
        internal Color TitleUnderLineHighlightColor = new (1f, 0.85f, 0.4f, 0.6f);
        
        internal int ButtonFontSize = 12;
        internal float ButtonHeight = 25;
        internal Color ButtonNormalColor = new (1f, 0.80f, 0.6f, 0.6f);
        internal Color ButtonSelectedColor = new (0.40f, 0.40f, 0.40f, 1f);
        internal Color ButtonHighlightColor = new (1f, 0.85f, 0.4f, 0.9f);
        internal Color ButtonFontNormalColor = Color.white; 
        internal Color ButtonFontHoverColor = new (0.5f, 0.8f, 0.7f, 1);
        
        internal float CategorySideWidth = 90;
        internal Color ContentsBackgroundColor = new (0f, 0f, 0f, 0.7f);
    }
}
