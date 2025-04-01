@echo off
dotnet new sln -n MIDIApp --force
cd Game2D
dotnet new classlib -n Game2D --force
cd ..
erase "Game2D\Game2D\Class1.cs"
dotnet new console -n MIDIApp --force
xcopy "vendor\bin\scripts\Program.cs" "MIDIApp\Program.cs" /Y
dotnet sln MIDIApp.sln add MIDIApp/MIDIApp.csproj
dotnet sln MIDIApp.sln add Game2D/Game2D/Game2D.csproj
dotnet add MIDIApp/MIDIApp.csproj reference Game2D/Game2D/Game2D.csproj
dotnet add MIDIApp/MIDIApp.csproj package Melanchall.DryWetMidi
cd Game2D
..\vendor\bin\scripts\setup-engine.bat
cd ..
pause