using Godot;
using System;



public partial class Mole : Area2D
{

	// Declare member variables here. Examples:
	Signal update_score;
	Timer timer;
	CollisionShape2D collision_shape;
	AnimatedSprite2D sprite;
	Tween tween = new Tween();
	int bonk_height = 50;
	float ease_value = 0.5f;
	int rand_int;
	bool hittable = false;
	bool mouse_in = false;
	Vector2 init_pos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", new Callable(this, "_on_Timer_timeout"));
		timer.Start();
		init_pos = GlobalPosition;
		Connect("update_score", new Callable(this, GetParent().Name + "_on_Mole_update_score"));
		collision_shape = GetNode<CollisionShape2D>("CollisionShape2D");
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

	public void _on_Timer_timeout()
	{
		rand_int = new Random().Next(0, 10);
		if (rand_int > 5 && !hittable)
		{
			hittable = true;
			move_up();
		}
		else if (rand_int <= 5 && hittable)
		{
			hittable = false;
			move_down();
		}
		
	}

	private void move_up()
	{
		collision_shape.Disabled = false;
		//tween.TweenProperty(this, "global_position", new Vector2(sprite.GlobalPosition.X, sprite.GlobalPosition.Y - bonk_height), 0.5f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		timer.Start();
		sprite.Visible = true;
		sprite.Play("rising");
		sprite.Pause();
		sprite.Play("new_animation");
	}

	private void move_down()
	{
		collision_shape.Disabled = true;
		//tween.TweenProperty(this, "global_position", new Vector2(sprite.GlobalPosition.X, sprite.GlobalPosition.Y + bonk_height), 0.5f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		timer.Start();
		sprite.Pause();
		sprite.Play("hiding");
		sprite.Visible = false;
	}

	private void _on_input_event(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton && hittable && mouse_in)
		{
			hittable = false;
			move_down();
			EmitSignal("update_score");
		}
	}

	private void _on_mouse_entered()
	{
		mouse_in = true;
	}


	private void _on_mouse_exited()
	{
		mouse_in = false;
	}

}






