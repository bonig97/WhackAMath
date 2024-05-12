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

	// Sign Up and Login Buttons
	private Button signUpButton;
	private Button goToLoginButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		emailInput = GetNode<LineEdit>("EmailInput");
		passwordInput = GetNode<LineEdit>("PasswordInput");
		confirmPasswordInput = GetNode<LineEdit>("ConfirmPasswordInput");
		signUpButton = GetNode<Button>("signUpButton");
		goToLoginButton = GetNode<Button>("goToLoginButton");
		errorLabel = GetNode<Label>("ErrorLabel");

		signUpButton.Connect("pressed", new Callable(this, nameof(OnSignUpButtonPressedAsync)));
		goToLoginButton.Connect("pressed", new Callable(this, nameof(OnGoToLoginButtonPressed)));
	}

	private async void OnSignUpButtonPressedAsync()
	{
		AudioManager.Singleton?.PlayButtonSound();
		string email = emailInput.Text;
		string password = passwordInput.Text;
		string confirmPassword = confirmPasswordInput.Text;

		if (password != confirmPassword)
		{
			errorLabel.Text = "Passwords do not match";
		}
		else
		{
			try
			{
				UserCredential user = await FirestoreHelper.CreateUser(email, password);
				if (user != null)
				{
					await FirestoreHelper.CreateDocument(SaveFile.ConvertToDictionary());
					PackedScene mainScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
					AudioManager.Singleton?.PlayConfirmSound();
					GetTree().ChangeSceneToPacked(mainScene);
				}
			}
			catch (FirebaseAuthException e)
			{
				AudioManager.Singleton?.PlayCancelSound();
				HandleFirebaseAuthException(e);
			}
		}
	}

	private void OnGoToLoginButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
		GetTree().ChangeSceneToPacked(loginScene);
	}

	private void HandleFirebaseAuthException(FirebaseAuthException e)
	{
		string errorMessage = e.Reason.ToString();

		errorLabel.Text = errorMessage switch
		{
			var message when message.Contains("EmailExists") => "- Email already in use",
			var message when message.Contains("InvalidEmailAddress") => "- Invalid email",
			var message when message.Contains("WeakPassword") => "- Password must be at least 6 characters long",
			var message when message.Contains("MissingPassword") => "- Missing password",
			_ => "- Connection error",
		};
	}
}
