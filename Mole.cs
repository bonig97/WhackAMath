using Godot;
using System;

/// <summary>
/// Represents a mole in the Whack-A-Math game.
/// Handles the behavior of the mole popping up and being hit.
/// </summary>
public partial class Mole : Area2D
{
	/// <summary>
	/// Event triggered when the mole is hit.
	/// </summary>
	public event Action MoleHit;

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

	/// <summary>
	/// Initialization method called when the node enters the scene tree.
	/// Sets up the mole and hides it by default.
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
		timer.WaitTime = random.Next(3, 5);
		SetProcessInput(true);
		initialPosition = GlobalPosition;
		Connect("input_event", new Callable(this, nameof(OnInputEvent)));
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		panel = GetNode<Panel>("Panel");
		label = GetNode<Label>("Panel/Label");

		if (sprite == null)
		{
			GD.Print("Sprite is null");
		}

		GD.Randomize();
		MoveDown(); // Ensure the mole starts in a hidden state.
	}

	// Regular update loop for frame-dependent behavior.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Timer event that randomly decides whether to make the mole hittable (pop up) or not (hide).
	/// </summary>
	private void OnTimerTimeout()
	{
		int randInt = random.Next(0, 10);
		if (randInt > 5 && !isHittable)
		{
			isHittable = true;
			MoveUp();
		}
		else if (randInt <= 5 && isHittable)
		{
			isHittable = false;
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
		label.Text = random.Next(1, 10).ToString();
		sprite.Play("rising");
		timer.Start(); // Start the timer to schedule next hide.
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
	}

	/// <summary>
	/// Input event handler to detect mouse clicks on the mole, invoking the hit event.
	/// </summary>
	private void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && isHittable && isMouseInside)
		{
			isHittable = false;
			MoveDown();
			MoleHit?.Invoke();
		}
	}

	// Detects when the mouse cursor enters the mole's area.
	private void OnMouseEntered()
	{
		isMouseInside = true;
	}

	// Detects when the mouse cursor exits the mole's area.
	private void OnMouseExited()
	{
		isMouseInside = false;
	}
}
