using Godot;
using System;

/// <summary>
/// Login page for the Whack-A-Math game.
/// </summary>
public partial class SignupPage : Control
{
    // Input Fields
    private LineEdit emailInput;
    private LineEdit passwordInput;

    // Login Button
    private Button loginButton;

    // "Sign Up" Text
    private Label signUpLabel;

    public override void _Ready()
    {
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
        loginButton = new Button();
        loginButton.Text = "Login";
        // loginButton.Connect("pressed", this, nameof(OnLoginButtonPressed));
        AddChild(loginButton);

        // Create the "Already have an account? Sign up" text
        signUpLabel = new Label();
        signUpLabel.Text = "Already have an account? Sign up";
        AddChild(signUpLabel);
    }

    private void OnLoginButtonPressed()
    {
        string email = emailInput.Text;
        string password = passwordInput.Text;

        // Implement your login logic here
        AuthenticateUser(email, password);
    }

    private void AuthenticateUser(string email, string password)
    {
        // Your authentication logic goes here
        // For example, you could make an API call to a server
        // and check the response to determine if the login is successful
    }
}
