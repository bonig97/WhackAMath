using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Numerics;
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
	private Label levelCompleteScore;
	private Button pauseButton;
	private Button restartButton;
	private Button resumeButton;
	private Button quitButton;

	private Label questionLabel;
	//private MathOperation operation;

	private float remainingTime = 30f; // Set the initial time to 60 seconds

	private int minRangeAddSub, maxRangeAddSub; // Range of numbers for question generation, depends on difficulty.
	private int minRangeMulDiv, maxRangeMulDiv;
	private int difficultyLevel;
	private int levelSelect;
	private MathOperation operation;
	private List<MathOperation> operations;
	private int correctAnswer; // Stores the correct answer to the current question.
	private string correctAnswerText; // Stores the correct answer text to the current question.
	private int questionsAnswered = 0; //Keeps track of how many questions have been answered.
	private MoleHouse moleHouse; // Reference to the MoleHouse node.
	private List<Mole> moleList; // List to keep track of mole instances.
	private readonly Random random = new();
	private const int levelIncrement = 3;
	private string question;
	private bool paused;

	/// <summary>
	/// Initializes the controller by reading the question format, generating a question, and setting up moles.
	/// </summary>
	public override void _Ready()
	{
		questionLabel = GetNode<Label>("QuestionPanel/QuestionLabel");
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
		
		moleList = new List<Mole>();

		minRangeAddSub = 0;
		maxRangeAddSub = 10;
		minRangeMulDiv = 1;
		maxRangeMulDiv = 10;

		operations = new List<MathOperation>();
		operation = MathOperation.Add;
		difficultyLevel = 1;
		levelSelect = difficultyLevel;
		// Read the question format for the first level
		ReadQuestionFormat($"data/levels/{levelSelect}.txt");
		correctAnswer = GenerateQuestion();
		paused = false;

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
		for (int i = 0; i < moleHouse.GetChildCount(); i++) {
			if (moleHouse.GetChild(i) is Mole mole)
			{
				if (moleList[i].IsHittable())
				{
					moleList[i].RecomputeCorrectness(correctAnswer);
				}
			}
		}
		questionLabel.Text = question;
	}

	/*
		Summary:
		......
	*/
	private void updateTimer(double delta)
	{
		if (paused) return;
		remainingTime -= (float)delta;
		if (remainingTime <= 0f)
		{
			// Game over condition, handle as needed
			moleHouse.PauseGame();
			GD.Print("Game Over!");
			// Show the game over panel
			levelCompletePanel.Visible = true;
			levelCompleteButton.Disabled = false;
			pauseButton.Disabled = true;
			// Display the final score
			var scoreLabel = GetNode<Label>("LevelCompletePanel/ScoreLabel");
			levelCompleteScore.Text = moleHouse.GetScore().ToString();
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
		timerLabel.Text = Mathf.CeilToInt(remainingTime).ToString();
	}
	/// <summary>
	/// Reads question format from a specified file. The file specifies the operation and range for the questions.
	/// </summary>
	/// <param name="filePath">Path to the question format file.</param>
	private void ReadQuestionFormat(string filePath)
	{
		int minRange = 0;
		int maxRange = 0;
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
						operations.Add(operation);
						break;
					case "range":
						var rangeParts = parts[1].Trim().Split('-');
						minRange = int.Parse(rangeParts[0]);
						maxRange = int.Parse(rangeParts[1]);

						if (operation == MathOperation.Add || operation == MathOperation.Subtract)
						{
							minRangeAddSub = minRange;
							maxRangeAddSub = maxRange;
						}
						else
						{
							minRangeMulDiv = minRange;
							maxRangeMulDiv = maxRange;
						}

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
		question = questionText;
	}

	private void UpdateQuestion(bool isCorrect) {
		questionsAnswered += 1;
		moleHouse.UpdateScore(isCorrect);
		moleHouse.ResetCorrectMoleCount();
		correctAnswer = GenerateQuestion();
		SetMoleAnswers();
		//reward the user with moretime for every correct answer
		float tempTime = remainingTime + 10f;
		if (tempTime <= 45f)
		{
			remainingTime = tempTime;
		}
		else
		{
			remainingTime = 45f;
		}

		// Check if all questions in the current level have been answered
		if (questionsAnswered >= 2)
		{
			questionsAnswered = 0; // Reset the questions answered count

			if (operations.Count() == 4 && difficultyLevel < 3)
			{
				operations.Clear();
				difficultyLevel++;
				levelSelect = difficultyLevel;
			}
			else if (levelSelect<12)
			{
				levelSelect+=levelIncrement;
			}

			ReadQuestionFormat($"data/levels/{levelSelect}.txt");

			// Generate a new question and set mole answers
			correctAnswer = GenerateQuestion();
			SetMoleAnswers();
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
				if (!moleHouse.IsCorrectMolePresent())
				{
					invisibleMoles[i].ForceArise();
				}
				// Remove the invisible mole from the list
				invisibleMoles.RemoveAt(i);
			}
		}

		if (!moleHouse.IsCorrectMolePresent())
		{
			// Randomly select an invisible mole to set the correct answer.
			//Force it to move up
			var correctMole = invisibleMoles[random.Next(invisibleMoles.Count)];
			correctMole.SetAnswer(correctAnswerText, true);
			correctMole.ForceArise();
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
		if (operations.Count == 0)
		{
			throw new InvalidOperationException("No operations available.");
		}
		int x;
		int y;
		int minRange = 0;
		int maxRange = 0;
		string answer = "";
		if (operation == MathOperation.Add || operation == MathOperation.Subtract)
		{
			minRange = minRangeAddSub;
			maxRange = maxRangeAddSub;
		}
		else
		{
			minRange = minRangeMulDiv;
			maxRange = maxRangeMulDiv;
		}

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
		return answer;
	}

	/// <summary>
	/// Generates a new question based on the operation and range, displaying it to the player.
	/// </summary>
	/// <returns>The correct answer to the generated question.</returns>
	private int GenerateQuestion()
	{
		lock (this)
		{
			
		
			MathOperation operation = operations[random.Next(operations.Count)];
			int minRange = 0;
			int maxRange = 0;
			if (operation == MathOperation.Add || operation == MathOperation.Subtract)
			{
				minRange = minRangeAddSub;
				maxRange = maxRangeAddSub;
			}
			else
			{
				minRange = minRangeMulDiv;
				maxRange = maxRangeMulDiv;
			}
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
					GD.Print(correctAnswerText);
					return answer;
				case MathOperation.Subtract:
					x = random.Next(minRange, maxRange);
					y = random.Next(1 , maxRange - x + 1);
					answer = Math.Max(x,y) - Math.Min(x,y);
					DisplayQuestion($"? = {answer}");
					correctAnswerText = $"{Math.Max(x,y)} - {Math.Min(x,y)}";
					GD.Print(correctAnswerText);
					return answer;
				case MathOperation.Multiply:
					x = random.Next(minRange, maxRange + 1);
					y = random.Next(1, 13);
					answer = x * y;
					DisplayQuestion($"? = {answer}");
					correctAnswerText = $"{x} * {y}";
					GD.Print(correctAnswerText);
					return answer;
				case MathOperation.Divide:
					x = random.Next(minRange, maxRange + 1);
					y = random.Next(1, 13);
					int a = x * y;
					answer = y;
					DisplayQuestion($"? = {answer}");
					correctAnswerText = $"{a} / {x}";
					GD.Print(correctAnswerText);
					//Future possibilty, use DisplayQuestion($"{a} / {y} = ?"); for equivalent fraction questions
					return answer;
				default:
					throw new InvalidOperationException("Unknown operation.");
			}
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
		paused = true;
		GetNode<Panel>("GamePausePanel").Visible = true;
	}
	private void OnResumeButtonPressed()
	{
		GetNode<Panel>("GamePausePanel").Visible = false;
		moleHouse.ResumeGame();
		paused = false;
	}
	private void OnQuitButtonPressed()
	{
		GD.Print("Quit button pressed!");
		PackedScene levelSelect = (PackedScene)ResourceLoader.Load("res://scenes/UI/mainUI.tscn");
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

