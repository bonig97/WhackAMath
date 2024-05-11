using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

public partial class ChangePassword : Control
{
	// Input Fields
	private LineEdit emailInput;
	private LineEdit oldPasswordInput;
	private LineEdit newPasswordInput;
	private LineEdit confirmPasswordInput;
	private Label errorLabel;
	private Button changePasswordButton;
	private Button backButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create the input fields
		emailInput = GetNode<LineEdit>("VBoxContainer/Control/EmailInput");
		oldPasswordInput = GetNode<LineEdit>("VBoxContainer/Control/OldPasswordInput");
		newPasswordInput = GetNode<LineEdit>("VBoxContainer/Control/NewPasswordInput");
		confirmPasswordInput = GetNode<LineEdit>("VBoxContainer/Control/ConfirmNewPasswordInput");
		errorLabel = GetNode<Label>("VBoxContainer/Control/ErrorLabel");

		// Create the login button
		changePasswordButton = GetNode<Button>("VBoxContainer/Control/ChangePasswordButton");
		changePasswordButton.Connect("pressed", new Callable(this, nameof(OnChangePasswordButtonPressed)));

		// Create the "Back" button
		backButton = GetNode<Button>("BackButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnChangePasswordButtonPressed()
	{
		string email = emailInput.Text;
		string oldPassword = oldPasswordInput.Text;
		string newPassword = newPasswordInput.Text;
		string confirmPassword = confirmPasswordInput.Text;

		if (newPassword != confirmPassword)
		{
			errorLabel.Text = "Passwords do not match";
		}
		else
		{
			try
			{
				if (newPassword != confirmPassword)
				{
					errorLabel.Text = "- Passwords do not match";
				}
				else if (newPassword == oldPassword)
				{
					errorLabel.Text = "- Password must be different from old password";
				}
				else
				{
					await FirestoreHelper.ChangePassword(email, oldPassword, newPassword);
					PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/mainUI.tscn");
					GetTree().ChangeSceneToPacked(mainScene);
				}
			}
			catch (FirebaseAuthException e)
			{
				string errorMessage = e.Reason.ToString();
				if (errorMessage.Contains("InvalidEmailAddress"))
				{
					errorLabel.Text = "- Invalid email";
				}
				else if (errorMessage.Contains("WeakPassword"))
				{
					errorLabel.Text = "- Password must be at least 6 characters long";
				}
				else if (errorMessage.Contains("MissingPassword"))
				{
					errorLabel.Text = "- Missing password";
				}
				else if (errorMessage.Contains("UserNotFound"))
				{
					errorLabel.Text = "- User not found";
				}
				else if (errorMessage.Contains("InvalidPassword"))
				{
					errorLabel.Text = "- Invalid password";
				}
				else if (errorMessage.Contains("Undefined"))
				{
					errorLabel.Text = "- Network Error";
				}
				else
				{
					errorLabel.Text = "- Incorrect email or password";
				}
			}
		}
	}

	private void OnBackButtonPressed()
	{
		PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/settingsUI.tscn");
		GetTree().ChangeSceneToPacked(mainScene);
	}
}
