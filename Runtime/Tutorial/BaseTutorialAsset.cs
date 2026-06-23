using UnityEngine;

namespace LLib
{
    public abstract class BaseTutorialAsset : ScriptableObject
    {
        [field: SerializeField] public string Description { get; private set; } = "";

        public abstract BaseTutorial Create();
    }
}