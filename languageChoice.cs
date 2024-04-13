using Godot;
using System;

public partial class languageChoice : Control
{
	
	private Button backButton;
	private Button englishButton;
	private Button italianButton;
	private Button arabicButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		englishButton = GetNode<Button>("EnglishButton");
		englishButton.Connect("pressed", new Callable(this, nameof(OnEnglishButtonPressed)));

		italianButton = GetNode<Button>("ItalianButton");
		italianButton.Connect("pressed", new Callable(this, nameof(OnItalianButtonPressed)));

		arabicButton = GetNode<Button>("ArabicButton");
		arabicButton.Connect("pressed", new Callable(this, nameof(OnArabicButtonPressed)));

		backButton = GetNode<Button>("BackButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBackButtonPressed()
	{
		//change this to go back to previous scene via global variable
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://mainUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}

	private void OnEnglishButtonPressed()
	{
		//change this to go to english language
		GD.Print("English Button Pressed");
	}

	private void OnItalianButtonPressed()
	{
		//change this to go to italian language
		GD.Print("Italian Button Pressed");
	}

	private void OnArabicButtonPressed()
	{
		//change this to go to arabic language
		GD.Print("Arabic Button Pressed");
	}
}
