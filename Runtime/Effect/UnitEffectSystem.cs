using System;
using System.Collections.Generic;
using UnityEngine;

namespace LLib
{
    public static class UnitEffectSystem
    {
        private static readonly List<IUnitEffectModifier> _modifiers = new();
        private static readonly Stack<UnitEffectContext> _contextPool = new();


        public static void Apply(Action<UnitEffectContext> onSetup)
        {
            var context = _contextPool.Count > 0 ? _contextPool.Pop() : new UnitEffectContext();

            try
            {
                onSetup?.Invoke(context);

                if (context.Target == null)
                {
                    Debug.LogWarning($"{context.GetType().Name} : Missing Target");
                    return;
                }

                if (context.Source == null)
                {
                    Debug.LogWarning($"{context.GetType().Name} : Missing Source");
                    return;
                }

                if (context.Effects == null ||
                    context.Effects.Count == 0)
                {
                    Debug.LogWarning($"{context.GetType().Name} : Missing Effects");
                    return;
                }


                foreach (var modifier in _modifiers) modifier.Modify(context);


                var targetStats = context.Target.Stats;
                var targetResources = context.Target.Resources;

                foreach (var effect in context.Effects)
                {
                    if (effect.TargetResourceID > 0)
                    {
                        var resource = targetResources.Get(effect.TargetResourceID);
                        resource?.Apply(effect.FinalValue);
                    }

                    if (effect.TargetStatID > 0)
                    {
                        var stat = targetStats.Get(effect.TargetStatID);
                        stat?.SetBaseValue(effect.FinalValue);
                    }
                }

                context.Target.OnApplyEffect(context);
            }
            finally
            {
                context.Reset();
                _contextPool.Push(context);
            }
        }


        public static void AddModifier(IUnitEffectModifier modifier)
        {
            _modifiers.Add(modifier);
            _modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }
}