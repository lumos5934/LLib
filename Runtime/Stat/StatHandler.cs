using System.Collections.Generic;

namespace LLib
{
    public class StatHandler
    {
        private readonly List<Stat> _stats;
        private readonly Dictionary<int, Stat> _statsById;

        public StatHandler()
        {
            _statsById = new Dictionary<int, Stat>();
            _stats = new List<Stat>();
        }


        public Stat Get(int id)
        {
            if (!_statsById.TryGetValue(id, out var stat))
            {
                stat = new Stat(id);
                _statsById.Add(id, stat);
                _stats.Add(stat);
            }

            return stat;
        }


        public IEnumerable<Stat> GetAll()
        {
            return _stats;
        }


        public float GetValue(int id)
        {
            var stat = Get(id);

            return stat.Value;
        }


        public float GetBaseValue(int id)
        {
            var stat = Get(id);

            return stat.BaseValue;
        }


        public IReadOnlyList<StatModifier> GetModifiers(int id)
        {
            var stat = Get(id);

            return stat?.Modifiers;
        }


        public void SetBaseValue(int id, float value)
        {
            var stat = Get(id);

            stat?.SetBaseValue(value);
        }


        public void AddModifier(int id, StatModifier modifier)
        {
            var stat = Get(id);

            stat?.AddModifier(modifier);
        }


        public void RemoveModifier(int id, StatModifier modifier)
        {
            var stat = Get(id);

            stat?.RemoveModifier(modifier);
        }


        public void RemoveAllFromSource(object source)
        {
            foreach (var stat in _statsById.Values) stat.RemoveAllFromSource(source);
        }
    }
}