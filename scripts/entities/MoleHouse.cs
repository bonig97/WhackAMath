using Godot;
using System;

/// <summary>
/// Manages the mole interactions and score in the Whack-A-Math game. It updates the score based on mole hits
/// and manages the game state related to mole presence.
/// </summary>
public partial class MoleHouse : Node
{
	private Label scoreLabel;
	private double timeElapsed = 0.0;
	private int score = 0;
	private bool isCorrectMolePresent = false;
	private int correctMoleCount = 0;
	private bool paused;

	/// <summary>
	/// Initializes the MoleHouse, setting up the score label and connecting events for mole interactions.
	/// </summary>
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScorePanel/ScoreLabel");
		scoreLabel.Text = score.ToString();
		paused = false;

		// Connect events for each mole in the scene to manage score and mole presence.
		foreach (Node child in GetChildren())
		{
			if (child is Mole mole)
			{
				mole.CorrectMoleAppeared += () => { 
					lock (this) {
						isCorrectMolePresent = true;
						correctMoleCount++; 
					}
					GD.Print("Correct Mole Count: " + correctMoleCount.ToString());
				};
				mole.CorrectMoleDisappeared += () => { 
					lock (this) {
						correctMoleCount--;
						if (correctMoleCount <= 0) 
							isCorrectMolePresent = false; 
					}
					GD.Print("Correct Mole Count: " + correctMoleCount.ToString());
				};
			}
		}
	}

	public override void _Process(double delta)
	{
		if (isCorrectMolePresent && !paused)
		{
			timeElapsed += delta;
		}
	}

	/// <summary>
	/// Updates the score based on the correctness of the mole hit. Correct hits increase the score,
	/// while incorrect hits decrease it.
	/// </summary>
	/// <param name="correct">True if the mole hit was correct, false otherwise.</param>
	public void UpdateScore(bool correct)
	{
		if (correct)
		{
			// Increase score for correct hits.
			score += 1000 / Math.Max(1, (int)Math.Ceiling(timeElapsed));
		}
		else
		{
			// Decrease score for incorrect hits.
			score -= 250;
			score = Math.Max(0, score);
		}
		scoreLabel.Text = score.ToString();
		timeElapsed = 0.0;
	}

	/// <summary>
	/// Returns whether a correct mole is currently visible.
	/// </summary>
	/// <returns>True if a correct mole is present, false otherwise.</returns>
	public bool IsCorrectMolePresent()
	{
		return isCorrectMolePresent;
	}

	/// <summary>
	/// Returns the current score.
	/// </summary>
	/// <returns>Current score in the game.</returns>
	public int GetScore()
	{
		return score;
	}

	/// <summary>
	/// Pauses the game, affecting all moles in the scene.
	/// </summary>
	public void PauseGame()
	{
		paused = true;
		foreach (Node child in GetChildren())
		{
			if (child is Mole mole)
			{
				mole.PauseGame();
			}
		}
	}

	/// <summary>
	/// Resumes the game, affecting all moles in the scene.
	/// </summary>
	public void ResumeGame()
	{
		paused = false;
		foreach (Node child in GetChildren())
		{
			if (child is Mole mole)
			{
				mole.ResumeGame();
			}
		}
	}

	/// <summary>
	/// Resets the game state, including score and mole presence.
	/// </summary>
	public void ResetGame()
	{
		score = 0;
		scoreLabel.Text = $"Score: {score}";
		isCorrectMolePresent = false;
		correctMoleCount = 0;
		timeElapsed = 0.0;
		PauseGame();
		ResumeGame();
	}

	public void ResetCorrectMoleCount()
	{
		correctMoleCount = 0;
	}
}
