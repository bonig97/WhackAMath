using Godot;
using System;

/// <summary>
/// MoleHouse class that manages the score and mole hit events in the Whack-A-Math game.
/// It updates the score whenever a mole is hit and ensures that the score label is accurate.
/// </summary>
public partial class MoleHouse : Node
{
	private Label scoreLabel;
	private int score = 0;

	/// <summary>
	/// Initializes the MoleHouse by setting up the score label and connecting to the MoleHit event for each mole.
	/// </summary>
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
		scoreLabel.Text = $"Score: {score}";

		// Subscribes to the MoleHit event for each Mole instance in the scene.
		foreach (Node child in GetChildren())
		{
			if (child is Mole mole)
			{
				mole.MoleHit += new Action<bool>(OnMoleHit); // Explicitly cast OnMoleHit to Action<bool>.
			}
		}
	}

	/// <summary>
	/// Increments the score and updates the score label when a mole is hit.
	/// </summary>
	private void OnMoleHit(bool hit)
	{
		score += 1;
		scoreLabel.Text = $"Score: {score}";
	}

	/// <summary>
	/// Unsubscribes from the MoleHit event for each Mole instance when the MoleHouse is removed from the scene tree.
	/// This is crucial to prevent memory leaks by ensuring that event subscriptions are properly cleaned up.
	/// </summary>
	public override void _ExitTree()
	{
		// Unsubscribes from the MoleHit event to clean up before the node is removed from the scene tree.
		foreach (Node child in GetChildren())
		{
			if (child is Mole mole)
			{
				mole.MoleHit -= OnMoleHit;
			}
		}
	}
}
