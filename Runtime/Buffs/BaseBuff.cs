using System.Collections.Generic;

namespace LLib
{
    public abstract class BaseBuff
    {
        private float _tickTimer;
        public float Duration;
        public List<UnitEffect> Effects;
        public int ID;
        public float Interval;
        public string Name;

        public IUnit Target { get; private set; }
        public float Timer { get; protected set; }
        public int StackCount { get; private set; } = 1;


        public void Apply(IUnit target)
        {
            Reset();

            Target = target;

            OnApply();
        }


        public void Remove()
        {
            Reset();

            OnRemove();
        }


        public void Update(float deltaTime)
        {
            Timer -= deltaTime;
            _tickTimer += deltaTime;

            if (_tickTimer >= Interval)
            {
                OnTick();
                _tickTimer -= Interval;
            }
        }

        public void AddStack()
        {
            StackCount++;
            OnStack();
        }


        protected abstract void OnApply();
        protected abstract void OnRemove();
        protected abstract void OnTick();
        protected abstract void OnStack();


        private void Reset()
        {
            Target = null;
            Timer = Duration;
            _tickTimer = 0;
            StackCount = 1;
        }
    }
}