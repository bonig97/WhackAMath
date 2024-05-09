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

    /// <summary>
    /// Initializes the settings UI and connects UI elements to their respective handlers.
    /// </summary>
    public override void _Ready()
    {
        InitializeControls();
        ConnectSignals();
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

    private void OnTutorialButtonPressed() => GD.Print("Tutorial button pressed");

    private void OnMusicSliderValueChanged(float value) => GD.Print($"Music volume: {value}");

    private void OnEffectsSliderValueChanged(float value) => GD.Print($"Effects volume: {value}");

    private void OnChangePasswordButtonPressed()
    {
        ChangeScene("res://scenes/UI/changePasswordUI.tscn");
    }

    private void OnDeleteAccountButtonPressed()
    {
        ChangeScene("res://scenes/UI/deleteAccountUI.tscn");
    }

    private void OnBackButtonPressed()
    {
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
