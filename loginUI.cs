using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

/// <summary>
/// Login page for the Whack-A-Math game.
/// </summary>
public partial class loginUI : Control
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
		// Create the input fields
		emailInput = GetNode<LineEdit>("EmailInput");
		passwordInput = GetNode<LineEdit>("PasswordInput");
		errorLabel = GetNode<Label>("ErrorLabel");

		// Create the login button
		loginButton = GetNode<Button>("LoginButton");
		loginButton.Connect("pressed", new Callable(this, nameof(OnLoginButtonPressed)));

		// Create the "Sign Up" text
		goToSignUpButton = GetNode<Button>("GoToSignUpButton");
		goToSignUpButton.Connect("pressed", new Callable(this, nameof(OnSignUpButtonPressed)));

		// Create the "Forget Password" text
		forgetPasswordButton = GetNode<LinkButton>("ForgetPasswordButton");
		forgetPasswordButton.Connect("pressed", new Callable(this, nameof(OnForgetPasswordButtonPressed)));

		// signUpLabel.Connect("gui_input", this, nameof(OnSignUpLabelClicked));

		// Load the SignUpPage scene
		// signUpScene = (PackedScene)ResourceLoader.Load("res://signupUI.tscn");  
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private async void OnLoginButtonPressed()
	{
		string email = emailInput.Text;
		string password = passwordInput.Text;


		//Implement your login logic here
		try
		{
			UserCredential user = await FirestoreHelper.AuthenticateUser(email, password);
			
			if (user != null)
			{
				GD.Print("User logged in successfully");
				PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://mainUI.tscn");
				GetTree().ChangeSceneToPacked(mainScene);
			}
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
				errorLabel.Text = "- Invalid Email or Password";
			}
			
		}
	}

	private void OnSignUpButtonPressed()
	{
		PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://signupUI.tscn");
		GetTree().ChangeSceneToPacked(mainScene);
	}

	private void OnForgetPasswordButtonPressed()
	{
		PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://forgetPasswordUI.tscn");
		GetTree().ChangeSceneToPacked(mainScene);
	}
}
