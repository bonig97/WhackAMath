using Godot;
using System;

public partial class Mole : Area2D
{
	[Signal]
	public delegate void UpdateScoreEventHandler();

	private Timer timer;
	private CollisionShape2D collisionShape;
	private AnimatedSprite2D sprite;
	private Tween tween = new();
	private const int BonkHeight = 50;
	private readonly Random random = new();
	private bool isHittable = false;
	private bool isMouseInside = false;
	private Vector2 initialPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", new Callable(this, nameof(_on_timer_timeout)));
		timer.Start();
		SetProcessInput(true);
		initialPosition = GlobalPosition;
		Connect("input_event", new Callable(this, nameof(_on_input_event)));
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (sprite == null)
		{
			GD.Print("sprite is null");
		}

		GD.Randomize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_timer_timeout()
	{
		int randInt = random.Next(0, 10);
		if (randInt > 5 && !isHittable)
		{
			isHittable = true;
			move_up();
		}
		else if (randInt <= 5 && isHittable)
		{
			isHittable = false;
			move_down();
		}
	}

	private void move_up()
	{
		collisionShape.Disabled = false;
		timer.Start();
		sprite.Visible = true;
		sprite.Play("rising");
	}

	private void move_down()
	{
		collisionShape.Disabled = true;
		timer.Start();
		sprite.Play("hiding");
		sprite.Visible = false;
	}

	private void _on_input_event(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && isHittable && isMouseInside)
		{
			GD.Print("Input event received");
			isHittable = false;
			move_down();
			EmitSignal(nameof(UpdateScoreEventHandler));
		}
	}

	private void _on_mouse_entered()
	{
		isMouseInside = true;
	}


	private void _on_mouse_exited()
	{
		isMouseInside = false;
	}
}
