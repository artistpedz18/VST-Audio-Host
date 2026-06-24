using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace VST_Audio_Host.Core
{
    /// <summary>
    /// Central audio engine managing playback, recording, and plugin processing
    /// </summary>
    public class AudioEngine : IDisposable
    {
        private IWavePlayer _wavePlayer;
        private WaveFormat _waveFormat;
        private List<PluginInstance> _plugins;
        private bool _isRunning;
        private float _masterVolume = 1.0f;

        public event EventHandler<AudioLevelEventArgs> AudioLevelChanged;

        public AudioEngine(int sampleRate = 44100, int channels = 2, int bitDepth = 16)
        {
            _waveFormat = new WaveFormat(sampleRate, bitDepth, channels);
            _plugins = new List<PluginInstance>();
            _isRunning = false;
        }

        public void Initialize()
        {
            _wavePlayer = new WaveOutEvent();
            _isRunning = true;
        }

        public void AddPlugin(PluginInstance plugin)
        {
            if (!_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
            }
        }

        public void RemovePlugin(PluginInstance plugin)
        {
            _plugins.Remove(plugin);
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Math.Clamp(volume, 0f, 1f);
        }

        public float GetMasterVolume() => _masterVolume;

        public List<PluginInstance> GetActivePlugins() => new List<PluginInstance>(_plugins);

        public int GetSampleRate() => _waveFormat.SampleRate;

        public int GetChannels() => _waveFormat.Channels;

        public void Stop()
        {
            _isRunning = false;
            _wavePlayer?.Stop();
        }

        public void Dispose()
        {
            Stop();
            _wavePlayer?.Dispose();
            foreach (var plugin in _plugins)
            {
                plugin?.Dispose();
            }
            _plugins.Clear();
        }
    }

    public class AudioLevelEventArgs : EventArgs
    {
        public float LeftLevel { get; set; }
        public float RightLevel { get; set; }
    }
}
