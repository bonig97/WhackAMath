using Godot;
using System;
using WhackAMath;
using Firebase.Auth;

/// <summary>
/// Manages the password reset functionality for the Whack-A-Math game.
/// </summary>
public partial class ForgetPassword : Control
{
    private LineEdit emailInput;
    private Label errorLabel;
    private Button resetPasswordButton;
    private Button backButton;

    /// <summary>
    /// Initializes the password reset UI and connects UI elements to their respective handlers.
    /// </summary>
    public override void _Ready()
    {
        emailInput = GetNode<LineEdit>("EmailInput");
        errorLabel = GetNode<Label>("ErrorLabel");
        resetPasswordButton = GetNode<Button>("ResetPasswordButton");
        backButton = GetNode<Button>("BackButton");

        resetPasswordButton.Connect("pressed", new Callable(this, nameof(OnResetPasswordButtonPressed)));
        backButton.Connect("pressed", new Callable(this, nameof(OnBackButtonPressed)));
    }

    private async void OnResetPasswordButtonPressed()
    {
        AudioManager.Singleton?.PlayButtonSound();
        string email = emailInput.Text;

        try
        {
            await FirestoreHelper.SendPasswordResetEmailAsync(email);
            GD.Print("Reset email sent successfully.");
            ChangeScene("res://scenes/UI/loginUI.tscn");
        }
        catch (FirebaseAuthException e)
        {
            string errorMessage = e.Reason.ToString();
            errorLabel.Text = errorMessage.Contains("InvalidEmailAddress") ? "- Invalid email" : "- Network error";
        }
    }

    private void OnBackButtonPressed()
    {
        AudioManager.Singleton?.PlayCancelSound();
        ChangeScene("res://scenes/UI/loginUI.tscn");
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
