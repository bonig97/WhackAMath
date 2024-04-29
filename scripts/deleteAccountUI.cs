using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

public partial class deleteAccountUI : Control
{
	// Input Fields
	private LineEdit emailInput;
	private LineEdit passwordInput;
	private Label errorLabel;
	private Button deleteAccountButton;
	private Button backButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create the input fields
		emailInput = GetNode<LineEdit>("VBoxContainer/Control/EmailInput");
		passwordInput = GetNode<LineEdit>("VBoxContainer/Control/PasswordInput");
		errorLabel = GetNode<Label>("VBoxContainer/Control/ErrorLabel");

		// Create the login button
		deleteAccountButton = GetNode<Button>("VBoxContainer/Control/DeleteAccountButton");
		deleteAccountButton.Connect("pressed", new Callable(this, nameof(OnDeleteAccountButtonPressed)));

		// Create the "Back" button
		backButton = GetNode<Button>("BackButton");
		backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnDeleteAccountButtonPressed()
	{
		string email = emailInput.Text;
		string password = passwordInput.Text;
		//Implement your delete account logic here
		try
		{
			await FirestoreHelper.DeleteUserAsync(email, password);
			PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
			GetTree().ChangeSceneToPacked(mainScene);
		}
		catch (FirebaseAuthException e)
		{
			string errorMessage = e.Reason.ToString();
			if (errorMessage.Contains("Undefined"))
			{
				errorLabel.Text = "- Network Error";
			}
			else
			{
				GD.Print(e);
				errorLabel.Text = "- Invalid Email or Password";
			}
		}
	}

	private void OnBackButtonPressed()
	{
		PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/settingsUI.tscn");
		GetTree().ChangeSceneToPacked(mainScene);
	}
}
