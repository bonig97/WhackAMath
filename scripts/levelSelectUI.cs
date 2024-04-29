using Godot;
using System;

public partial class levelSelectUI : Control
{
	// Declare member variables here. Examples:
	private MarginContainer marginContainer;
	private Button backButton;
	private Button nextButton;
	private Button prevButton;
	private int current_grid = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		marginContainer = GetNode<MarginContainer>("MarginContainer");
		backButton = GetNode<Button>("BackButton");
		nextButton = GetNode<Button>("NextButton");
		prevButton = GetNode<Button>("PrevButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
		nextButton.Connect("pressed", new Callable(this, nameof(OnNextButtonPressed)));
		prevButton.Connect("pressed", new Callable(this, nameof(OnPrevButtonPressed)));


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBackButtonPressed()
	{
		//change this to go back to previous scene via global variable
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/mainUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}

	private void OnNextButtonPressed()
	{
		//change this to go to next level
		GD.Print("Next Button Pressed");
		if (current_grid < 3)
		{
			current_grid++;
			
			marginContainer.Position = new Vector2(-current_grid * 1152, 0);
		}
	}

	private void OnPrevButtonPressed()
	{
		//change this to go to previous level
		GD.Print("Prev Button Pressed");
		if (current_grid > 0)
		{
			current_grid--;
			marginContainer.Position = new Vector2(-current_grid * 1152, 0);
		}
	}
}
