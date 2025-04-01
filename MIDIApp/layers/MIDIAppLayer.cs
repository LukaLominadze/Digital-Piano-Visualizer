using Game2D.glcore;
using Game2D.layers;
using Game2D.core;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using ImGuiNET;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;

namespace MIDIApp.layers
{
    internal class MIDIAppLayer : Layer
    {
        private Framebuffer? framebuffer;
        private InputDeviceManager? inputDeviceManager;
        // Used for checking if we have switched to another midi device
        private int current = 0;
        private int previous = 0;
        // ------------------------------------------------------------
        // Used for toggling the colors of the keys
        private bool[] pressed = new bool[88];

        // Define parameters for how the keys are going to look
        public class KeyStats
        {
            public static float Offset = 0.05f;
            public static float Width = 0.1f, Height = 0.3f;
            public static float BlackWidth = 0.1f, BlackHeight = 0.2f;
            public static Color4 PressedWhiteColor
            {
                get
                {
                    return new Color4(pressedWhiteColor.X,
                                      pressedWhiteColor.Y,
                                      pressedWhiteColor.Z,
                                      pressedWhiteColor.W);
                }
                set
                {

                }
            }
            public static Color4 PressedBlackColor
            {
                get
                {
                    return new Color4(pressedBlackColor.X,
                                      pressedBlackColor.Y,
                                      pressedBlackColor.Z,
                                      pressedBlackColor.W);
                }
                private set { }
            }
            public static System.Numerics.Vector4 pressedWhiteColor = new System.Numerics.Vector4(0.18f, 0.2f, 0.8f, 1.0f);
            public static System.Numerics.Vector4 pressedBlackColor = new System.Numerics.Vector4(0.34f, 0.6f, 1.0f, 1.0f);


            public static void SetKeyStats()
            {
                float width = 2.0f / 52.0f;
                Width = width * 0.65f;
                Height = 2.0f * 0.1f;
                Offset = width * 0.2f;
                BlackWidth = KeyStats.Width * 0.8f;
                BlackHeight = KeyStats.Height * 0.7f;
            }
        }

        public override void OnAttach()
        {
            inputDeviceManager = new InputDeviceManager();
            framebuffer = new Framebuffer((int)Game.WindowData.ScreenWidth, (int)(Game.WindowData.ScreenHeight * 0.4f));
            float width = 2.0f / 52.0f;
            KeyStats.SetKeyStats();

            inputDeviceManager.SetCurrentDevice(inputDeviceManager.InputDevices[current].Name);
            inputDeviceManager.SetEventRecievedCallback(OnEventReceived!);
            inputDeviceManager.StartEventsListening();
        }

        public override void OnResize(ResizeEventArgs args)
        {
            framebuffer = new Framebuffer(args.Width, (int)(args.Height * 0.2f));
            KeyStats.SetKeyStats();
        }

        public override void OnUpdate(FrameEventArgs args)
        {
            // If we have switched to another device, start listening
            if (previous != current)
            {
                ReloadDevice();
            }
        }

        private void ReloadDevice()
        {
            previous = current;
            inputDeviceManager!.StopEventsListening();
            inputDeviceManager!.ClearEventRecievedCallback();

            inputDeviceManager!.SetCurrentDevice(inputDeviceManager.InputDevices[current].Name);
            inputDeviceManager!.SetEventRecievedCallback(OnEventReceived!);
            inputDeviceManager!.StartEventsListening();
        }

        public override void OnRender(FrameEventArgs args)
        {
            framebuffer!.Bind();
            Renderer2D.BeginScene();
            Renderer2D.Clear();
            // Draw the keys
            float offsetX = (KeyStats.Width + KeyStats.Offset) * 26 - KeyStats.Width / 2.0f;
            float offsetY = KeyStats.Height * 2;
            int k = 9;
            int w = 0;
            for (int i = 0; i < 88; i++)
            {
                Color4 blackColor = Color4.Black;
                Color4 whiteColor = Color4.White;
                if (pressed[i])
                {
                    whiteColor = KeyStats.PressedWhiteColor;
                    blackColor = KeyStats.PressedBlackColor;
                }
                if (k != 1 && k != 3 && k != 6 && k != 8 && k != 10)
                {
                    Renderer2D.DrawQuad(new Vector3(-offsetX + w * (KeyStats.Width + KeyStats.Offset), -offsetY, -0.11f),
                                    new Vector2(KeyStats.Width, KeyStats.Height),
                                    whiteColor);
                    w++;
                }
                else
                {
                    Renderer2D.DrawQuad(new Vector3(-offsetX + ((w - 1) * (KeyStats.Width + KeyStats.Offset)) + (KeyStats.Width + KeyStats.BlackWidth / 2.0f) / 2.0f,
                                                    -offsetY + (KeyStats.Height - KeyStats.BlackHeight) / 2.0f,
                                                    -0.1f),
                                        new Vector2(KeyStats.BlackWidth, KeyStats.BlackHeight),
                                        blackColor);
                }
                if (++k > 11)
                {
                    k = 0;
                }
            }
            // ------------------
            Renderer2D.EndScene();
            framebuffer.Unbind();
        }

        public override void OnImGuiRender()
        {
            ImGui.Begin("App");
            ImGui.ColorEdit4("Pressed White Key Color", ref KeyStats.pressedWhiteColor);
            ImGui.ColorEdit4("Pressed Black Key Color", ref KeyStats.pressedBlackColor);
            ImGui.End();
            ImGui.Begin("Piano");
            ImGui.Combo("Device", ref current, inputDeviceManager!.GetDeviceNames(), inputDeviceManager.InputDevices.Count);
            ImGui.SameLine();
            if (ImGui.Button("Reload")) ReloadDevice();
            ImGui.Image(framebuffer!.ColorAttachment,
                        new System.Numerics.Vector2(Game.WindowData.ScreenWidth, Game.WindowData.ScreenHeight * 0.2f),
                        new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
            ImGui.End();
        }

        private void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            var midiDevice = (MidiDevice)sender;
            Log.DebugLog($"Event received from '{midiDevice.Name}' at {DateTime.Now}: {e.Event}");

            if (e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.NoteOn)
            {
                NoteOnEvent E = (NoteOnEvent)e.Event;
                pressed[E.NoteNumber - 21] = true;
            }
            else if (e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.NoteOff)
            {
                NoteOffEvent E = (NoteOffEvent)e.Event;
                pressed[E.NoteNumber - 21] = false;
            }
        }
    }
}
