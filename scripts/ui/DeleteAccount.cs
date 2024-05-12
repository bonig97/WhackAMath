using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

/// <summary>
/// Manages the account deletion process for the Whack-A-Math game.
/// </summary>
public partial class DeleteAccount : Control
{
	private LineEdit emailInput;
	private LineEdit passwordInput;
	private Label errorLabel;
	private Button deleteAccountButton;
	private Button backButton;

	/// <summary>
	/// Initializes the delete account UI and connects UI elements to their respective handlers.
	/// </summary>
	public override void _Ready()
	{
		emailInput = GetNode<LineEdit>("VBoxContainer/Control/EmailInput");
		passwordInput = GetNode<LineEdit>("VBoxContainer/Control/PasswordInput");
		errorLabel = GetNode<Label>("VBoxContainer/Control/ErrorLabel");

		deleteAccountButton = GetNode<Button>("VBoxContainer/Control/DeleteAccountButton");
		deleteAccountButton.Connect("pressed", new Callable(this, nameof(OnDeleteAccountButtonPressed)));

		backButton = GetNode<Button>("BackButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	private async void OnDeleteAccountButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		string email = emailInput.Text;
		string password = passwordInput.Text;

		try
		{
			await FirestoreHelper.DeleteUserAsync(email, password);
			GD.Print("Account deleted successfully.");
			ChangeScene("res://scenes/UI/loginUI.tscn");
		}
		catch (FirebaseAuthException e)
		{
			string errorMessage = e.Reason.ToString();
			errorLabel.Text = errorMessage.Contains("Undefined") ? "- Network Error" : "- Invalid Email or Password";
		}
	}

	private void OnBackButtonPressed()
	{
		AudioManager.Singleton?.PlayCancelSound();
		ChangeScene("res://scenes/UI/settingsUI.tscn");
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
