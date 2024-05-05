using Godot;
using System;

/// <summary>
/// Manages the difficulty of the game by adjusting the complexity, speed, and distractors based on player performance.
/// </summary>
public class DifficultyManager
{
    // The accuracy threshold above which the game difficulty will increase.
    private const float accuracyThreshold = 0.90f;

    // The speed threshold below which the game will increase complexity.
    private const float speedThreshold = 5.0f;

    // The streak threshold for consistent correct answers to increase distractors.
    private const int consistencyThreshold = 5;

    // The progression threshold to move to the next level.
    private const int progressionThreshold = 10;

    // Tracks the current accuracy based on the player's performance.
    private float currentAccuracy = 0f;

    // Tracks the average speed of answers over time.
    private float currentSpeed = 0f;

    // Tracks the number of consecutive correct answers.
    private int currentStreak = 0;

    // Tracks the player's level progression.
    private int levelProgression = 0;

    // The total number of questions answered by the player.
    private int totalQuestionsAnswered = 0;

    // The total time taken by the player to answer all questions.
    private float totalResponseTime = 0f;

    /// <summary>
    /// Updates the difficulty metrics when a question is answered, adjusting the game's difficulty accordingly.
    /// </summary>
    /// <param name="isCorrect">Whether the player's answer was correct.</param>
    /// <param name="timeTaken">The time taken by the player to answer the question.</param>
    public void OnQuestionAnswered(bool isCorrect, float timeTaken)
    {
        UpdateAccuracy(isCorrect);
        UpdateSpeed(timeTaken);
        UpdateConsistency(isCorrect);
        UpdateLevelProgression();
        AdjustDifficulty();
    }

    /// <summary>
    /// Updates the current accuracy based on the player's response.
    /// </summary>
    /// <param name="isCorrect">Whether the player's answer was correct.</param>
    private void UpdateAccuracy(bool isCorrect)
    {
        totalQuestionsAnswered++;
        if (isCorrect) currentStreak++;
        currentAccuracy = (float)currentStreak / totalQuestionsAnswered;
    }

    /// <summary>
    /// Updates the average response speed based on the time taken to answer the latest question.
    /// </summary>
    /// <param name="timeTaken">The time taken to answer the question.</param>
    private void UpdateSpeed(float timeTaken)
    {
        totalResponseTime += timeTaken;
        currentSpeed = totalResponseTime / totalQuestionsAnswered;
    }

    /// <summary>
    /// Updates the consistency streak based on the correctness of the response.
    /// </summary>
    /// <param name="isCorrect">Whether the player's answer was correct.</param>
    private void UpdateConsistency(bool isCorrect)
    {
        currentStreak = isCorrect ? currentStreak + 1 : 0;
    }

    /// <summary>
    /// Updates the player's progress through the level.
    /// </summary>
    private void UpdateLevelProgression()
    {
        levelProgression++;
    }

    /// <summary>
    /// Adjusts the game's difficulty based on the current performance metrics.
    /// </summary>
    private void AdjustDifficulty()
    {
        if (currentAccuracy > accuracyThreshold) IncreaseQuestionComplexity();
        if (currentSpeed < speedThreshold) ReduceTimeConstraint();
        if (currentStreak >= consistencyThreshold) IncreaseDistractors();
        if (levelProgression >= progressionThreshold) MoveToHarderLevel();
    }

    /// <summary>
    /// Increases the complexity of questions presented to the player.
    /// </summary>
    private void IncreaseQuestionComplexity()
    {
        // TODO: Implement logic to increase the complexity of questions
    }

    /// <summary>
    /// Reduces the amount of time available to answer questions.
    /// </summary>
    private void ReduceTimeConstraint()
    {
        // TODO: Implement logic to reduce the time available to respond
    }

    /// <summary>
    /// Increases the number of incorrect distractors presented to the player.
    /// </summary>
    private void IncreaseDistractors()
    {
        // TODO: Implement logic to increase the number of distractors
    }

    /// <summary>
    /// Moves to a harder level, increasing the overall difficulty of the game.
    /// </summary>
    private void MoveToHarderLevel()
    {
        // TODO: Implement logic for moving to higher levels of difficulty
    }
}
