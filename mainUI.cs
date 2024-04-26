using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

public partial class mainUI : Control
{
	// Input Fields
	private Button levelSelectButton;
	private Button settingsButton;
	private Button logoutButton;
	private Button endlessModeButton;
	private Button languageButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create the input fields
		levelSelectButton = GetNode<Button>("LevelSelectButton");
		settingsButton = GetNode<Button>("SettingsButton");
		logoutButton = GetNode<Button>("LogoutButton");
		endlessModeButton = GetNode<Button>("EndlessModeButton");
		languageButton = GetNode<Button>("LanguageButton");

		// Create the login button
		levelSelectButton.Connect("pressed", new Callable(this, nameof(OnLevelSelectButtonPressed)));
		settingsButton.Connect("pressed", new Callable(this, nameof(OnSettingsButtonPressed)));
		logoutButton.Connect("pressed", new Callable(this, nameof(OnLogoutButtonPressed)));
		endlessModeButton.Connect("pressed", new Callable(this, nameof(OnEndlessModeButtonPressed)));
		languageButton.Connect("pressed", new Callable(this, nameof(OnLanguageButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnLevelSelectButtonPressed()
	{
		GD.Print("Level Select Button Pressed");
	}

	private void OnSettingsButtonPressed()
	{
		PackedScene settingsScene = (PackedScene)ResourceLoader.Load("res://settingsUI.tscn");
		GetTree().ChangeSceneToPacked(settingsScene);
	}

	private void OnLogoutButtonPressed()
	{
		FirestoreHelper.SignOut();
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://loginUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}

	private void OnEndlessModeButtonPressed()
	{
		PackedScene endlessModeScene = (PackedScene)ResourceLoader.Load("res://Level.tscn");
		GetTree().ChangeSceneToPacked(endlessModeScene);
	}

	private void OnLanguageButtonPressed()
	{
		PackedScene languageScene = (PackedScene)ResourceLoader.Load("res://languageUI.tscn");
		GetTree().ChangeSceneToPacked(languageScene);
	}
}
