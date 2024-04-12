using Godot;
using System;
using WhackAMath;

/// <summary>
/// Login page for the Whack-A-Math game.
/// </summary>
public partial class SignupPage : Control
{
    // Input Fields
    private LineEdit emailInput;
    private LineEdit passwordInput;

    // Sign Up Button
    private Button signUpButton;

    // "Sign Up" Text
    private Label signUpLabel;

    public override void _Ready()
    {
        //Sets up the database environment
        FirestoreHelper.SetEnvironmentVariable();
        
        // Create the title label
        var titleLabel = new Label();
        titleLabel.Text = "Whack-A-Math";
        AddChild(titleLabel);

        // Create the input fields
        emailInput = new LineEdit();
        emailInput.PlaceholderText = "Email";
        AddChild(emailInput);

        passwordInput = new LineEdit();
        passwordInput.PlaceholderText = "Password";
        passwordInput.Secret = true;
        AddChild(passwordInput);

        // Create the login button
        signUpButton = new Button();
        signUpButton.Text = "Login";
        // loginButton.Connect("pressed", this, nameof(OnLoginButtonPressed));
        AddChild(signUpButton);

        // Create the "Already have an account? Sign up" text
        signUpLabel = new Label();
        signUpLabel.Text = "Already have an account? Login";
        AddChild(signUpLabel);
    }

    private async void OnSignUpButtonPressed()
    {
        string email = emailInput.Text;
        string password = passwordInput.Text;

        // Implement your login logic here
        await FirestoreHelper.CreateUser(email, password);
    }
}
