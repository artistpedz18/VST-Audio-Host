using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VST_Audio_Host.Core
{
    /// <summary>
    /// Manages VST plugin discovery, loading, and unloading
    /// </summary>
    public class PluginManager
    {
        private List<PluginInfo> _discoveredPlugins;
        private List<PluginInstance> _loadedPlugins;
        private string[] _pluginSearchPaths;

        public PluginManager()
        {
            _discoveredPlugins = new List<PluginInfo>();
            _loadedPlugins = new List<PluginInstance>();
            _pluginSearchPaths = GetDefaultSearchPaths();
        }

        private string[] GetDefaultSearchPaths()
        {
            return new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steinberg", "VstPlugins"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Common Files", "VST2"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Common Files", "VST3"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Common", "VST"),
            };
        }

        public void ScanForPlugins()
        {
            _discoveredPlugins.Clear();

            foreach (var searchPath in _pluginSearchPaths)
            {
                if (Directory.Exists(searchPath))
                {
                    ScanDirectory(searchPath);
                }
            }
        }

        private void ScanDirectory(string path)
        {
            try
            {
                var dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        var pluginInfo = new PluginInfo
                        {
                            Name = Path.GetFileNameWithoutExtension(dllFile),
                            Path = dllFile,
                            Version = "1.0",
                            Category = DetermineCategory(dllFile)
                        };

                        if (!_discoveredPlugins.Any(p => p.Path == dllFile))
                        {
                            _discoveredPlugins.Add(pluginInfo);
                        }
                    }
                    catch
                    {
                        // Skip plugins that can't be loaded
                    }
                }
            }
            catch
            {
                // Skip directories that can't be accessed
            }
        }

        private string DetermineCategory(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path).ToLower();
            
            if (name.Contains("synth")) return "Synthesizer";
            if (name.Contains("reverb") || name.Contains("delay")) return "Reverb/Delay";
            if (name.Contains("eq") || name.Contains("filter")) return "EQ/Filter";
            if (name.Contains("compressor") || name.Contains("limiter")) return "Dynamics";
            
            return "Effect";
        }

        public PluginInstance LoadPlugin(string pluginPath)
        {
            var instance = new PluginInstance
            {
                PluginPath = pluginPath,
                Name = Path.GetFileNameWithoutExtension(pluginPath),
                IsLoaded = true
            };

            _loadedPlugins.Add(instance);
            return instance;
        }

        public void UnloadPlugin(PluginInstance plugin)
        {
            _loadedPlugins.Remove(plugin);
            plugin.Dispose();
        }

        public List<PluginInfo> GetDiscoveredPlugins() => new List<PluginInfo>(_discoveredPlugins);

        public List<PluginInstance> GetLoadedPlugins() => new List<PluginInstance>(_loadedPlugins);
    }

    public class PluginInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Version { get; set; }
        public string Category { get; set; }
    }

    public class PluginInstance : IDisposable
    {
        public string Name { get; set; }
        public string PluginPath { get; set; }
        public bool IsLoaded { get; set; }
        public float[] Parameters { get; set; }
        public int Id { get; } = Guid.NewGuid().GetHashCode();

        public PluginInstance()
        {
            Parameters = new float[128];
        }

        public void SetParameter(int index, float value)
        {
            if (index >= 0 && index < Parameters.Length)
            {
                Parameters[index] = Math.Clamp(value, 0f, 1f);
            }
        }

        public float GetParameter(int index)
        {
            if (index >= 0 && index < Parameters.Length)
                return Parameters[index];
            return 0f;
        }

        public void Dispose()
        {
            // Cleanup plugin resources
        }
    }
}
