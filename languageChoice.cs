using Godot;
using System;

public partial class languageChoice : Control
{
	
	private Button backButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backButton = GetNode<Button>("BackButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBackButtonPressed()
	{
		//change this to go back to previous scene via global variable
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://mainUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}
}
