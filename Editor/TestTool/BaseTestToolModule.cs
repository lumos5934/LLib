using TriInspector;
using UnityEngine;

public abstract class BaseTestToolModule : ScriptableObject
{
    public string Title = "";
    public bool IsRunTimeOnly;

    public bool UseCustomButtonColor;
    [ShowIf(nameof(UseCustomButtonColor))] public Color ButtonColor;

    public abstract void Init();
    public abstract void OnGUI();
}