# Professional VST Audio Host

A modern, professional-grade audio plugin host for Windows 10 that allows you to load VST plugins, manage MIDI inputs, and create complex audio setups.

## Features

- **VST Plugin Hosting** - Load VST2/VST3 plugins dynamically
- **MIDI Support** - Read and route MIDI commands from keyboards and controllers
- **Setlist Management** - Organize and switch between different patch configurations
- **MIDI Mapping** - Map MIDI CC, notes, and other controls to plugin parameters
- **Audio Routing** - Flexible audio input/output configuration
- **Real-time Performance Monitoring** - CPU usage, latency, buffer info
- **Preset Management** - Save and load plugin states
- **Modern UI** - Clean, intuitive interface with tabbed navigation
- **Performance Optimization** - Multi-threaded audio processing

## Technology Stack

- **Language**: C# (.NET 6+)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Audio Processing**: NAudio
- **Threading**: Task Parallel Library (TPL)

## Project Structure

```
VST-Audio-Host/
├── Core/
│   ├── AudioEngine.cs
│   ├── PluginManager.cs
│   ├── MidiManager.cs
│   ├── PresetManager.cs
│   └── SetlistManager.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── App.xaml
├── App.xaml.cs
└── README.md
```

## Getting Started

### Requirements

- Windows 10 or later
- .NET 6 SDK or later
- Visual Studio 2019 or later

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/artistpedz18/VST-Audio-Host.git
   ```

2. Open the solution in Visual Studio

3. Restore NuGet packages
   ```bash
   dotnet restore
   ```

4. Build the solution
   ```bash
   dotnet build
   ```

5. Run the application
   ```bash
   dotnet run
   ```

## Usage

### Loading Plugins

1. Click "Scan Plugins" to discover VST plugins on your system
2. Select a plugin from the available list
3. Click "Add Plugin" to load it into the plugin rack
4. Plugins appear as cards in the Plugin Rack tab

### MIDI Setup

1. Go to the "MIDI Setup" tab
2. Select your MIDI input device from the dropdown
3. Create MIDI mappings to control plugin parameters
4. Test with your MIDI controller

### Creating a Setlist

1. Go to the "Setlists" tab
2. Click "New Setlist" to create a new configuration
3. Add plugin slots and assign presets
4. Switch between setlists during performance

### Performance Monitoring

1. Click the "Monitor" tab to view real-time statistics
2. Track CPU usage and active plugin count
3. Monitor latency information

## Features in Development

- [ ] VST3 native plugin loading
- [ ] Audio visualization and metering
- [ ] Advanced MIDI learn mode
- [ ] Plugin parameter automation
- [ ] Multi-track recording
- [ ] Effect chaining with drag-and-drop
- [ ] Custom skin support
- [ ] Plugin grouping and organization

## Architecture

### Core Components

**AudioEngine**: Central audio processing system managing playback and plugin routing

**PluginManager**: Discovers, loads, and manages VST plugins

**MidiManager**: Handles MIDI device input and routing

**PresetManager**: Saves and loads plugin states

**SetlistManager**: Organizes plugin configurations into setlists

## Building from Source

```bash
cd VST-Audio-Host
dotnet build -c Release
```

The compiled executable will be in `bin/Release/net6.0-windows/`

## Known Limitations

- VST plugin loading currently shows available plugins but requires extended plugin wrapper library for full VST integration
- MIDI learn mode is in the UI but requires additional implementation
- Some complex VST3 features not yet supported

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

MIT License - See LICENSE file for details

## Troubleshooting

### Plugins Not Found
- Ensure your VST plugins are installed in standard locations
- Check the "Scan Plugins" function to auto-discover plugins

### MIDI Device Not Connecting
- Ensure your MIDI device is properly connected and drivers are installed
- Try reconnecting the device and refreshing the MIDI device list

### Audio Issues
- Adjust buffer size in Settings
- Check audio output device configuration
- Reduce number of active plugins if experiencing latency

## Support

For issues and feature requests, please create an issue on GitHub.

---

**Warning**: Some VST plugins may require additional runtime dependencies. Ensure all required plugins are properly installed on your system.
