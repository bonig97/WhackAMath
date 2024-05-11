using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

public partial class ForgetPassword : Control
{
	// Input Fields
	private LineEdit emailInput;
	private Label errorLabel;
	private Button resetPasswordButton;
	private Button backButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create the input fields
		emailInput = GetNode<LineEdit>("EmailInput");
		errorLabel = GetNode<Label>("ErrorLabel");

		// Create the login button
		resetPasswordButton = GetNode<Button>("ResetPasswordButton");
		resetPasswordButton.Connect("pressed", new Callable(this, nameof(OnResetPasswordButtonPressed)));

		// Create the "Back" button
		backButton = GetNode<Button>("BackButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnResetPasswordButtonPressed()
	{
		string email = emailInput.Text;

		try
		{
			await FirestoreHelper.SendPasswordResetEmailAsync(email);
			PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
			GetTree().ChangeSceneToPacked(mainScene);
		}
		catch (FirebaseAuthException e)
		{
			string errorMessage = e.Reason.ToString();
			if (errorMessage.Contains("InvalidEmailAddress"))
			{
				errorLabel.Text = "- Invalid email";
			}
			else
			{
				errorLabel.Text = "- Network error";
			}
		}
	}

	private void OnBackButtonPressed()
	{
		PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
		GetTree().ChangeSceneToPacked(mainScene);
	}
}
