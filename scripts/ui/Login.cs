using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

/// <summary>
/// Login page for the Whack-A-Math game.
/// </summary>
public partial class Login : Control
{
    private LineEdit emailInput;
    private LineEdit passwordInput;
    private Label errorLabel;
    private Label showPasswordLabel;
    private Button loginButton;
    private Button goToSignUpButton;
    private LinkButton forgetPasswordButton;
    private Button languageButton;
    private Button showPasswordButton;
    private Mole showPasswordMole;
    private bool isPasswordShown;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Initialize UI elements
        emailInput = GetNode<LineEdit>("EmailInput");
        passwordInput = GetNode<LineEdit>("PasswordInput");
        errorLabel = GetNode<Label>("ErrorLabel");
        showPasswordLabel = GetNode<Label>("ShowPasswordLabel");
        isPasswordShown = false;

        // Initialize buttons
        languageButton = GetNode<Button>("LanguageButton");
        languageButton.Connect("pressed", new Callable(this, nameof(OnLanguageButtonPressed)));


        loginButton = GetNode<Button>("LoginButton");
        loginButton.Connect("pressed", new Callable(this, nameof(OnLoginButtonPressed)));

        goToSignUpButton = GetNode<Button>("GoToSignUpButton");
        goToSignUpButton.Connect("pressed", new Callable(this, nameof(OnSignUpButtonPressed)));

        forgetPasswordButton = GetNode<LinkButton>("ForgetPasswordButton");
        forgetPasswordButton.Connect("pressed", new Callable(this, nameof(OnForgetPasswordButtonPressed)));

        showPasswordButton = GetNode<Button>("ShowPasswordButton");
        showPasswordButton.Connect("pressed", new Callable(this, nameof(OnShowPasswordButtonPressed)));
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
                await SaveFile.LoadSaveFile();
                GD.Print("User logged in successfully");
                AudioManager.Singleton?.PlayConfirmSound();

                // Load user settings and play music
                float musicVolume = 0.5f;
                ConfigFile configFile = new ConfigFile();
                var err = configFile.Load("user://settings.cfg");
                if (err == Error.Ok)
                {
                    musicVolume = (float)configFile.GetValue("audio", "music_volume", 0.5);
                }

                AudioManager.Singleton.PlayMainMusic(musicVolume);

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

    private void OnLanguageButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
        PackedScene loginScene = (PackedScene)ResourceLoader.Load("res://scenes/UI/loginUI.tscn");
        SaveFile.prevScene = loginScene;
        ChangeScene("res://scenes/UI/languageUI.tscn");
    }

    private void OnShowPasswordButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();

        if (!isPasswordShown)
        {
            isPasswordShown = true;
            //set mole to go down
            showPasswordLabel.Text = "Hide password";
            passwordInput.Secret = false;
            GD.Print("button pressed");
            //change label text to "hide"
            //code for showing the text
        }
        else
        {
            isPasswordShown = false;
            showPasswordLabel.Text = "Show password";
            passwordInput.Secret = true;
            //make mole pop up
        }
    }
}
