using UnityEngine;

namespace LLib
{
    public class TutorialManager : SingletonGlobal<TutorialManager>, ITutorialManager
    {
        private int _curStep;
        private BaseTutorial _curTutorial;
        private TutorialTable _curTutorialTable;



        private void Update()
        {
            if (_curTutorial != null)
            {
                _curTutorial.Update();

                if (_curTutorial.IsComplete()) ChangeNextStep();
            }
        }


        public TutorialTable GetTable()
        {
            return _curTutorialTable;
        }

        public BaseTutorial GetTutorial()
        {
            return _curTutorial;
        }

        public void Play(TutorialTable table)
        {
            _curStep = 0;
            _curTutorialTable = table;

            _curTutorial = _curTutorialTable.CreateTutorial(_curStep);
            _curTutorial.Enter();
        }

        private void ChangeNextStep()
        {
            _curStep++;

            _curTutorial.Exit();
            _curTutorial = _curStep < _curTutorialTable.GetAssetCount()
                ? _curTutorialTable.CreateTutorial(_curStep)
                : null;
            _curTutorial?.Enter();
        }
    }
}