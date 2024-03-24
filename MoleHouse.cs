using Godot;
using System;

public partial class MoleHouse : Node
{
	private Label scoreLabel;
	private int score = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
		scoreLabel.Text = "Score: " + score;

		foreach (Node child in GetChildren())
		{
			if (child is Mole mole)
			{
				mole.Connect(nameof(Mole.UpdateScoreEventHandler), new Callable(this, nameof(_on_mole_update_score)));
			}
		}
	}

	private void _on_mole_update_score()
	{
		GD.Print("Score update signal received");
		score += 1;
		scoreLabel.Text = "Score: " + score;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
