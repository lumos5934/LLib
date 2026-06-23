using System.Collections.Generic;
using UnityEngine;

namespace LLib
{
    public class DataContainer<T> where T : BaseData
    {
        protected readonly Dictionary<int, T> _dataById;
        protected readonly Dictionary<string, T> _dataByName;
        protected readonly List<T> _dataList;


        public DataContainer(List<T> dataList)
        {
            _dataList = dataList;
            _dataByName = new Dictionary<string, T>();
            _dataById = new Dictionary<int, T>();

            foreach (var data in _dataList)
            {
                if (data.ID > 0)
                    if (!_dataById.TryAdd(data.ID, data))
                        Debug.LogError($"[DataContainer<{typeof(T).Name}>] {data.ID}");

                if (!string.IsNullOrWhiteSpace(data.Name))
                {
                    if (!_dataById.ContainsKey(data.ID))
                        continue;

                    if (!_dataByName.TryAdd(data.Name, data))
                        Debug.LogError($"[DataContainer<{typeof(T).Name}>] {data.ID}");
                }
            }
        }


        public T Get(string name)
        {
            return _dataByName.GetValueOrDefault(name);
        }


        public T Get(int id)
        {
            return _dataById.GetValueOrDefault(id);
        }


        public IEnumerable<T> GetAll()
        {
            return _dataList;
        }
    }
}