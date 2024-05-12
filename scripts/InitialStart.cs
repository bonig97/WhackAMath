using Godot;
using System;
using WhackAMath;

public partial class InitialStart : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Sets up the database environment
        FirestoreHelper.SetEnvironmentVariable();

        // Creates a default save file
        SaveFile.InitialSaveFile();

        // Checks if AudioManager is initialized and starts playing the main music
        if (AudioManager.Singleton != null)
        {
            AudioManager.Singleton.PlayMainMusic(-20.0f);
            GD.Print("AudioManager is ready and music is now playing.");
        }
        else
        {
            GD.PrintErr("AudioManager is not initialized!");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
