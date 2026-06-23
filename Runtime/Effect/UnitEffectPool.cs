using System.Collections.Generic;

namespace LLib
{
    internal static class UnitEffectPool
    {
        private static readonly Stack<UnitEffect> _pool = new();

        public static UnitEffect Get()
        {
            return _pool.Count > 0 ? _pool.Pop() : new UnitEffect();
        }

        public static void Return(UnitEffect effect)
        {
            if (effect == null)
                return;

            effect.Reset();
            _pool.Push(effect);
        }
    }
}