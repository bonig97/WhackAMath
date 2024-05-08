using Godot;
using System;
using WhackAMath;

public partial class levelSelectUI : Control
{
	// Declare member variables here. Examples:
	private MarginContainer marginContainer;
	private HBoxContainer hboxContainer;
	private Button backButton;
	private Button nextButton;
	private Button prevButton;
	private Panel panel;
	private Button playButton;
	private Button cancelButton;
	private Label label;
	private int current_grid = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		marginContainer = GetNode<MarginContainer>("MarginContainer");
		hboxContainer = GetNode<HBoxContainer>("MarginContainer/ClipControl/HBoxContainer");
		
		backButton = GetNode<Button>("BackButton");
		nextButton = GetNode<Button>("NextButton");
		prevButton = GetNode<Button>("PrevButton");
		panel = GetNode<Panel>("Panel");
		playButton = GetNode<Button>("Panel/PlayButton");
		cancelButton = GetNode<Button>("Panel/CancelButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
		nextButton.Connect("pressed", new Callable(this, nameof(OnNextButtonPressed)));
		prevButton.Connect("pressed", new Callable(this, nameof(OnPrevButtonPressed)));
		playButton.Connect("pressed", new Callable(this, nameof(OnPlayButtonPressed)));
		cancelButton.Connect("pressed", new Callable(this, nameof(OnCancelButtonPressed)));

		foreach (Node child in hboxContainer.GetChildren())
		{
			if (child is LevelGrid levelGrid)
			{
				levelGrid.LevelSelected += new Action<int>(OnLevelSelected);
			}
			
		}


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

	private void OnLevelSelected(int num)
	{
		GD.Print("Level Selected: " + num);
		panel.Visible = true;
		cancelButton.Disabled = false;
		playButton.Disabled = false;
		SaveFile.currentLevel = num;
	}

	private void OnPlayButtonPressed()
	{
		//change this to go to selected level
		GD.Print("Play Button Pressed");
		PackedScene gameScene = (PackedScene)ResourceLoader.Load("res://scenes/levels/Level.tscn");
		GetTree().ChangeSceneToPacked(gameScene);

	}

	private void OnCancelButtonPressed()
	{
		panel.Visible = false;
		cancelButton.Disabled = true;
		playButton.Disabled = true;
	}
}
