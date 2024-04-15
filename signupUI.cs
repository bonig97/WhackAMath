using Godot;
using System;
using System.Threading.Tasks;
using WhackAMath;
using Firebase.Auth;

public partial class signupUI : Control
{
	// Input Fields
	private LineEdit emailInput;
	private LineEdit passwordInput;
	private LineEdit confirmPasswordInput;

	// Sign Up Button
	private Button signUpButton;
	private Button goToLoginButton;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Sets up the database environment
		//FirestoreHelper.SetEnvironmentVariable();

		// Create the login button
		emailInput = GetNode<LineEdit>("EmailInput");
		passwordInput = GetNode<LineEdit>("PasswordInput");
		confirmPasswordInput = GetNode<LineEdit>("ConfirmPasswordInput");
		signUpButton = GetNode<Button>("signUpButton");
		signUpButton.Connect("pressed", new Callable(this, nameof(OnSignUpButtonPressedAsync)));
		goToLoginButton = GetNode<Button>("goToLoginButton");
		goToLoginButton.Connect("pressed", new Callable(this, nameof(OnGoToLoginButtonPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private async void OnSignUpButtonPressedAsync()
	{
		string email = emailInput.Text;
		string password = passwordInput.Text;
		string confirmPassword = confirmPasswordInput.Text;

		GD.Print("Email: " + email);

		if (password != confirmPassword)
		{
			// Display an error message
			GD.Print("Passwords do not match");
		}
		else
		{
			//Implement your login logic here
			try
			{
				UserCredential user = await FirestoreHelper.CreateUser(email, password);
				if (user != null)
				{
					GD.Print("User created successfully");
					PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://loginUI.tscn");
					GetTree().ChangeSceneToPacked(mainScene);
				}
				else
				{
					GD.Print("User creation failed");
				}
			}
			catch (FirebaseAuthException e)
			{
				GD.Print("Error: " + e.Reason);
			}
			
			
			
		}
	}

	private void OnGoToLoginButtonPressed()
	{
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://loginUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}
}
