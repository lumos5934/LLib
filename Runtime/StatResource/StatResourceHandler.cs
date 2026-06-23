using System.Collections.Generic;

namespace LLib
{
    public class StatResourceHandler
    {
        private readonly Dictionary<int, StatResource> _resources;

        public StatResourceHandler()
        {
            _resources = new Dictionary<int, StatResource>();
        }


        public void Register(StatResource resource)
        {
            _resources.TryAdd(resource.ID, resource);
        }


        public void Unregister(StatResource resource)
        {
            Unregister(resource.ID);
        }


        public void Unregister(int id)
        {
            var contains = Get(id);
            if (contains != null)
            {
                contains.Dispose();
                _resources.Remove(id);
            }
        }


        public StatResource Get(int id)
        {
            return _resources.GetValueOrDefault(id);
        }


        public float GetCurrent(int id)
        {
            var target = Get(id);
            if (target == null)
                return 0;

            return target.Current;
        }


        public float GetMax(int id)
        {
            var target = Get(id);
            if (target == null)
                return 0;

            return target.Max;
        }


        public float GetRatio(int id)
        {
            var target = Get(id);
            if (target == null)
                return 0;

            return target.Ratio;
        }


        public void Apply(int id, float amount)
        {
            var resource = Get(id);
            resource?.Apply(amount);
        }


        public void SetEmpty(int id)
        {
            var resource = Get(id);
            resource?.SetEmpty();
        }


        public void SetFull(int id)
        {
            var resource = Get(id);
            resource?.SetFull();
        }
    }
}