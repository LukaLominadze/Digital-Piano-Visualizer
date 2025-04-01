using Game2D.core;
using MIDIApp.layers;

Game app = new Game(1280, 720, "MIDI Piano Visualizer", true);
app.AddLayer(new MIDIAppLayer());
app.Run();