using Godot;
using System;

public partial class LevelGrid : Control
{
	public event Action<int> LevelSelected;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is Button button)
			{
				button.Pressed += () => OnButtonPressed(button.Text.ToInt());
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnButtonPressed(int num)
	{
		GD.Print("Level Selected: ");
		GD.Print(num);
		LevelSelected?.Invoke(num);
	}
}
