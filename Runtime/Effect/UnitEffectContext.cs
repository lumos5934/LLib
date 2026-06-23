using System;
using System.Collections.Generic;
using UnityEngine;

namespace LLib
{
    public sealed class UnitEffectContext
    {
        private readonly Dictionary<Type, object> _customData = new();


        private readonly List<UnitEffect> _effects = new();
        public Vector3 HitDirection;
        public float HitDistance;

        public int HitFlags;
        public Vector3 HitPosition;
        public IUnit Source;
        public IUnit Target;


        internal UnitEffectContext()
        {
        }

        public IReadOnlyList<UnitEffect> Effects => _effects;


        public void SetEffects(IEnumerable<UnitEffect> effects)
        {
            ClearEffects();

            foreach (var effect in effects)
            {
                var pooledEffect = UnitEffectPool.Get();
                pooledEffect.Copy(effect);
                _effects.Add(pooledEffect);
            }
        }


        private void ClearEffects()
        {
            foreach (var effect in _effects) UnitEffectPool.Return(effect);
            _effects.Clear();
        }


        public void Reset()
        {
            Source = null;
            Target = null;
            HitFlags = 0;
            HitPosition = default;
            HitDirection = default;
            HitDistance = 0f;
            _customData.Clear();
            ClearEffects();
        }


        public void SetData<T>(T data)
        {
            _customData[typeof(T)] = data;
        }

        public T GetData<T>()
        {
            return _customData.TryGetValue(typeof(T), out var v) ? (T)v : default;
        }

        public bool HasData<T>()
        {
            return _customData.ContainsKey(typeof(T));
        }
    }
}