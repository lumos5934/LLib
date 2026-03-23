using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace LumosLib
{
    [DeclareBoxGroup("Resources")]
    public class AudioManager : MonoBehaviour, IAudioManager, IPreInitializable
    {
        [Title("REQUIREMENT"), PropertyOrder(-1)]
        [ShowInInspector, HideReferencePicker, ReadOnly, LabelText("IPoolManager")] private IPoolManager _poolManager;
        [PropertySpace(20f)]
        
        [Title("SETTINGS")]
        [SerializeField] private AudioPlayer _audioPlayerPrefab;
        [SerializeField] private AudioMixer _mixer;
        
        [Group("Resources"), SerializeField, ReadOnly] private List<SoundAsset> _soundAssets = new();
        
        private readonly Dictionary<string, SoundAsset> _assetResources = new();
        private readonly Dictionary<int, AudioPlayer> _bgmPlayers = new();
        private readonly HashSet<AudioPlayer> _activePlayers = new();


        private void Awake()
        {
            Services.Register<IAudioManager>(this);
        }

        
        public UniTask<bool> InitAsync(PreInitContext ctx)
        {
            _poolManager = Services.Get<IPoolManager>();
            if (_poolManager == null)
            {
                return UniTask.FromResult(false);
            }
            
            foreach (var asset in _soundAssets)
            {
                _assetResources[asset.name] = asset;
            }
            
            return UniTask.FromResult(true);
        }
        
        
        public void SetVolume(string groupName, float volume)
        {
            _mixer.SetFloat(groupName, Mathf.Log10(volume) * 20f);
        }


        public void PlayBGM(int bgmType, string assetName)
        {
            if (_assetResources.TryGetValue(assetName, out SoundAsset asset))
            {
                var bgmPlayer = GetBGMPlayer(bgmType) ?? GetNewAudioPlayer();

                _bgmPlayers.TryAdd(bgmType, bgmPlayer);
            
                bgmPlayer.Play(asset, _ =>
                {
                    OnStopBGM(bgmType, bgmPlayer);
                });
            }
        }

        public void PlaySFX(string assetName)
        {
            if (_assetResources.TryGetValue(assetName, out SoundAsset asset))
            {
                GetNewAudioPlayer().Play(asset, OnStopSFX);
            }
        }
        

        public void StopBGM(int bgmType)
        {
            GetBGMPlayer(bgmType)?.Stop();
        }
        
        public void StopSFXAll()
        {
            var temp = new List<AudioPlayer>(_activePlayers);
            
            foreach (var player in temp)
            {
                if (!_bgmPlayers.ContainsValue(player))
                {
                    player.Stop();
                }
            }
        }
        
        public void StopAll()
        {
            var temp = new List<AudioPlayer>(_activePlayers);
            
            foreach (var player in temp)
            {
                player.Stop();
            }
        }

        public void PauseBGM(int bgmType, bool enable)
        {
            GetBGMPlayer(bgmType)?.Pause(enable);
        }
        
        public void PauseSFXAll(bool enable)
        {
            var temp = new List<AudioPlayer>(_activePlayers);
            
            foreach (var player in temp)
            {
                if (!_bgmPlayers.ContainsValue(player))
                {
                    player.Pause(enable);
                }
            }
        }

        public void PauseAll(bool enable)
        {
            var temp = new List<AudioPlayer>(_activePlayers);
            
            foreach (var player in temp)
            {
                player.Pause(enable);
            }
        }
        
        private void OnStopBGM(int bgmType, AudioPlayer player)
        {
            _poolManager.Release(player);

            _bgmPlayers.Remove(bgmType);
            
            if (_activePlayers.Contains(player))
            {
                _activePlayers.Remove(player);
            }
        }
        
        private void OnStopSFX(AudioPlayer player)
        {
            _poolManager.Release(player);
            
            if (_activePlayers.Contains(player))
            {
                _activePlayers.Remove(player);
            }
        }

                
        
        private AudioPlayer GetBGMPlayer(int bgmType)
        {
            return _bgmPlayers.GetValueOrDefault(bgmType);
        }

       
        private AudioPlayer GetNewAudioPlayer()
        {
            var player = _poolManager.Get(_audioPlayerPrefab);
            
            _activePlayers.Add(player);
            
            return player;
        }

       
    }
}