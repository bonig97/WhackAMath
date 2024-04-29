using Godot;
using System;
using WhackAMath;

public partial class initial_start : Node
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		FirestoreHelper.SetEnvironmentVariable();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
