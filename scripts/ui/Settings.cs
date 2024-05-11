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

    // Added for managing settings persistence
    private const string MUSIC_VOLUME_KEY = "user_settings/music_volume";
    private const string EFFECTS_VOLUME_KEY = "user_settings/effects_volume";

    /// <summary>
    /// Initializes the settings UI and connects UI elements to their respective handlers.
    /// </summary>
    public override void _Ready()
    {
        InitializeControls();
        ConnectSignals();
        DefineDefaultSettings();
        LoadSettings();
    }

    private void DefineDefaultSettings()
    {
        if (!ProjectSettings.HasSetting(MUSIC_VOLUME_KEY))
        {
            ProjectSettings.SetSetting(MUSIC_VOLUME_KEY, 0.5f);
        }
        if (!ProjectSettings.HasSetting(EFFECTS_VOLUME_KEY))
        {
            ProjectSettings.SetSetting(EFFECTS_VOLUME_KEY, 0.5f);
        }
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
    /// Loads settings from persistent storage.
    /// </summary>
    private void LoadSettings()
    {
        var musicVolume = (float)ProjectSettings.GetSetting(MUSIC_VOLUME_KEY);
        var effectsVolume = (float)ProjectSettings.GetSetting(EFFECTS_VOLUME_KEY);

        musicSlider.Value = musicVolume;
        effectsSlider.Value = effectsVolume;

        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), musicVolume);
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), effectsVolume);
    }

    /// <summary>
    /// Saves current settings to persistent storage.
    /// </summary>
    private void SaveSettings()
    {
        ProjectSettings.SetSetting(MUSIC_VOLUME_KEY, musicSlider.Value);
        ProjectSettings.SetSetting(EFFECTS_VOLUME_KEY, effectsSlider.Value);
        ProjectSettings.Save();
        GD.Print("Settings saved: Music Volume = " + musicSlider.Value + ", Effects Volume = " + effectsSlider.Value);
    }

    private float ConvertVolumeToDb(float volume)
    {
        return volume > 0.0 ? 20f * Mathf.Log(volume) : -80f;
    }

    private void OnTutorialButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
        GD.Print("Tutorial button pressed");
    }

    private void OnMusicSliderValueChanged(float value)
    {
        AudioManager.Singleton?.PlaySliderSound(value);
        float dbValue = ConvertVolumeToDb(value);
        GD.Print($"Music volume set to: {value}");
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"), dbValue);
        SaveSettings();
    }

    private void OnEffectsSliderValueChanged(float value)
    {
        AudioManager.Singleton?.PlaySliderSound(value);
        float dbValue = ConvertVolumeToDb(value);
        GD.Print($"Effects volume set to: {value}");
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Effects"), dbValue);
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
