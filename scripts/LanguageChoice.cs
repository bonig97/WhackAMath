using Godot;
using System;
using System.Globalization;
using System.Threading;
using WhackAMath;


public partial class LanguageChoice : Control
{
	private Button backButton;
	private Button englishButton;
	private Button italianButton;
	private Button arabicButton;

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

	private void OnBackButtonPressed()
	{
		AudioManager.Singleton?.PlayCancelSound();
		GetTree().ChangeSceneToPacked(SaveFile.prevScene);
	}

	private void OnEnglishButtonPressed()
	{
		AudioManager.Singleton?.PlayConfirmSound();
		SaveFile.UpdateLanguageSelected("English");
		GD.Print("English Button Pressed");
		Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
		// CultureInfo.CurrentUICulture = new CultureInfo("en");
	}

	private void OnItalianButtonPressed()
	{
		AudioManager.Singleton?.PlayConfirmSound();
		SaveFile.UpdateLanguageSelected("Italian");
		GD.Print("Italian Button Pressed");
		Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("it");	
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("it");
	}

	private void OnArabicButtonPressed()
	{
		AudioManager.Singleton?.PlayConfirmSound();
		SaveFile.UpdateLanguageSelected("Arabic");
		GD.Print("Arabic Button Pressed");
		Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ar");
		Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ar");
	}
}
