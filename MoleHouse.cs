using Godot;
using System;

public partial class MoleHouse : Node
{
	private Label score_label;
	private int score = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		score_label = GetNode<Label>("%ScoreLabel");
		score_label.Text = "Score: " + score;

		foreach (Node child in GetChildren())
		{
			if (child is Mole mole)
			{
				mole.Connect(nameof(Mole.UpdateScoreEventHandler), new Callable(this, nameof(_on_Mole_update_score)));
			}
		}
	}

	public void _on_Mole_update_score()
	{
		score += 1;
		score_label.Text = "Score: " + score;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
