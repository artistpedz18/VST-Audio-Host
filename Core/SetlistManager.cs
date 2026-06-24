using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace VST_Audio_Host.Core
{
    /// <summary>
    /// Manages setlists of plugin configurations
    /// </summary>
    public class SetlistManager
    {
        private string _setlistsDirectory;
        private List<Setlist> _setlists;
        private Setlist _currentSetlist;

        public event EventHandler CurrentSetlistChanged;

        public SetlistManager(string setlistsDirectory = null)
        {
            _setlistsDirectory = setlistsDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "VST-Audio-Host", "Setlists");

            _setlists = new List<Setlist>();
            EnsureDirectoryExists();
        }

        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_setlistsDirectory))
            {
                Directory.CreateDirectory(_setlistsDirectory);
            }
        }

        public Setlist CreateSetlist(string name)
        {
            var setlist = new Setlist
            {
                Name = name,
                CreatedAt = DateTime.Now,
                Slots = new List<SetlistSlot>()
            };

            _setlists.Add(setlist);
            SaveSetlistToFile(setlist);
            return setlist;
        }

        public void AddSlotToSetlist(Setlist setlist, SetlistSlot slot)
        {
            setlist.Slots.Add(slot);
            SaveSetlistToFile(setlist);
        }

        public void SelectSetlist(Setlist setlist)
        {
            _currentSetlist = setlist;
            CurrentSetlistChanged?.Invoke(this, EventArgs.Empty);
        }

        public Setlist GetCurrentSetlist() => _currentSetlist;

        public void DeleteSetlist(string name)
        {
            var setlist = _setlists.Find(s => s.Name == name);
            if (setlist != null)
            {
                _setlists.Remove(setlist);
                var filepath = Path.Combine(_setlistsDirectory, $"{name}.json");
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
            }
        }

        private void SaveSetlistToFile(Setlist setlist)
        {
            var filename = Path.Combine(_setlistsDirectory, $"{setlist.Name}.json");
            var json = JsonSerializer.Serialize(setlist, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
        }

        public void LoadSetlistsFromDisk()
        {
            _setlists.Clear();
            if (!Directory.Exists(_setlistsDirectory)) return;

            var setlistFiles = Directory.GetFiles(_setlistsDirectory, "*.json");
            foreach (var file in setlistFiles)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var setlist = JsonSerializer.Deserialize<Setlist>(json);
                    if (setlist != null)
                    {
                        _setlists.Add(setlist);
                    }
                }
                catch
                {
                    // Skip corrupted setlist files
                }
            }
        }

        public List<Setlist> GetSetlists() => new List<Setlist>(_setlists);
    }

    public class Setlist
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SetlistSlot> Slots { get; set; }
    }

    public class SetlistSlot
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public List<PluginConfiguration> PluginConfigs { get; set; }
    }

    public class PluginConfiguration
    {
        public string PluginPath { get; set; }
        public string PresetName { get; set; }
        public Dictionary<int, float> Parameters { get; set; }
    }
}
