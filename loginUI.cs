using Godot;
using System;

/// <summary>
/// 
/// </summary>
public partial class LoginPage : Control
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
        // Create the input fields
        emailInput = GetNode<LineEdit>("EmailInput");
        passwordInput = GetNode<LineEdit>("PasswordInput");

        // Create the login button
        loginButton = GetNode<Button>("LoginButton");
        // loginButton.Connect("pressed", this, nameof(OnLoginButtonPressed));

        // Create the "Sign Up" text
        signUpLabel = GetNode<Label>("SignUpLabel");

        // signUpLabel.Connect("gui_input", this, nameof(OnSignUpLabelClicked));

        // Load the SignUpPage scene
        // signUpScene = (PackedScene)ResourceLoader.Load("res://signupUI.tscn");        
    }

    private void OnLoginButtonPressed()
    {
        string email = emailInput.Text;
        string password = passwordInput.Text;

        // Implement your login logic here
        AuthenticateUser(email, password);
    }

    // private void OnSignUpLabelClicked(InputEvent inputEvent)
    // {
    //     if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == (int)ButtonList.Left)
    //     {
    //         // Switch to the sign up page
    //         SwitchToSignUpPage();
    //     }
    // }

    // private void SwitchToSignUpPage()
    // {
    //     // Create a new instance of the SignUpPage scene
    //     Node signUpPage = signUpScene.Instance();

    //     // Add the SignUpPage to the scene tree
    //     GetTree().Root.AddChild(signUpPage);

    //     // Remove the LoginPage from the scene tree
    //     QueueFree();
    // }

    private void AuthenticateUser(string email, string password)
    {
        // Your authentication logic goes here
        // For example, you could make an API call to a server
        // and check the response to determine if the login is successful
    }
}