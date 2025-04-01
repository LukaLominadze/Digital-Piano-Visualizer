using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Melanchall.DryWetMidi.Multimedia;

namespace MIDIApp
{
    internal class InputDeviceManager
    {
        // Toggled device where inputs will be read from
        public InputDevice MIDIDevice { get; private set; }
        // All connected input devices

        public List<InputDevice> InputDevices = new List<InputDevice>();
        // Function to clear the listening events of the current device

        Action? ClearEventsFunc;

        public InputDeviceManager()
        {
            InputDevices = InputDevice.GetAll().ToList();
        }

        public void UpdateInputDeviceList()
        {
            InputDevices = InputDevice.GetAll().ToList();
        }

        public string[] GetDeviceNames()
        {
            return InputDevices.Select(x => x.Name).ToArray();
        }

        public InputDevice? SetCurrentDevice(string name)
        {
            InputDevice? device = InputDevices.FirstOrDefault(x => x.Name == name);
            if (device == null)
            {
                Console.WriteLine($"Device ({name}) not found!");
                return null;
            }
            MIDIDevice = device;
            return device;
        }

        // Requires typedef void (object sender, MidiEventReceivedEventArgs args)
        public bool SetEventRecievedCallback(EventHandler<MidiEventReceivedEventArgs> callback)
        {
            if (MIDIDevice == null || ClearEventsFunc != null)
            {
                return false;
            }
            MIDIDevice.EventReceived += callback;
            ClearEventsFunc = () => { MIDIDevice.EventReceived -= callback; };
            return true;
        }

        public void StartEventsListening()
        {
            MIDIDevice.StartEventsListening();
        }

        public void StopEventsListening()
        {
            MIDIDevice.StopEventsListening();
        }

        public void ClearEventRecievedCallback()
        {
            ClearEventsFunc?.Invoke();
            ClearEventsFunc = null;
        }
    }
}
