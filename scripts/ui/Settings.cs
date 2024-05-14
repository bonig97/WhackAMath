using Godot;
using System;

/// <summary>
/// Manages the settings UI, including sound controls, account management, and navigation.
/// </summary>
public partial class Settings : Control
{
	private Button tutorialButton;
	private HSlider musicSlider;
	private HSlider effectsSlider;
	private Button changePasswordButton;
	private Button deleteAccountButton;
	private Button backButton;
	private ConfigFile configFile = new();
	private string configFilePath = "user://settings.cfg";

	/// <summary>
	/// Initializes the settings UI and connects UI elements to their respective handlers.
	/// </summary>
	public override void _Ready()
	{
		InitializeControls();
		ConnectSignals();
		LoadSettings();
	}

	/// <summary>
	/// Initializes UI controls by finding them in the node tree.
	/// </summary>
	private void InitializeControls()
	{
		tutorialButton = GetNode<Button>("TutorialButton");
		musicSlider = GetNode<HSlider>("VBoxContainer/Music/MusicSlider");
		effectsSlider = GetNode<HSlider>("VBoxContainer/Effects/EffectsSlider");
		changePasswordButton = GetNode<Button>("VBoxContainer/Control/ChangePasswordButton");
		deleteAccountButton = GetNode<Button>("VBoxContainer/Control/DeleteAccountButton");
		backButton = GetNode<Button>("BackButton");
	}

	/// <summary>
	/// Connects signals from UI elements to their respective handlers.
	/// </summary>
	private void ConnectSignals()
	{
		tutorialButton.Connect("pressed", new Callable(this, nameof(OnTutorialButtonPressed)));
		musicSlider.Connect("value_changed", new Callable(this, nameof(OnMusicSliderValueChanged)));
		effectsSlider.Connect("value_changed", new Callable(this, nameof(OnEffectsSliderValueChanged)));
		changePasswordButton.Connect("pressed", new Callable(this, nameof(OnChangePasswordButtonPressed)));
		deleteAccountButton.Connect("pressed", new Callable(this, nameof(OnDeleteAccountButtonPressed)));
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	/// <summary>
	/// Loads settings from persistent storage using ConfigFile.
	/// </summary>
	private void LoadSettings()
	{
		var err = configFile.Load(configFilePath);
		if (err == Error.Ok)
		{
			float musicVolume = (float)configFile.GetValue("audio", "music_volume", 0.5);
			float effectsVolume = (float)configFile.GetValue("audio", "effects_volume", 0.5);

			musicSlider.Value = musicVolume;
			effectsSlider.Value = effectsVolume;

			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), musicVolume);
			AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), effectsVolume);
		}
		else
		{
			GD.Print("Failed to load settings: ", err);
		}
	}

	/// <summary>
	/// Saves current settings to persistent storage using ConfigFile.
	/// </summary>
	private void SaveSettings()
	{
		configFile.SetValue("audio", "music_volume", musicSlider.Value);
		configFile.SetValue("audio", "effects_volume", effectsSlider.Value);
		Error err = configFile.Save(configFilePath);
		if (err == Error.Ok)
		{
			GD.Print("Settings saved successfully");
		}
		else
		{
			GD.Print("Failed to save settings: ", err);
		}
	}

	private void OnTutorialButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		GD.Print("Tutorial button pressed");
	}

	private void OnMusicSliderValueChanged(float value)
	{
		AudioManager.Singleton?.PlayMainMusic(value);
		GD.Print($"Music volume set to: {value}");
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), value);
		SaveSettings();
	}

	private void OnEffectsSliderValueChanged(float value)
	{
		AudioManager.Singleton?.PlaySliderSound(value);
		GD.Print($"Effects volume set to: {value}");
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), value);
		SaveSettings();
	}

	private void OnChangePasswordButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/changePasswordUI.tscn");
	}

	private void OnDeleteAccountButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/deleteAccountUI.tscn");
	}

	private void OnBackButtonPressed()
	{
		AudioManager.Singleton?.PlayCancelSound();
		ChangeScene("res://scenes/UI/mainUI.tscn");
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
