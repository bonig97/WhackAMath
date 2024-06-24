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

	private Button languageButton;
	private Button showPasswordButton;
	private Label showPasswordLabel;
	private bool isPasswordShown;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		emailInput = GetNode<LineEdit>("EmailInput");
		passwordInput = GetNode<LineEdit>("PasswordInput");
		confirmPasswordInput = GetNode<LineEdit>("ConfirmPasswordInput");
		signUpButton = GetNode<Button>("signUpButton");
		goToLoginButton = GetNode<Button>("goToLoginButton");
		errorLabel = GetNode<Label>("ErrorLabel");
		languageButton = GetNode<Button>("LanguageButton");
		showPasswordButton = GetNode<Button>("ShowPasswordButton");
		showPasswordLabel = GetNode<Label>("ShowPasswordLabel");

		signUpButton.Connect("pressed", new Callable(this, nameof(OnSignUpButtonPressedAsync)));
		goToLoginButton.Connect("pressed", new Callable(this, nameof(OnGoToLoginButtonPressed)));
		languageButton.Connect("pressed", new Callable(this, nameof(OnLanguageButtonPressed)));
		showPasswordButton.Connect("pressed", new Callable(this, nameof(OnShowPasswordButtonPressed)));

		isPasswordShown = false;
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

	private void ChangeScene(string scenePath)
	{
		PackedScene scene = (PackedScene)ResourceLoader.Load(scenePath);
		GetTree().ChangeSceneToPacked(scene);
	}

	private void OnLanguageButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();
		PackedScene signupScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/signupUI.tscn");
		SaveFile.PrevScene = signupScene;
		ChangeScene("res://scenes/UI/languageUI.tscn");
	}

	private void OnShowPasswordButtonPressed()
	{
		AudioManager.Singleton?.PlayButtonSound();

		if(!isPasswordShown)
		{
			isPasswordShown = true;
			showPasswordLabel.Text = "Hide password";
			passwordInput.Secret = false;
		}
		else
		{
			isPasswordShown = false;
			showPasswordLabel.Text = "Show password";
			passwordInput.Secret = true;
		}
	}
}
