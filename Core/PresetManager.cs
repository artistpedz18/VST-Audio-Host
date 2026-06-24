using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace VST_Audio_Host.Core
{
    /// <summary>
    /// Manages plugin presets and configurations
    /// </summary>
    public class PresetManager
    {
        private string _presetsDirectory;
        private List<Preset> _presets;

        public PresetManager(string presetsDirectory = null)
        {
            _presetsDirectory = presetsDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "VST-Audio-Host", "Presets");

            _presets = new List<Preset>();
            EnsureDirectoryExists();
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_presetsDirectory))
            {
                Directory.CreateDirectory(_presetsDirectory);
            }
        }

        public void SavePreset(string name, PluginInstance plugin)
        {
            var preset = new Preset
            {
                Name = name,
                PluginName = plugin.Name,
                PluginPath = plugin.PluginPath,
                Parameters = new Dictionary<int, float>(plugin.Parameters.Length),
                CreatedAt = DateTime.Now
            };

            for (int i = 0; i < plugin.Parameters.Length; i++)
            {
                preset.Parameters[i] = plugin.Parameters[i];
            }

            _presets.Add(preset);
            SavePresetToFile(preset);
        }

        private void SavePresetToFile(Preset preset)
        {
            var filename = Path.Combine(_presetsDirectory, $"{preset.Name}.json");
            var json = JsonSerializer.Serialize(preset, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
        }

        public void LoadPreset(string presetName, PluginInstance plugin)
        {
            var preset = _presets.Find(p => p.Name == presetName);
            if (preset != null && preset.PluginPath == plugin.PluginPath)
            {
                foreach (var param in preset.Parameters)
                {
                    plugin.SetParameter(param.Key, param.Value);
                }
            }
        }

        public void LoadPresetsFromDisk()
        {
            _presets.Clear();
            if (!Directory.Exists(_presetsDirectory)) return;

            var presetFiles = Directory.GetFiles(_presetsDirectory, "*.json");
            foreach (var file in presetFiles)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var preset = JsonSerializer.Deserialize<Preset>(json);
                    if (preset != null)
                    {
                        _presets.Add(preset);
                    }
                }
                catch
                {
                    // Skip corrupted preset files
                }
            }
        }

        public List<Preset> GetPresets() => new List<Preset>(_presets);

        public Preset GetPreset(string name) => _presets.Find(p => p.Name == name);
    }

    public class Preset
    {
        public string Name { get; set; }
        public string PluginName { get; set; }
        public string PluginPath { get; set; }
        public Dictionary<int, float> Parameters { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
