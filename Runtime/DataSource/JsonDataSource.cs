using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace LLib
{
    [CreateAssetMenu(fileName = "Json_DataSource", menuName = "[ LLib ]/Scriptable Objects/Data Source/Json")]
    public class JsonDataSource : BaseDataSource
    {
        [SerializeField] private string _fileName;
        [ShowInInspector] private string FolderPath => Application.persistentDataPath;

        private JsonSerializerSettings _settings;

        private void OnEnable()
        {
            _settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }



        public override async UniTask WriteAsync<T>(T data)
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            
            JObject root = await LoadRootAsync();

            root[data.GetType().Name] = JToken.FromObject(data, JsonSerializer.Create(_settings));

            string json = root.ToString();

            await File.WriteAllTextAsync(GetPath(), json);
        }

        public override async UniTask<T> ReadAsync<T>()
        {
            if (!File.Exists(GetPath()))  
                return await UniTask.FromResult<T>(default);
            
            JObject root = await LoadRootAsync();
            
            string key = typeof(T).FullName;

            if (!root.TryGetValue(key, out JToken token)) 
                return await UniTask.FromResult<T>(default);
            
            return await UniTask.FromResult(token.ToObject<T>());
        }
        
        private async UniTask<JObject> LoadRootAsync()
        {
            var path = GetPath();
        
            if (!File.Exists(path))
                return new JObject();
        
            string json = await File.ReadAllTextAsync(path);
        
            if (string.IsNullOrEmpty(json))
                return new JObject();
        
            return JObject.Parse(json);
        }

        private string GetPath()
        {
            var path = Path.Combine(FolderPath, _fileName);

            if (!path.EndsWith(".json"))
            {
                path += ".json";
            }

            return path;
        }
        
#if UNITY_EDITOR
        [Button("Open")]
        public void OpenFolder()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
#endif
    }
}