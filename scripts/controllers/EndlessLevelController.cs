using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using WhackAMath;

/// <summary>
/// Enum to represent the different mathematical operations.
/// </summary>


/// <summary>
/// Controls the logic for generating and managing level questions,
/// including operations, range, and interaction with moles for answer selection.
/// </summary>
public partial class EndlessLevelController : Node
{
	private Panel levelCompletePanel;
	private Panel gameOverPanel;
	private Button levelCompleteButton;
	private Label levelCompleteStars;
	private Label levelCompleteScore;
	private Button pauseButton;
	private Button restartButton;
	private Button resumeButton;
	private Button quitButton;
	private MathOperation operation;

	private float remainingTime = 30f; // Set the initial time to 60 seconds

	private int minRange, maxRange; // Range of numbers for question generation, depends on difficulty.
	private int correctAnswer; // Stores the correct answer to the current question.
	private string correctAnswerText; // Stores the correct answer text to the current question.
	private int questionsAnswered = 0; //Keeps track of how many questions have been answered.
	private MoleHouse moleHouse; // Reference to the MoleHouse node.
	private List<Mole> moleList; // List to keep track of mole instances.
	private readonly Random random = new();

	/// <summary>
	/// Initializes the controller by reading the question format, generating a question, and setting up moles.
	/// </summary>
	public override void _Ready()
	{
		levelCompletePanel = GetNode<Panel>("LevelCompletePanel");
		levelCompleteButton = GetNode<Button>("LevelCompletePanel/LevelCompleteButton");
		levelCompleteScore = GetNode<Label>("LevelCompletePanel/LevelCompleteScore");
		levelCompletePanel.Visible = false;
		levelCompleteButton.Connect("pressed", new Callable(this, nameof(OnLevelCompleteButtonPressed)));
		levelCompleteButton.Disabled = true;
		moleHouse = GetNode<MoleHouse>("MoleHouse");
		pauseButton = GetNode<Button>("PauseButton");
		pauseButton.Connect("pressed", new Callable(this, nameof(OnPauseButtonPressed)));
		resumeButton = GetNode<Button>("GamePausePanel/ResumeButton");
		quitButton = GetNode<Button>("GamePausePanel/QuitButton");
		restartButton = GetNode<Button>("LevelCompletePanel/RestartButton");
		resumeButton.Connect("pressed", new Callable(this, nameof(OnResumeButtonPressed)));
		quitButton.Connect("pressed", new Callable(this, nameof(OnQuitButtonPressed)));
		restartButton.Connect("pressed", new Callable(this, nameof(OnRestartButtonPressed)));
		ReadQuestionFormat($"data/levels/{SaveFile.currentLevel}.txt");
		correctAnswer = GenerateQuestion();
		moleList = new List<Mole>();

		// Initialize moles and subscribe to their answer switching event.
		for (int i = 0; i < moleHouse.GetChildCount(); i++)
		{
			if (moleHouse.GetChild(i) is Mole mole)
			{
				moleList.Add(mole);
				moleList[i].SwitchAnswers += SetMoleAnswers;
				moleList[i].MoleHit += UpdateQuestion;
			}
		}

		SetMoleAnswers();
		TimerUI(); // initialize the timer UI
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		updateTimer(delta);
	}

	/*
		Summary:
		......
	*/
	private void updateTimer(double delta)
	{
		remainingTime -= (float)delta;
		if (remainingTime <= 0f)
		{
			// Game over condition, handle as needed
			moleHouse.PauseGame();
			GD.Print("Game Over!");
			// Show the game over panel
			levelCompletePanel.Visible = true;
			levelCompleteButton.Disabled = false;
			// Display the final score
			var scoreLabel = GetNode<Label>("LevelCompletePanel/ScoreLabel");
			levelCompleteScore.Text = $"Final Score: {moleHouse.GetScore()}";
		}
		else
		{
			// Update the timer UI
			TimerUI();
		}
	}

	/*
		Summary:  

	*/
	private void TimerUI()
	{
		var timerLabel = GetNode<Label>("TimerLabel");
		timerLabel.Text = $"Time: {Mathf.CeilToInt(remainingTime)}";
	}
	/// <summary>
	/// Reads question format from a specified file. The file specifies the operation and range for the questions.
	/// </summary>
	/// <param name="filePath">Path to the question format file.</param>
	private void ReadQuestionFormat(string filePath)
	{
		try
		{
			string[] lines = File.ReadAllLines(filePath);

			foreach (string line in lines)
			{
				var parts = line.Split(':');
				switch (parts[0].Trim())
				{
					case "operation":
						operation = Enum.Parse<MathOperation>(parts[1].Trim(), true);
						break;
					case "range":
						var rangeParts = parts[1].Trim().Split('-');
						minRange = int.Parse(rangeParts[0]);
						maxRange = int.Parse(rangeParts[1]);
						break;
				}
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to read question format from {filePath}: {ex.Message}");
		}
	}

	/// <summary>
	/// Displays the generated question on the UI.
	/// </summary>
	/// <param name="questionText">The text of the question to display.</param>
	private void DisplayQuestion(string questionText)
	{
		var questionLabel = GetNode<Label>("QuestionPanel/QuestionLabel");
		questionLabel.Text = questionText;
	}

	private void UpdateQuestion(bool isCorrect) {
		GD.Print($"{isCorrect}");
		if (isCorrect)
		{
			GD.Print($"{isCorrect}");
			questionsAnswered += 1;
			moleHouse.UpdateScore();
			correctAnswer = GenerateQuestion();
			SetMoleAnswers();
			for (int i = 0; i < moleHouse.GetChildCount(); i++) {
				if (moleHouse.GetChild(i) is Mole mole)
				{
					if (moleList[i].IsHittable())
					{
						moleList[i].RecomputeCorrectness(correctAnswer);
					}
				}
			}
			//reward the user with moretime for every correct answer
			remainingTime += 10f;
			// Check if all questions in the current level have been answered
			if (questionsAnswered >= 10)
			{
				questionsAnswered = 0; // Reset the questions answered count

				// Read the question format for the next level
				SaveFile.currentLevel += 1;
				ReadQuestionFormat($"data/levels/{SaveFile.currentLevel}.txt");

				// Generate a new question and set mole answers
				correctAnswer = GenerateQuestion();
				SetMoleAnswers();
			}
		}
	}

	/// <summary>
	/// Assigns correct and incorrect answers to the moles randomly.
	/// </summary>
		private void SetMoleAnswers()
	{
		var invisibleMoles = moleList.Where(mole => !mole.IsHittable()).ToList();

		if (invisibleMoles.Count == 0) return; // No invisible moles to set answers for.

		for (int i = 0; i < invisibleMoles.Count(); i++) {
			if (invisibleMoles[i].GetCorrectness())
			{
				// Remove the invisible mole from the list
				invisibleMoles.RemoveAt(i);
			}
		}

		if (!moleHouse.IsCorrectMolePresent())
		{
			// Randomly select an invisible mole to set the correct answer.
			var correctMole = invisibleMoles[random.Next(invisibleMoles.Count)];
			correctMole.SetAnswer(correctAnswerText, true);
		}

		// Set random answers to the rest of the moles.
		foreach (var mole in invisibleMoles)
		{
			if(!mole.GetCorrectness()) //if mole is not correct, set a random incorrect answer
			{
				string randomAnswer = GenerateRandomAnswer();
				int randomAnswerInt = Convert.ToInt32(new DataTable().Compute(randomAnswer, null));

				if(randomAnswerInt == correctAnswer) {
					mole.SetAnswer(randomAnswer,true);
				} else {
					mole.SetAnswer(randomAnswer, false);
				}
			}
		}
	}

	/// <summary>
	/// Generates a random incorrect answer based on the operation and the range.
	/// </summary>
	/// <returns>A randomly generated incorrect answer.</returns>
	private string GenerateRandomAnswer()
	{
		int x;
		int y;
		string answer = "";
		switch (operation)
		{
			case MathOperation.Add:
				x = random.Next(minRange, maxRange);
				y = random.Next(1 , maxRange - x + 1);
				answer = $"{x} + {y}";
				break;
			case MathOperation.Subtract:
				x = random.Next(minRange, maxRange);
				y = random.Next(1 , maxRange - x + 1);
				answer = $"{Math.Max(x,y)} - {Math.Min(x,y)}";
				break;
			case MathOperation.Multiply:
				x = random.Next(minRange, maxRange + 1);
				y = random.Next(1, 13);
				answer = $"{x} * {y}";
				break;
			case MathOperation.Divide:
				x = random.Next(minRange, maxRange + 1);
				y = random.Next(1, 13);
				int a = x * y;
				answer = $"{a} / {x}";
				break;
			default:
				throw new InvalidOperationException("Unknown operation.");
		}
		GD.Print(correctAnswerText);
		return answer;
	}

	/// <summary>
	/// Generates a new question based on the operation and range, displaying it to the player.
	/// </summary>
	/// <returns>The correct answer to the generated question.</returns>
	private int GenerateQuestion()
	{
		int x = random.Next(minRange, maxRange + 1);
		int y = random.Next(minRange, maxRange + 1);
		int answer = 0;

		switch (operation)
		{
			case MathOperation.Add:
				x = random.Next(minRange, maxRange);
				y = random.Next(1 , maxRange - x + 1);
				answer = x + y;
				DisplayQuestion($"? = {answer}");
				correctAnswerText = $"{x} + {y}";
				return answer;
			case MathOperation.Subtract:
				x = random.Next(minRange, maxRange);
				y = random.Next(1 , maxRange - x + 1);
				answer = Math.Max(x,y) - Math.Min(x,y);
				DisplayQuestion($"? = {answer}");
				correctAnswerText = $"{Math.Max(x,y)} - {Math.Min(x,y)}";
				return answer;
			case MathOperation.Multiply:
				x = random.Next(minRange, maxRange + 1);
				y = random.Next(1, 13);
				answer = x * y;
				DisplayQuestion($"? = {answer}");
				correctAnswerText = $"{x} * {y}";
				return answer;
			case MathOperation.Divide:
				x = random.Next(minRange, maxRange + 1);
				y = random.Next(1, 13);
				int a = x * y;
				answer = y;
				DisplayQuestion($"? = {answer}");
				correctAnswerText = $"{a} / {x}";
				//Future possibilty, use DisplayQuestion($"{a} / {y} = ?"); for equivalent fraction questions
				return answer;
			default:
				throw new InvalidOperationException("Unknown operation.");
		}
	}
	private void OnLevelCompleteButtonPressed()
	{
		GD.Print("Level complete button pressed!");
		if (moleHouse.GetScore()>1000 && SaveFile.currentLevel == SaveFile.MaxLevelUnlocked)
		{
			SaveFile.UpdateMaxLevelUnlocked(SaveFile.MaxLevelUnlocked+1);
		}
		PackedScene levelSelect = (PackedScene)ResourceLoader.Load("res://scenes/UI/mainUI.tscn");
		GetTree().ChangeSceneToPacked(levelSelect);
	}
	private void OnPauseButtonPressed()
	{
		moleHouse.PauseGame();
		GetNode<Panel>("GamePausePanel").Visible = true;
	}
	private void OnResumeButtonPressed()
	{
		GetNode<Panel>("GamePausePanel").Visible = false;
		moleHouse.ResumeGame();
	}
	private void OnQuitButtonPressed()
	{
		GD.Print("Quit button pressed!");
		PackedScene levelSelect = (PackedScene)ResourceLoader.Load("res://scenes/UI/levelSelectUI.tscn");
		GetTree().ChangeSceneToPacked(levelSelect);
	}

	private void OnRestartButtonPressed()
	{
		GD.Print("Restart button pressed!");
		// Read the question format for the next level
		SaveFile.currentLevel = 1;

		PackedScene endless = (PackedScene)ResourceLoader.Load("res://scenes/levels/EndlessLevel.tscn");
		GetTree().ChangeSceneToPacked(endless);
	}
}

