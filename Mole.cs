using Godot;
using System;

public partial class Mole : Area2D
{
	public event Action MoleHit;
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
		timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		timer.Start();
		SetProcessInput(true);
		initialPosition = GlobalPosition;
		Connect("input_event", new Callable(this, nameof(OnInputEvent)));
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (sprite == null)
		{
			GD.Print("Sprite is null");
		}

		GD.Randomize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

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

	private void MoveUp()
	{
		collisionShape.Disabled = false;
		timer.Start();
		sprite.Visible = true;
		sprite.Play("rising");
	}

	private void MoveDown()
	{
		collisionShape.Disabled = true;
		timer.Start();
		sprite.Play("hiding");
		sprite.Visible = false;
	}

	private void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && isHittable && isMouseInside)
		{
			isHittable = false;
			MoveDown();
			MoleHit?.Invoke();
		}
	}

	private void OnMouseEntered()
	{
		isMouseInside = true;
	}

	private void OnMouseExited()
	{
		isMouseInside = false;
	}
}
