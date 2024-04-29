using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class LevelController : Node
{
	private string operation; //Operation is one of the following: "Add", "Subtract", "Multiply", "Divide".
	private int minRange, maxRange; //Depends on the difficulty
	private int correctAnswer; //Correct answer to the question
	private MoleHouse moleHouse; //Mole house node
	//list of mole pointers
	private List<Mole> moleList;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		moleHouse = GetNode<MoleHouse>("MoleHouse");
		ReadQuestionFormat("AddLevelEasy.txt");
		correctAnswer = GenerateQuestion();
		moleList = new List<Mole>();
		//Get all moles
		for (int i = 0; i < moleHouse.GetChildCount(); i++) {
			if (moleHouse.GetChild(i) is Mole) {
				moleList.Add((Mole)moleHouse.GetChild(i));
				moleList[i].SwitchAnswers += SetMoleAnswers;
			}
		}
		SetMoleAnswers();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	//Reads question format from a file. File names are formulated as follows "OperationLevelDifficulty.txt"
	//Operation is one of the following: "Add", "Subtract", "Multiply", "Divide".
	//Difficulty is one of the following: "Easy", "Normal", "Hard".

	//Files are in this format:
	//operation: operation
	//range: minRange-maxRange

	private void ReadQuestionFormat(string filePath) {

		string [] lines = File.ReadAllLines(filePath); 
		
		foreach (string line in lines) {

			var parts = line.Split(':');
			if(parts[0].Trim() == "operation") {

				operation = parts[1].Trim();

			} else if (parts[0].Trim() == "range") {

				var rangeParts = parts[1].Trim().Split('-');
				minRange = int.Parse(rangeParts[0]);
				maxRange = int.Parse(rangeParts[1]);

			}
		}
	}

	private void DisplayQuestion(string questionText) {

		var questionLabel = GetNode<Label>("QuestionPanel/QuestionLabel");
		questionLabel.Text = questionText;

	}

	//Sets the correct answer to exactly one mole
	private void SetMoleAnswers() {

		//Randomly select an invisible mole to set the correct answer
		List<int> invisibleMoles = new List<int>();
		for (int i = 0; i < moleList.Count; i++) {
			if (!moleList[i].IsHittable()) {
				invisibleMoles.Add(i);
			}
		}

		//select an invisible mole to set the correct answer
		Random rnd = new Random();

		int moleIndex = invisibleMoles[rnd.Next(0, invisibleMoles.Count)];
		moleList[moleIndex].SetAnswer(correctAnswer, true);

		//Set random answers to the rest of the moles
		for (int i = 0; i < moleList.Count; i++) {
			if (i != moleIndex && !moleList[i].IsHittable()) {
				//temporary random answer until we figure out a proper way to set the answers
				if (operation == "add") {
					moleList[i].SetAnswer(rnd.Next(minRange+minRange, maxRange+maxRange + 1), false);
				} else if (operation == "subtract") {
					moleList[i].SetAnswer(rnd.Next(minRange - maxRange, maxRange-minRange + 1), false);
				} else if (operation == "multiply") {
					moleList[i].SetAnswer(rnd.Next(minRange*minRange, maxRange*maxRange + 1), false);
				} else if (operation == "divide") {
					moleList[i].SetAnswer(rnd.Next(minRange/maxRange, maxRange/minRange + 1), false);
				}
			}
		}



	}

	//Generates question based on information from the file
	private int GenerateQuestion() {
		 Random rnd = new Random();
		int x = rnd.Next(minRange, maxRange + 1);
		int y = rnd.Next(minRange, maxRange + 1);
		int answer = 0;

		 if (operation == "add")
		{
			answer = x + y;
			// Displays question to label
			DisplayQuestion($"{x} + {y} = ?");
		}

		return answer;


	}

	

	
}
