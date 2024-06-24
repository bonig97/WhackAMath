using Godot;
using System;
using WhackAMath;

public partial class InitialStart : Node
{
    private Button OfflineModeButton;
    private Button OnlineModeButton;
    private Button LanguageButton;

    public static bool IsOfflineMode { get; private set; }
    public static IDataService DataService { get; private set; }

    public override void _Ready()
    {
        // Sets up the database environment
        FirestoreHelper.SetEnvironmentVariable();

        // Creates a default save file
        SaveFile.InitialSaveFile();

        OfflineModeButton = GetNodeSafe<Button>("OfflineModeButton");
        OnlineModeButton = GetNodeSafe<Button>("OnlineModeButton");
        LanguageButton = GetNodeSafe<Button>("LanguageButton");

        OfflineModeButton?.Connect("pressed", new Callable(this, nameof(OnOfflineModeButtonPressed)));
        OnlineModeButton?.Connect("pressed", new Callable(this, nameof(OnOnlineModeButtonPressed)));
        LanguageButton?.Connect("pressed", new Callable(this, nameof(OnLanguageButtonPressed)));
    }

    private T GetNodeSafe<T>(string path) where T : class
    {
        Node node = GetNode(path);
        if (node == null)
        {
            GD.PrintErr($"Node not found: {path}");
            return null;
        }
        return node as T;
    }

    private void OnOfflineModeButtonPressed()
    {
        AudioManager.Singleton?.PlayConfirmSound();
        IsOfflineMode = true;
        DataService = new OfflineDataService(GetTree());
        ChangeScene("res://scenes/UI/mainUI.tscn");
    }

    private void OnOnlineModeButtonPressed()
    {
        AudioManager.Singleton?.PlayConfirmSound();
        IsOfflineMode = false;
        DataService = FirestoreHelper.Instance;
        ChangeScene("res://scenes/UI/signupUI.tscn");
    }

    private void OnLanguageButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
        PackedScene initialScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/initial.tscn");
        SaveFile.PrevScene = initialScene;
        ChangeScene("res://scenes/UI/languageUI.tscn");
    }

    private void ChangeScene(string scenePath)
    {
        PackedScene scene = (PackedScene)ResourceLoader.Load(scenePath);
        GetTree().ChangeSceneToPacked(scene);
    }

    public override void _Process(double delta)
    {
    }
}
