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

        if (AudioManager.Singleton != null)
        {
            GD.Print("AudioManager is ready and can be used.");
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
