using Godot;
using System;
using System.IO;

public partial class LevelController : Node
{
	private string operation; //Operation is one of the following: "Add", "Subtract", "Multiply", "Divide".
    private int minRange, maxRange; //Depends on the difficulty

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ReadQuestionFormat("AddLevelEasy.txt");
		GenerateQuestion();
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
	private void SetMoleAnswers(int correctAnswer) {


	}

	//Generates question based on information from the file
	private void GenerateQuestion() {
		 Random rnd = new Random();
        int x = rnd.Next(minRange, maxRange + 1);
        int y = rnd.Next(minRange, maxRange + 1);
        int answer;

		 if (operation == "add")
        {
            answer = x + y;
            // Displays question to label
            DisplayQuestion($"{x} + {y} = ?");
        }


	}

	

	
}
