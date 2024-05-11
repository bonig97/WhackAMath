using Godot;
using System;
using WhackAMath;

/// <summary>
/// Manages the level selection interface, allowing users to navigate and select levels.
/// </summary>
public partial class LevelSelect : Control
{
    private MarginContainer marginContainer;
    private HBoxContainer hboxContainer;
    private Button backButton;
    private Button nextButton;
    private Button prevButton;
    private Panel panel;
    private Button playButton;
    private Button cancelButton;
    private int current_grid = 0;

    /// <summary>
    /// Initializes the level selection UI components and connects signals when the node enters the scene tree.
    /// </summary>
    public override void _Ready()
    {
        SetupControls();
        ConnectSignals();
    }

    /// <summary>
    /// Initializes UI controls by finding them in the node tree.
    /// </summary>
    private void SetupControls()
    {
        marginContainer = GetNode<MarginContainer>("MarginContainer");
        hboxContainer = GetNode<HBoxContainer>("MarginContainer/ClipControl/HBoxContainer");
        backButton = GetNode<Button>("BackButton");
        nextButton = GetNode<Button>("NextButton");
        prevButton = GetNode<Button>("PrevButton");
        panel = GetNode<Panel>("Panel");
        playButton = GetNode<Button>("Panel/PlayButton");
        cancelButton = GetNode<Button>("Panel/CancelButton");
        panel.Visible = false;
    }

    /// <summary>
    /// Connects button signals to their respective event handlers.
    /// </summary>
    private void ConnectSignals()
    {
        backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
        nextButton.Connect("pressed", new Callable(this, nameof(OnNextButtonPressed)));
        prevButton.Connect("pressed", new Callable(this, nameof(OnPrevButtonPressed)));
        playButton.Connect("pressed", new Callable(this, nameof(OnPlayButtonPressed)));
        cancelButton.Connect("pressed", new Callable(this, nameof(OnCancelButtonPressed)));

        foreach (Node child in hboxContainer.GetChildren())
        {
            if (child is LevelGrid levelGrid)
            {
                levelGrid.LevelSelected += OnLevelSelected;
            }
        }
    }

    private void OnBackButtonPressed()
    {
        AudioManager.Singleton?.PlayCancelSound();
        ChangeScene("res://scenes/UI/mainUI.tscn");
    }

    private void OnNextButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
        if (current_grid < 3)
        {
            current_grid++;
            marginContainer.Position = new Vector2(-current_grid * 1152, 0);
        }
    }

    private void OnPrevButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
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
        AudioManager.Singleton?.PlayConfirmSound();
        ChangeScene("res://scenes/levels/Level.tscn");
    }

    private void OnCancelButtonPressed()
    {
        AudioManager.Singleton?.PlayCancelSound();
        panel.Visible = false;
        cancelButton.Disabled = true;
        playButton.Disabled = true;
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
