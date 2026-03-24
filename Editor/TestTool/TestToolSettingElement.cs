using LumosLib;
using LumosLib.Editor;
using UnityEditor;


public class TestToolSettingElement : ITestToolElement
{
    public string Title => "Settings";
    public int Priority => int.MaxValue;
    public bool IsRunTimeOnly => false;

    private LumosLibSettings Settings => LumosLibSettings.Instance;
    
    
    public void OnEnable(TestTool testTool)
    {
    }

    public void OnGUI()
    {
        EditorGUILayout.LabelField("Top", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            DrawTop();
            
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Category", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            DrawCategory();
            
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.Space(20f);
        EditorGUILayout.LabelField("Contents", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {
            DrawContents();
            
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawTop()
    {
        Settings.TitleFontSize = EditorGUILayout.IntField("Font Size", Settings.TitleFontSize);
        Settings.TitleFontColor = EditorGUILayout.ColorField("Font Color", Settings.TitleFontColor);
        Settings.TitleFontShadowColor = EditorGUILayout.ColorField("Font Shadow Color", Settings.TitleFontShadowColor);
        Settings.TitleUnderLineColor = EditorGUILayout.ColorField("UnderLine Color", Settings.TitleUnderLineColor);
        Settings.TitleUnderLineHighlightColor = EditorGUILayout.ColorField("UnderLine Highlight Color", Settings.TitleUnderLineHighlightColor);
    }

    private void DrawCategory()
    {
        Settings.ButtonFontSize = EditorGUILayout.IntField("Font Size", Settings.ButtonFontSize);
        Settings.ButtonFontNormalColor = EditorGUILayout.ColorField("Font Normal Color", Settings.ButtonFontNormalColor);
        Settings.ButtonFontHoverColor = EditorGUILayout.ColorField("Font Hover Color", Settings.ButtonFontHoverColor);
        EditorGUILayout.Space(5f);
        Settings.ButtonHeight = EditorGUILayout.FloatField("Button Height", Settings.ButtonHeight);
        Settings.ButtonNormalColor = EditorGUILayout.ColorField("Button Normal Color", Settings.ButtonNormalColor);
        Settings.ButtonSelectedColor = EditorGUILayout.ColorField("Button Selected Color", Settings.ButtonSelectedColor);
        Settings.ButtonHighlightColor = EditorGUILayout.ColorField("Button Highlight Color", Settings.ButtonHighlightColor);
        EditorGUILayout.Space(5f);
        Settings.CategorySideWidth = EditorGUILayout.FloatField("Side Width", Settings.CategorySideWidth);
    }
    
    
    private void DrawContents()
    {
        Settings.ContentsBackgroundColor = EditorGUILayout.ColorField("Background Color", Settings.ContentsBackgroundColor);
    }
}