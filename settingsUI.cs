using Godot;
using System;

public partial class settingsUI : Control
{
	private Button tutorialButton;
	private HSlider musicSlider;
	private HSlider effectsSlider;
	private Button changePasswordButton;
	private Button deleteAccountButton;
	private Button backButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tutorialButton = GetNode<Button>("TutorialButton");
		musicSlider = GetNode<HSlider>("VBoxContainer/Music/MusicSlider");
		effectsSlider = GetNode<HSlider>("VBoxContainer/Effects/EffectsSlider");
		changePasswordButton = GetNode<Button>("VBoxContainer/Control/ChangePasswordButton");
		deleteAccountButton = GetNode<Button>("VBoxContainer/Control/DeleteAccountButton");
		backButton = GetNode<Button>("BackButton");
		
		tutorialButton.Connect("pressed", new Callable(this,nameof(OnTutorialButtonPressed)));
		musicSlider.Connect("value_changed", new Callable(this,nameof(OnMusicSliderValueChanged)));
		effectsSlider.Connect("value_changed", new Callable(this,nameof(OnEffectsSliderValueChanged)));
		changePasswordButton.Connect("pressed", new Callable(this,nameof(OnChangePasswordButtonPressed)));
		deleteAccountButton.Connect("pressed", new Callable(this,nameof(OnDeleteAccountButtonPressed)));
		backButton.Connect("pressed", new Callable(this,nameof(OnBackButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnTutorialButtonPressed()
	{
		// Handle tutorial button press
		GD.Print("Tutorial button pressed");
	}

	private void OnMusicSliderValueChanged(float value)
	{
		// Handle music slider value change
		GD.Print($"Music volume: {value}");
	}

	private void OnEffectsSliderValueChanged(float value)
	{
		// Handle effects slider value change
		GD.Print($"Effects volume: {value}");
	}

	private void OnChangePasswordButtonPressed()
	{
		PackedScene changePasswordScene = (PackedScene)ResourceLoader.Load("res://changePasswordUI.tscn");
		GetTree().ChangeSceneToPacked(changePasswordScene);
	}

	private void OnDeleteAccountButtonPressed()
	{
		PackedScene deleteAccountScene = (PackedScene)ResourceLoader.Load("res://deleteAccountUI.tscn");
		GetTree().ChangeSceneToPacked(deleteAccountScene);
	}

	private void OnBackButtonPressed()
	{
		PackedScene mainUIScene = (PackedScene)ResourceLoader.Load("res://mainUI.tscn");
		GetTree().ChangeSceneToPacked(mainUIScene);
	}
}
