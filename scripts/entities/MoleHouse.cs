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
    private int activeMolesCount = 0;
    private int maxActiveMoles = 4;
    private readonly object lockObject = new();
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
				mole.CorrectMoleAppeared += () => { lock (lockObject) { isCorrectMolePresent = true; correctMoleCount++; } };
                mole.CorrectMoleDisappeared += () => { lock (lockObject) { correctMoleCount--; if (correctMoleCount == 0) isCorrectMolePresent = false; } };
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
    /// Increments the score and updates the score label when a mole is hit.
    /// </summary>
    /// <param name="isCorrect">Indicates if the hit was on the correct mole.</param>
    public void UpdateScore(bool isCorrect)
    {
        score += isCorrect ? 1000 / Math.Max(1, (int)Math.Ceiling(timeElapsed)) : -250;
        scoreLabel.Text = score.ToString();
        timeElapsed = 0.0;
    }

    /// <summary>
    /// Returns whether a correct mole is currently visible.
    /// </summary>
    /// <returns>True if a correct mole is present, false otherwise.</returns>
    public bool IsCorrectMolePresent()
    {
        lock (lockObject)
        {
            return isCorrectMolePresent;
        }
    }

    public void ResetCorrectMoleCount()
    {
        lock (lockObject)
        {
            correctMoleCount = 0;
        }
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
        activeMolesCount = 0;
        PauseGame();
        ResumeGame();
    }

    /// <summary>
    /// Sets the maximum number of active moles allowed on the screen.
    /// </summary>
    /// <param name="max">The maximum number of active moles.</param>
    public void SetMaxActiveMoles(int max)
    {
        maxActiveMoles = max;
    }

    /// <summary>
    /// Checks if another mole can pop up based on the maximum limit.
    /// </summary>
    public bool CanMolePopUp()
    {
        lock (lockObject)
        {
            GD.Print($"Checking if mole can pop up: {activeMolesCount} < {maxActiveMoles}?");
            return activeMolesCount < maxActiveMoles;
        }
    }

    /// <summary>
    /// Increments the active mole count when a mole pops up.
    /// </summary>
    public void RegisterMoleAppearance()
    {
        lock (lockObject)
        {
            activeMolesCount++;
            GD.Print($"Mole appeared. Active moles: {activeMolesCount}/{maxActiveMoles}");
        }
    }

    /// <summary>
    /// Decrements the active mole count when a mole goes down.
    /// </summary>
    public void RegisterMoleDisappearance()
    {
        lock (lockObject)
        {
            activeMolesCount = Math.Max(0, activeMolesCount - 1);
            GD.Print($"Mole disappeared. Active moles: {activeMolesCount}/{maxActiveMoles}");
        }
    }
}
