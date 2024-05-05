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
	private Button customizeButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create the input fields
		levelSelectButton = GetNode<Button>("LevelSelectButton");
		settingsButton = GetNode<Button>("SettingsButton");
		logoutButton = GetNode<Button>("LogoutButton");
		endlessModeButton = GetNode<Button>("EndlessModeButton");
		languageButton = GetNode<Button>("LanguageButton");
		customizeButton = GetNode<Button>("CustomizeButton");

		// Create the login button
		levelSelectButton.Connect("pressed", new Callable(this, nameof(OnLevelSelectButtonPressed)));
		settingsButton.Connect("pressed", new Callable(this, nameof(OnSettingsButtonPressed)));
		logoutButton.Connect("pressed", new Callable(this, nameof(OnLogoutButtonPressed)));
		endlessModeButton.Connect("pressed", new Callable(this, nameof(OnEndlessModeButtonPressed)));
		languageButton.Connect("pressed", new Callable(this, nameof(OnLanguageButtonPressed)));
		customizeButton.Connect("pressed", new Callable(this, nameof(OnCustomizeButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnLevelSelectButtonPressed()
	{
		PackedScene levelSelectScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/levelSelectUI.tscn");
		GetTree().ChangeSceneToPacked(levelSelectScene);
	}

	private void OnSettingsButtonPressed()
	{
		PackedScene settingsScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/settingsUI.tscn");
		GetTree().ChangeSceneToPacked(settingsScene);
	}

	private void OnLogoutButtonPressed()
	{
		FirestoreHelper.SignOut();
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}

	private void OnEndlessModeButtonPressed()
	{
		PackedScene endlessModeScene = (PackedScene)ResourceLoader.Load("res://scenes/levels/Level.tscn");
		GetTree().ChangeSceneToPacked(endlessModeScene);
	}

	private void OnLanguageButtonPressed()
	{
		PackedScene languageScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/languageUI.tscn");
		GetTree().ChangeSceneToPacked(languageScene);
	}
	private void OnCustomizeButtonPressed()
	{
		PackedScene customizeScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/customizeUI.tscn");
		GetTree().ChangeSceneToPacked(customizeScene);
	}
}
