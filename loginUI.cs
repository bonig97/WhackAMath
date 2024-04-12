using Godot;
using System;

/// <summary>
/// Login page for the Whack-A-Math game.
/// </summary>
public partial class loginUI : Control
{
	// Input Fields
	private LineEdit emailInput;
	private LineEdit passwordInput;

	// Login Button
	private Button loginButton;

	// "Sign Up" Text
	private Button goToSignUpButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Create the input fields
		emailInput = GetNode<LineEdit>("EmailInput");
		passwordInput = GetNode<LineEdit>("PasswordInput");

		// Create the login button
		loginButton = GetNode<Button>("LoginButton");
		loginButton.Connect("pressed", new Callable(this, nameof(OnLoginButtonPressed)));

		// Create the "Sign Up" text
		goToSignUpButton = GetNode<Button>("GoToSignUpButton");
		goToSignUpButton.Connect("pressed", new Callable(this, nameof(OnSignUpButtonPressed)));

		// signUpLabel.Connect("gui_input", this, nameof(OnSignUpLabelClicked));

		// Load the SignUpPage scene
		// signUpScene = (PackedScene)ResourceLoader.Load("res://signupUI.tscn");  
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnLoginButtonPressed()
	{
		string email = emailInput.Text;
		string password = passwordInput.Text;

		// Implement your login logic here
		//await FirestoreHelper.AuthenticateUser(email, password);
		PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://mainUI.tscn");
		GetTree().ChangeSceneToPacked(mainScene);
	}

	private void OnSignUpButtonPressed()
	{
		PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://signupUI.tscn");
		GetTree().ChangeSceneToPacked(mainScene);
	}
}
