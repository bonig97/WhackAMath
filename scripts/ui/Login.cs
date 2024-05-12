using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

/// <summary>
/// Login page for the Whack-A-Math game.
/// </summary>
public partial class Login : Control
{
	// Input Fields
	private LineEdit emailInput;
	private LineEdit passwordInput;
	private Label errorLabel;

	// Login Button
	private Button loginButton;

	// "Sign Up" Text
	private Button goToSignUpButton;
	private LinkButton forgetPasswordButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Initialize UI elements
		emailInput = GetNode<LineEdit>("EmailInput");
		passwordInput = GetNode<LineEdit>("PasswordInput");
		errorLabel = GetNode<Label>("ErrorLabel");

		// Initialize buttons
		loginButton = GetNode<Button>("LoginButton");
		loginButton.Connect("pressed", new Callable(this, nameof(OnLoginButtonPressed)));

		goToSignUpButton = GetNode<Button>("GoToSignUpButton");
		goToSignUpButton.Connect("pressed", new Callable(this, nameof(OnSignUpButtonPressed)));

		forgetPasswordButton = GetNode<LinkButton>("ForgetPasswordButton");
		forgetPasswordButton.Connect("pressed", new Callable(this, nameof(OnForgetPasswordButtonPressed)));
	}

	private async void OnLoginButtonPressed()
	{
		string email = emailInput.Text;
		string password = passwordInput.Text;

		try
		{
			UserCredential user = await FirestoreHelper.AuthenticateUser(email, password);
			if (user != null)
			{
				SaveFile.LoadSaveFile();
				GD.Print("User logged in successfully");
				AudioManager.Singleton?.PlayConfirmSound();
				ChangeScene("res://scenes/UI/mainUI.tscn");
			}
		}
		catch (FirebaseAuthException e)
		{
			GD.Print($"Error: {e.Reason}");
			string errorMessage = e.Reason.ToString();
			errorLabel.Text = errorMessage.Contains("Undefined") ? "- Network Error" : "- Invalid Email or Password";
			AudioManager.Singleton?.PlayCancelSound();
		}
	}

	private void OnSignUpButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/signupUI.tscn");
	}

	private void OnForgetPasswordButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		ChangeScene("res://scenes/UI/forgetPasswordUI.tscn");
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
