using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

public partial class ChangePassword : Control
{
	private LineEdit emailInput;
	private LineEdit oldPasswordInput;
	private LineEdit newPasswordInput;
	private LineEdit confirmPasswordInput;
	private Label errorLabel;
	private Button changePasswordButton;
	private Button backButton;

	public override void _Ready()
	{
		InitializeUI();
		ConnectSignals();
	}

	private void InitializeUI()
	{
		emailInput = GetNode<LineEdit>("VBoxContainer/Control/EmailInput");
		oldPasswordInput = GetNode<LineEdit>("VBoxContainer/Control/OldPasswordInput");
		newPasswordInput = GetNode<LineEdit>("VBoxContainer/Control/NewPasswordInput");
		confirmPasswordInput = GetNode<LineEdit>("VBoxContainer/Control/ConfirmNewPasswordInput");
		errorLabel = GetNode<Label>("VBoxContainer/Control/ErrorLabel");
		changePasswordButton = GetNode<Button>("VBoxContainer/Control/ChangePasswordButton");
		backButton = GetNode<Button>("BackButton");
	}

	private void ConnectSignals()
	{
		changePasswordButton.Connect("pressed", new Callable(this, nameof(OnChangePasswordButtonPressed)));
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	private async void OnChangePasswordButtonPressed()
	{
		AudioManager.Singleton?.PlayConfirmSound();
		string email = emailInput.Text;
		string oldPassword = oldPasswordInput.Text;
		string newPassword = newPasswordInput.Text;
		string confirmPassword = confirmPasswordInput.Text;

		if (newPassword != confirmPassword)
		{
			errorLabel.Text = "Passwords do not match";
			AudioManager.Singleton?.PlayCancelSound();
		}
		else if (newPassword == oldPassword)
		{
			errorLabel.Text = "- Password must be different from old password";
			AudioManager.Singleton?.PlayCancelSound();
		}
		else
		{
			try
			{
				await FirestoreHelper.ChangePassword(email, oldPassword, newPassword);
				ChangeScene("res://scenes/UI/mainUI.tscn");
			}
			catch (FirebaseAuthException e)
			{
				HandleFirebaseAuthException(e);
			}
		}
	}

	private void OnBackButtonPressed()
	{
		AudioManager.Singleton?.PlayCancelSound();
		ChangeScene("res://scenes/UI/settingsUI.tscn");
	}

	private void HandleFirebaseAuthException(FirebaseAuthException e)
	{
		string errorMessage = e.Reason.ToString();
		switch (errorMessage)
		{
			case "InvalidEmailAddress":
				errorLabel.Text = "- Invalid email";
				break;
			case "WeakPassword":
				errorLabel.Text = "- Password must be at least 6 characters long";
				break;
			case "MissingPassword":
				errorLabel.Text = "- Missing password";
				break;
			case "UserNotFound":
				errorLabel.Text = "- User not found";
				break;
			case "InvalidPassword":
				errorLabel.Text = "- Invalid password";
				break;
			case "Undefined":
				errorLabel.Text = "- Network Error";
				break;
			default:
				errorLabel.Text = "- Incorrect email or password";
				break;
		}
		AudioManager.Singleton?.PlayCancelSound();
	}

	private void ChangeScene(string scenePath)
	{
		PackedScene scene = (PackedScene)ResourceLoader.Load(scenePath);
		GetTree().ChangeSceneToPacked(scene);
	}
}
