using System;
using UnityEngine;

namespace LLib
{
    public class StatResource
    {
        private readonly Stat _refStat;


        public StatResource(Stat refStat)
        {
            ID = refStat.ID;

            _refStat = refStat;
            _refStat.OnValueChanged += OnRefStatChanged;

            Current = refStat.Value;
        }

        public int ID { get; }

        public float Current { get; private set; }

        public float Max => _refStat.Value;
        public float Ratio => Mathf.Clamp01(Current / Max);

        public event Action<float, float> OnValueChanged;
        public event Action OnEmpty;


        public void Dispose()
        {
            if (_refStat != null) _refStat.OnValueChanged -= OnRefStatChanged;
        }


        public void Apply(float amount)
        {
            if (amount == 0)
                return;

            var previous = Current;
            Current = Mathf.Clamp(Current + amount, 0, Max);

            if (!Mathf.Approximately(previous, Current))
            {
                OnValueChanged?.Invoke(Current, Max);

                if (Current <= 0) OnEmpty?.Invoke();
            }
        }


        public void SetEmpty()
        {
            Current = 0;

            OnValueChanged?.Invoke(Current, Max);
            OnEmpty?.Invoke();
        }


        public void SetFull()
        {
            Current = Max;

            OnValueChanged?.Invoke(Current, Max);
        }


        protected virtual void OnRefStatChanged(float max)
        {
            Current = Math.Min(Current, max);

            OnValueChanged?.Invoke(Current, max);
        }
    }
}