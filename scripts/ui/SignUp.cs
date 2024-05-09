using Godot;
using System;
using System.Threading.Tasks;
using WhackAMath;
using Firebase.Auth;

public partial class SignUp : Control
{
	// Input Fields
	private LineEdit emailInput;
	private LineEdit passwordInput;
	private LineEdit confirmPasswordInput;
	private Label errorLabel;

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
		errorLabel = GetNode<Label>("ErrorLabel");
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

		if (password != confirmPassword)
		{
			// Display an error message
			errorLabel.Text = "Passwords do not match";
		}
		else
		{
			//Implement your login logic here
			try
			{
				UserCredential user = await FirestoreHelper.CreateUser(email, password);
				if (user != null)
				{
					await FirestoreHelper.CreateDocument(SaveFile.ConvertToDictionary());
					GD.Print("User created successfully");
					PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
					GetTree().ChangeSceneToPacked(mainScene);
				}
			}
			catch (FirebaseAuthException e)
			{
				string errorMessage = e.Reason.ToString();
				if (errorMessage.Contains("EmailExists"))
				{
					errorLabel.Text = "- Email already in use";
				}
				else if (errorMessage.Contains("InvalidEmailAddress"))
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
				else
				{
					errorLabel.Text = "- Connection error";
				}
			}
			
			
			
		}
	}

	private void OnGoToLoginButtonPressed()
	{
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}
}
