using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

public partial class Main : Control
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
		// Initialize buttons
		levelSelectButton = GetNode<Button>("LevelSelectButton");
		settingsButton = GetNode<Button>("SettingsButton");
		logoutButton = GetNode<Button>("LogoutButton");
		endlessModeButton = GetNode<Button>("EndlessModeButton");
		languageButton = GetNode<Button>("LanguageButton");
		customizeButton = GetNode<Button>("CustomizeButton");

		// Connect signals with button press handlers
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
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/levelSelectUI.tscn");
	}

	private void OnSettingsButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/settingsUI.tscn");
	}

	private void OnLogoutButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		FirestoreHelper.SignOut();
		ChangeScene("res://scenes/UI/loginUI.tscn");
	}

	private void OnEndlessModeButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/levels/Level.tscn");
	}

	private void OnLanguageButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/languageUI.tscn");
	}

	private void OnCustomizeButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/CustomizeUI.tscn");
	}

	/// <summary>
	/// Changes the current scene to the specified scene.
	/// </summary>
	/// <param name="scenePath">The path to the scene resource to load and switch to.</param>
	private void ChangeScene(string scenePath)
	{
		PackedScene scene = (PackedScene)ResourceLoader.Load(scenePath);
		GetTree().ChangeSceneToPacked(scene);
	}
}
