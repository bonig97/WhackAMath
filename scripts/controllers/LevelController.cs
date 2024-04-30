using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Enum to represent the different mathematical operations.
/// </summary>
public enum MathOperation
{
	Add,
	Subtract,
	Multiply,
	Divide
}

/// <summary>
/// Controls the logic for generating and managing level questions,
/// including operations, range, and interaction with moles for answer selection.
/// </summary>
public partial class LevelController : Node
{
	private Panel levelCompletePanel;
	private Button levelCompleteButton;
	private Label levelCompleteStars;
	private Label levelCompleteScore;
	private Button pauseButton;
	private Button resumeButton;
	private Button quitButton;
	private MathOperation operation;
	private int minRange, maxRange; // Range of numbers for question generation, depends on difficulty.
	private int correctAnswer; // Stores the correct answer to the current question.
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
		levelCompleteStars = GetNode<Label>("LevelCompletePanel/LevelCompleteStars");
		levelCompleteScore = GetNode<Label>("LevelCompletePanel/LevelCompleteScore");
		levelCompletePanel.Visible = false;
		levelCompleteButton.Connect("pressed", new Callable(this, nameof(OnLevelCompleteButtonPressed)));
		levelCompleteButton.Disabled = true;
		moleHouse = GetNode<MoleHouse>("MoleHouse");
		pauseButton = GetNode<Button>("PauseButton");
		pauseButton.Connect("pressed", new Callable(this, nameof(OnPauseButtonPressed)));
		resumeButton = GetNode<Button>("GamePausePanel/ResumeButton");
		quitButton = GetNode<Button>("GamePausePanel/QuitButton");
		resumeButton.Connect("pressed", new Callable(this, nameof(OnResumeButtonPressed)));
		quitButton.Connect("pressed", new Callable(this, nameof(OnQuitButtonPressed)));
		ReadQuestionFormat("data/levels/AddLevelEasy.txt");
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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

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
	
		if (questionsAnswered<10) {
			if (isCorrect) 
			{
				questionsAnswered += 1;
				correctAnswer = GenerateQuestion();
				SetMoleAnswers();
			}
		} else {

			for (int i = 0; i < moleHouse.GetChildCount(); i++) {
				if (moleHouse.GetChild(i) is Mole mole) 
				{
					moleList[i].SetActive(false);
					
		   		}
			}

			levelCompletePanel.Visible = true;
			levelCompleteButton.Disabled = false;
			levelCompleteScore.Text = "Score: " + moleHouse.GetScore();
			if (moleHouse.GetScore() < 1000)
			{
				levelCompleteStars.Text = "☆☆☆";
			}
			else if (moleHouse.GetScore() < 2500)
			{
				levelCompleteStars.Text = "★☆☆";
			}
			else if (moleHouse.GetScore() < 5000)
			{
				levelCompleteStars.Text = "★★☆";
			}
			else
			{
				levelCompleteStars.Text = "★★★";
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

		if (moleHouse.IsCorrectMolePresent())
		{
			// Randomly select an invisible mole to set the correct answer.
			var correctMole = invisibleMoles[random.Next(invisibleMoles.Count)];
			correctMole.SetAnswer(correctAnswer, true);
		}
		

		// Set random answers to the rest of the moles.
		foreach (var mole in invisibleMoles)
		{
			if(!mole.GetCorrectness()) //if mole is not correct, set a random incorrect answer 
			{
				int randomAnswer = GenerateRandomAnswer(); 
				if(randomAnswer == correctAnswer) {
					mole.SetAnswer(randomAnswer+1,false);
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
	private int GenerateRandomAnswer()
	{
		switch (operation)
		{
			case MathOperation.Add:
				return random.Next(minRange + minRange, maxRange + maxRange + 1);
			case MathOperation.Subtract:
				return random.Next(minRange - maxRange, maxRange - minRange + 1);
			case MathOperation.Multiply:
				return random.Next(minRange * minRange, maxRange * maxRange + 1);
			case MathOperation.Divide:
				int dividend = random.Next(minRange, maxRange + 1);
				int divisor = random.Next(minRange, maxRange + 1);
				return dividend / Math.Max(1, divisor); // Avoid division by zero.
			default:
				throw new InvalidOperationException("Unknown operation.");
		}
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
				answer = x + y;
				DisplayQuestion($"{x} + {y} = ?");
				return answer;
			case MathOperation.Subtract:
				answer = x - y;
				DisplayQuestion($"{x} - {y} = ?");
				return answer;
			case MathOperation.Multiply:
				answer = x * y;
				DisplayQuestion($"{x} * {y} = ?");
				return answer;
			case MathOperation.Divide:
				answer = x / y;
				DisplayQuestion($"{x} / {y} = ?");
				return answer;
			default:
				throw new InvalidOperationException("Unknown operation.");
		}
	}
	private void OnLevelCompleteButtonPressed()
	{
		GD.Print("Level complete button pressed!");
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
	}
	
}

