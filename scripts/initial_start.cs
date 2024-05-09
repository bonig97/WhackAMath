using Godot;
using System;
using WhackAMath;

public partial class initial_start : Node
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Sets up the database environment
		FirestoreHelper.SetEnvironmentVariable();

		// Createss a default save file
		SaveFile.InitialSaveFile();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
