using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LLib
{
    public static class DrawUtil
    {
        public static T GetWeighted<T>(IEnumerable<T> targets) where T : IDrawable
        {
            if (targets == null)
                return default;

            var list = targets.ToList();
            if (list.Count == 0)
                return default;

            if (list.Count == 1)
                return list[0];

            var totalWeight = list.Sum(x => x.DrawWeight);

            if (totalWeight <= 0f)
                return list[0];

            var roll = Random.Range(0f, totalWeight);
            var cumulative = 0f;

            foreach (var item in list)
            {
                cumulative += item.DrawWeight;

                if (roll < cumulative)
                    return item;
            }

            return list[^1];
        }
    }
}