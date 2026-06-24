using NAudio.Midi;
using System;
using System.Collections.Generic;

namespace VST_Audio_Host.Core
{
    /// <summary>
    /// Manages MIDI input devices and routing
    /// </summary>
    public class MidiManager
    {
        private MidiInPort _midiIn;
        private List<MidiMapping> _mappings;
        private int _currentDevice;

        public event EventHandler<MidiMessageEventArgs> MidiMessageReceived;

        public MidiManager()
        {
            _mappings = new List<MidiMapping>();
            _currentDevice = -1;
        }

        public int GetDeviceCount() => MidiIn.NumberOfDevices;

        public string GetDeviceName(int deviceId)
        {
            if (deviceId >= 0 && deviceId < MidiIn.NumberOfDevices)
            {
                return MidiIn.DeviceInfo(deviceId).ProductName;
            }
            return "Unknown Device";
        }

        public void SelectDevice(int deviceId)
        {
            if (deviceId == _currentDevice) return;

            CloseDevice();

            if (deviceId >= 0 && deviceId < MidiIn.NumberOfDevices)
            {
                _midiIn = new MidiInPort(deviceId);
                _midiIn.MessageReceived += OnMidiMessageReceived;
                _currentDevice = deviceId;
            }
        }

        private void OnMidiMessageReceived(long position, int data1, int data2, int data3)
        {
            var e = new MidiMessageEventArgs
            {
                Status = (byte)((data1 >> 0) & 0xFF),
                Data1 = (byte)((data1 >> 8) & 0xFF),
                Data2 = (byte)((data1 >> 16) & 0xFF)
            };

            MidiMessageReceived?.Invoke(this, e);
            ProcessMappings(e);
        }

        private void ProcessMappings(MidiMessageEventArgs e)
        {
            foreach (var mapping in _mappings)
            {
                if (mapping.MidiChannel == (e.Status & 0x0F))
                {
                    if (mapping.MidiType == MidiMessageType.ControlChange && (e.Status >> 4) == 11)
                    {
                        mapping.OnMidiReceived(e.Data1, e.Data2 / 127f);
                    }
                    else if (mapping.MidiType == MidiMessageType.NoteOn && (e.Status >> 4) == 9)
                    {
                        mapping.OnMidiReceived(e.Data1, e.Data2 / 127f);
                    }
                }
            }
        }

        public void AddMapping(MidiMapping mapping)
        {
            _mappings.Add(mapping);
        }

        public void RemoveMapping(MidiMapping mapping)
        {
            _mappings.Remove(mapping);
        }

        public void CloseDevice()
        {
            if (_midiIn != null)
            {
                _midiIn.Close();
                _midiIn.Dispose();
                _midiIn = null;
                _currentDevice = -1;
            }
        }
    }

    public class MidiMessageEventArgs : EventArgs
    {
        public byte Status { get; set; }
        public byte Data1 { get; set; }
        public byte Data2 { get; set; }
    }

    public enum MidiMessageType
    {
        NoteOff = 8,
        NoteOn = 9,
        ControlChange = 11,
        ProgramChange = 12
    }

    public class MidiMapping
    {
        public int MidiChannel { get; set; }
        public byte MidiCC { get; set; }
        public MidiMessageType MidiType { get; set; }
        public PluginInstance TargetPlugin { get; set; }
        public int TargetParameter { get; set; }

        public event EventHandler<MidiMappingEventArgs> ValueChanged;

        public void OnMidiReceived(byte controller, float normalizedValue)
        {
            if (TargetPlugin != null)
            {
                TargetPlugin.SetParameter(TargetParameter, normalizedValue);
                ValueChanged?.Invoke(this, new MidiMappingEventArgs { Value = normalizedValue });
            }
        }
    }

    public class MidiMappingEventArgs : EventArgs
    {
        public float Value { get; set; }
    }
}
