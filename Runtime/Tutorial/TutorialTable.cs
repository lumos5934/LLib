using UnityEngine;

namespace LLib
{
    [CreateAssetMenu(fileName = "TutorialTable", menuName = "[ LLib ]/Scriptable Objects/Tutorial Table")]
    public class TutorialTable : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private BaseTutorialAsset[] _assets;
    
        public int GetID()
        {
            return _id;
        }

        public BaseTutorial CreateTutorial(int index)
        {
            return _assets[index].Create();
        }

        public int GetAssetCount()
        {
            return _assets.Length;
        }
    }
}
