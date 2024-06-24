using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

public partial class Main : Control
{
    // Input Fields
    private Button LevelSelectButton;
    private Button SettingsButton;
    private Button LogoutButton;
    private Button EndlessModeButton;
    private Button LanguageButton;
    private Button CustomizeButton;
    private IDataService DataService;

    public override void _Ready()
    {
        LevelSelectButton = GetNode<Button>("LevelSelectButton");
        SettingsButton = GetNode<Button>("SettingsButton");
        LogoutButton = GetNode<Button>("LogoutButton");
        EndlessModeButton = GetNode<Button>("EndlessModeButton");
        LanguageButton = GetNode<Button>("LanguageButton");
        CustomizeButton = GetNode<Button>("CustomizeButton");

        LevelSelectButton.Connect("pressed", new Callable(this, nameof(OnLevelSelectButtonPressed)));
        SettingsButton.Connect("pressed", new Callable(this, nameof(OnSettingsButtonPressed)));
        LogoutButton.Connect("pressed", new Callable(this, nameof(OnLogoutButtonPressed)));
        EndlessModeButton.Connect("pressed", new Callable(this, nameof(OnEndlessModeButtonPressed)));
        LanguageButton.Connect("pressed", new Callable(this, nameof(OnLanguageButtonPressed)));
        CustomizeButton.Connect("pressed", new Callable(this, nameof(OnCustomizeButtonPressed)));

        if (InitialStart.IsOfflineMode)
        {
            LogoutButton.Text = "Exit";
        }
        else
        {
            LogoutButton.Text = "Logout";
        }

        DataService = InitialStart.DataService;

        // Load user settings and play music
        float musicVolume = 0.5f;
        ConfigFile configFile = new ConfigFile();
        var err = configFile.Load("user://settings.cfg");
        if (err == Error.Ok)
        {
            musicVolume = (float)configFile.GetValue("audio", "music_volume", 0.5);
        }

        AudioManager.Singleton.PlayMainMusic(musicVolume);
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
        DataService.Logout();
        if (DataService is FirestoreHelper)
        {
            ChangeScene("res://scenes/UI/loginUI.tscn");
        }
    }

    private void OnEndlessModeButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
        ChangeScene("res://scenes/levels/EndlessLevel.tscn");
    }

    private void OnLanguageButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
        PackedScene mainUI = (PackedScene)ResourceLoader.Load("res://scenes/UI/mainUI.tscn");
        SaveFile.PrevScene = mainUI;
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
