using System;
using System.Collections.Generic;

namespace LLib
{
    public class StateHandler
    {
        private readonly Dictionary<int, int> _stateCounts = new(); // ID, Count


        public int GetCount(int id)
        {
            return _stateCounts.GetValueOrDefault(id, 0);
        }


        public void SetCount(int id, int count)
        {
            _stateCounts[id] = count;
        }


        public void AddStack(int id)
        {
            SetCount(id, GetCount(id) + 1);
        }


        public void RemoveStack(int id)
        {
            SetCount(id, Math.Max(0, GetCount(id) - 1));
        }
    }
}