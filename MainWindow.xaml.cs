using System;
using System.Collections.ObjectModel;
using System.Windows;
using VST_Audio_Host.Core;

namespace VST_Audio_Host
{
    public partial class MainWindow : Window
    {
        private AudioEngine _audioEngine;
        private PluginManager _pluginManager;
        private MidiManager _midiManager;
        private PresetManager _presetManager;
        private SetlistManager _setlistManager;

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            try
            {
                // Initialize all managers
                _audioEngine = new AudioEngine();
                _pluginManager = new PluginManager();
                _midiManager = new MidiManager();
                _presetManager = new PresetManager();
                _setlistManager = new SetlistManager();

                _audioEngine.Initialize();

                // Load existing data
                _presetManager.LoadPresetsFromDisk();
                _setlistManager.LoadSetlistsFromDisk();

                // Populate UI
                PopulateMidiDevices();
                RefreshSetlistUI();

                StatusText.Text = "Application initialized successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Initialization Error");
            }
        }

        private void PopulateMidiDevices()
        {
            MidiDeviceComboBox.Items.Clear();
            MidiDeviceComboBox.Items.Add("No Device");

            for (int i = 0; i < _midiManager.GetDeviceCount(); i++)
            {
                MidiDeviceComboBox.Items.Add(_midiManager.GetDeviceName(i));
            }

            MidiDeviceComboBox.SelectedIndex = 0;
            MidiDeviceComboBox.SelectionChanged += (s, e) =>
            {
                if (MidiDeviceComboBox.SelectedIndex > 0)
                {
                    _midiManager.SelectDevice(MidiDeviceComboBox.SelectedIndex - 1);
                    StatusText.Text = $"MIDI Device: {_midiManager.GetDeviceName(MidiDeviceComboBox.SelectedIndex - 1)}";
                }
            };
        }

        private void ScanPlugins_Click(object sender, RoutedEventArgs e)
        {
            AvailablePluginsList.Items.Clear();
            StatusText.Text = "Scanning for plugins...";

            _pluginManager.ScanForPlugins();
            
            foreach (var plugin in _pluginManager.GetDiscoveredPlugins())
            {
                AvailablePluginsList.Items.Add($"{plugin.Name} ({plugin.Category})");
            }

            StatusText.Text = $"Found {_pluginManager.GetDiscoveredPlugins().Count} plugins";
        }

        private void AddPlugin_Click(object sender, RoutedEventArgs e)
        {
            if (AvailablePluginsList.SelectedIndex >= 0)
            {
                var plugins = _pluginManager.GetDiscoveredPlugins();
                var selectedPlugin = plugins[AvailablePluginsList.SelectedIndex];
                
                var instance = _pluginManager.LoadPlugin(selectedPlugin.Path);
                _audioEngine.AddPlugin(instance);

                AddPluginToRack(instance);
                StatusText.Text = $"Loaded: {selectedPlugin.Name}";
                ActivePluginsText.Text = _audioEngine.GetActivePlugins().Count.ToString();
            }
        }

        private void AddPluginToRack(PluginInstance plugin)
        {
            var pluginCard = new System.Windows.Controls.Border
            {
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48)),
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 100, 100)),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                CornerRadius = new System.Windows.CornerRadius(5)
            };

            var panel = new System.Windows.Controls.StackPanel();
            panel.Children.Add(new System.Windows.Controls.TextBlock
            {
                Text = plugin.Name,
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                FontWeight = System.Windows.FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            var removeBtn = new System.Windows.Controls.Button
            {
                Content = "Remove",
                Padding = new Thickness(10, 5, 10, 5),
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(211, 52, 56)),
                Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White),
                Width = 80
            };

            removeBtn.Click += (s, e) =>
            {
                _audioEngine.RemovePlugin(plugin);
                PluginRackPanel.Children.Remove(pluginCard);
                StatusText.Text = $"Removed: {plugin.Name}";
                ActivePluginsText.Text = _audioEngine.GetActivePlugins().Count.ToString();
            };

            panel.Children.Add(removeBtn);
            pluginCard.Child = panel;
            PluginRackPanel.Children.Add(pluginCard);
        }

        private void AddMidiMapping_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MIDI Mapping feature coming soon!", "Feature Preview");
        }

        private void NewSetlist_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Setlist creation dialog coming soon!", "Feature Preview");
        }

        private void DeleteSetlist_Click(object sender, RoutedEventArgs e)
        {
            if (SetlistBox.SelectedIndex >= 0)
            {
                var setlists = _setlistManager.GetSetlists();
                _setlistManager.DeleteSetlist(setlists[SetlistBox.SelectedIndex].Name);
                RefreshSetlistUI();
            }
        }

        private void RefreshSetlistUI()
        {
            SetlistBox.Items.Clear();
            foreach (var setlist in _setlistManager.GetSetlists())
            {
                SetlistBox.Items.Add(setlist.Name);
            }
        }

        private void ApplySettings_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "Settings applied";
        }

        protected override void OnClosed(EventArgs e)
        {
            _audioEngine?.Dispose();
            _midiManager?.CloseDevice();
            base.OnClosed(e);
        }
    }
}
