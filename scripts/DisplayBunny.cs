using Godot;
using System;

public partial class DisplayBunny : Area2D
{
	// Declare member variables here. Examples:
	private AnimatedSprite2D animatedSprite;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite.Play("default");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public AnimatedSprite2D getSprite()
	{
		return animatedSprite;
	}
}
