using Godot;
using System;

/// <summary>
/// Represents a mole in the Whack-A-Math game that handles behavior of popping up and being hit.
/// </summary>
public partial class Mole : Area2D
{
    /// <summary>
    /// Event fired when the mole is hit.
    /// </summary>
    public event Action<bool> MoleHit;

    /// <summary>
    /// Event fired to signal a switch in answers among the moles.
    /// </summary>
    public event Action SwitchAnswers;
    public event Action CorrectMoleAppeared;
    public event Action CorrectMoleDisappeared;

    private Timer timer;
    private Timer timer2;
    private CollisionShape2D collisionShape;
    private AnimatedSprite2D sprite;
    private Label label;
    private Panel panel;
    private Tween tween = new();
    private const int BonkHeight = 50;
    private readonly Random random = new();
    private bool isHittable = false;
    private bool isMouseInside = false;
    private Vector2 initialPosition;
    private bool isCorrect = false;
    private bool isActive = true;



    /// <summary>
    /// Called when the node enters the scene tree for the first time to set up initial values and states.
    /// </summary>
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        timer2 = GetNode<Timer>("Timer2");
        timer2.OneShot = true;
		timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		timer2.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
        timer.Start();
        timer2.Start();
        SetProcessInput(true);
        initialPosition = GlobalPosition;
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        panel = GetNode<Panel>("Panel");
        label = GetNode<Label>("Panel/Label");

        GD.Randomize();
        MoveDown(); // Ensure the mole starts hidden.
    }

    /// <summary>
    /// Logic to execute every frame. Can be used to implement frame-based logic.
    /// </summary>
    public override void _Process(double delta)
    {
    }

    /// <summary>
    /// Determines the mole's behavior each time the timer times out. It either pops up or hides the mole.
    /// </summary>
    private void OnTimerTimeout()
    {
        int randInt = random.Next(0, 10);
        isHittable = randInt > 5;
        if (isHittable && isActive)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }

    /// <summary>
    /// Moves the mole up and makes it visible and hittable.
    /// </summary>
    private void MoveUp()
    {
        collisionShape.Disabled = false;
        sprite.Visible = true;
        panel.Visible = true;
        sprite.Play("rising");
        timer.Start();
        CorrectMoleAppeared?.Invoke();
    }

    /// <summary>
    /// Hides the mole by moving it down, making it invisible and not hittable.
    /// </summary>
    private void MoveDown()
    {
        collisionShape.Disabled = true;
        sprite.Visible = false;
        panel.Visible = false;
        sprite.Play("hiding");
        SwitchAnswers?.Invoke();
        CorrectMoleDisappeared?.Invoke();
    }

    /// <summary>
    /// Called when the mole receives an input event, such as a mouse click.
    /// </summary>
    private void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && isHittable && isMouseInside)
        {
            isHittable = false;
            MoveDown();
            MoleHit?.Invoke(isCorrect);
        }
    }

    /// <summary>
    /// Called when the mouse enters the collision shape of the mole.
    /// </summary>
    private void OnMouseEntered()
    {
        isMouseInside = true;
    }

    /// <summary>
    /// Called when the mouse exits the collision shape of the mole.
    /// </summary>
    private void OnMouseExited()
    {
        isMouseInside = false;
    }

    /// <summary>
    /// Checks if the mole is currently hittable.
    /// </summary>
    /// <returns>True if the mole is hittable, false otherwise.</returns>
    public bool IsHittable()
    {
        return isHittable;
    }

    /// <summary>
    /// Sets the answer displayed by the mole and specifies if it is correct.
    /// </summary>
    /// <param name="answer">The numeric answer to display.</param>
    /// <param name="isCorrect">A flag indicating whether the answer is correct.</param>
    public void SetAnswer(int answer, bool isCorrect)
    {
        this.isCorrect = isCorrect;
        label.Text = answer.ToString();
    }

    public void SetActive(bool activity)
    {
        if (!activity) 
        {
            MoveDown();
        }
        this.isActive = activity;
    }

    public bool GetCorrectness() 
    {
        return isCorrect;
    }
}
