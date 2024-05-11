using Godot;
using System;
using WhackAMath;

public partial class Customize : Control
{
	private Button backButton;
	private Button nextButton;
	private Button prevButton;
	private Button selectButton;
	private MarginContainer marginContainer;
	private HBoxContainer hboxContainer;
	private int current_grid = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backButton = GetNode<Button>("BackButton");
		nextButton = GetNode<Button>("NextButton");
		prevButton = GetNode<Button>("PrevButton");
		selectButton = GetNode<Button>("SelectButton");
		marginContainer = GetNode<MarginContainer>("MarginContainer");
		hboxContainer = GetNode<HBoxContainer>("MarginContainer/ClipControl/HBoxContainer");

		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
		nextButton.Connect("pressed", new Callable(this, nameof(OnNextButtonPressed)));
		prevButton.Connect("pressed", new Callable(this, nameof(OnPrevButtonPressed)));
		selectButton.Connect("pressed", new Callable(this, nameof(OnSelectButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBackButtonPressed()
	{
		PackedScene scene = GD.Load<PackedScene>("res://scenes/UI/MainUI.tscn");
		GetTree().ChangeSceneToPacked(scene);
	}
	private void OnNextButtonPressed()
	{
		if (current_grid < hboxContainer.GetChildren().Count - 1)
		{
			current_grid++;
			hboxContainer.Position = new Vector2(-current_grid * 128, 0);
		}
		else
		{
			current_grid = 0;
			hboxContainer.Position = new Vector2(-current_grid*128, 0);
		}
	}
	private void OnPrevButtonPressed()
	{
		if (current_grid > 0)
		{
			current_grid--;
			hboxContainer.Position = new Vector2(-current_grid * 128, 0);
		}
		else
		{
			current_grid = hboxContainer.GetChildren().Count - 1;
			hboxContainer.Position = new Vector2(-current_grid*128, 0);
		}
	}
	private void OnSelectButtonPressed()
	{
		GD.Print("Select button pressed");
		SaveFile.UpdateMoleSelected(current_grid);
	}

}
