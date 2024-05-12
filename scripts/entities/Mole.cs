using Godot;
using System;
using System.Data;
using WhackAMath;

/// <summary>
/// Represents a mole in the Whack-A-Math game that handles behavior of popping up and being hit.
/// </summary>
public partial class Mole : Area2D
{
	/// <summary>
	/// Event fired when the mole is hit.
	/// </summary>
	public event Action<bool> MoleHit;

	/// <summary>
	/// Event fired to signal a switch in answers among the moles.
	/// </summary>
	public event Action SwitchAnswers;
	public event Action CorrectMoleAppeared;
	public event Action CorrectMoleDisappeared;

	private Timer timer;
	private Timer timer2;
	private CollisionShape2D collisionShape;
	private AnimatedSprite2D sprite, spriteChoice0, spriteChoice1, spriteChoice2, spriteChoice3, spriteChoice4, spriteChoice5;
	private Label label;
	private Panel panel;
	private Tween tween = new();
	private const int BonkHeight = 50;
	private readonly Random random = new();
	private bool isHittable = false;
	private bool isMouseInside = false;
	private Vector2 initialPosition;
	private bool isCorrect = false;
	private bool isActive = true;

	/// <summary>
	/// Called when the node enters the scene tree for the first time to set up initial values and states.
	/// </summary>
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		timer2 = GetNode<Timer>("Timer2");
		timer2.OneShot = true;
		timer.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		timer2.Connect("timeout", new Callable(this, nameof(OnTimerTimeout)));
		timer.Start();
		timer2.Start();
		SetProcessInput(true);
		initialPosition = GlobalPosition;
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		sprite = GetNode<AnimatedSprite2D>("Chicken");
		spriteChoice0 = GetNode<AnimatedSprite2D>("RealMole");
		spriteChoice1 = GetNode<AnimatedSprite2D>("Pig");
		spriteChoice2 = GetNode<AnimatedSprite2D>("Trunk");
		spriteChoice3 = GetNode<AnimatedSprite2D>("Bunny");
		spriteChoice4 = GetNode<AnimatedSprite2D>("Duck");
		spriteChoice5 = GetNode<AnimatedSprite2D>("Chicken");
		panel = GetNode<Panel>("Panel");
		label = GetNode<Label>("Panel/Label");

		switch (SaveFile.moleSelected)
		{
			case 0:
				sprite = spriteChoice0;
				break;
			case 1:
				sprite = spriteChoice1;
				break;
			case 2:
				sprite = spriteChoice2;
				break;
			case 3:
				sprite = spriteChoice3;
				break;
			case 4:
				sprite = spriteChoice4;
				break;
			case 5:
				sprite = spriteChoice5;
				break;
			default:
				break;
		}

		GD.Randomize();
		MoveDown();
	}

	/// <summary>
	/// Logic to execute every frame. Can be used to implement frame-based logic.
	/// </summary>
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// Determines the mole's behavior each time the timer times out. It either pops up or hides the mole.
	/// </summary>
	private void OnTimerTimeout()
	{
		int randInt = random.Next(0, 10);
		isHittable = randInt > 2;
		if (isHittable && isActive)
		{
			MoveUp();
		}
		else
		{
			MoveDown();
		}
	}

	/// <summary>
	/// Moves the mole up and makes it visible and hittable.
	/// </summary>
	private void MoveUp()
	{
		collisionShape.Disabled = false;
		sprite.Visible = true;
		panel.Visible = true;
		timer.Start();
		if (isCorrect)
		{
			CorrectMoleAppeared?.Invoke();
		}
		sprite.Play("default");
	}

	/// <summary>
	/// Hides the mole by moving it down, making it invisible and not hittable.
	/// </summary>
	private void MoveDown()
	{
		collisionShape.Disabled = true;
		sprite.Visible = false;
		panel.Visible = false;
		SwitchAnswers?.Invoke();
		if (isCorrect)
		{
			CorrectMoleDisappeared?.Invoke();
		}
	}

	/// <summary>
	/// Called when the mole receives an input event, such as a mouse click.
	/// </summary>
	private void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && isHittable && isMouseInside)
		{
			isHittable = false;
			MoveDown();
			AudioManager.Singleton.PlayHitMoleSound();
			GD.Print(label.Text + $" = {Convert.ToInt32(new DataTable().Compute(label.Text, null))}  {isCorrect}");
			MoleHit?.Invoke(isCorrect);
		}
	}

	/// <summary>
	/// Called when the mouse enters the collision shape of the mole.
	/// </summary>
	private void OnMouseEntered()
	{
		isMouseInside = true;
	}

	/// <summary>
	/// Called when the mouse exits the collision shape of the mole.
	/// </summary>
	private void OnMouseExited()
	{
		isMouseInside = false;
	}

	/// <summary>
	/// Checks if the mole is currently hittable.
	/// </summary>
	/// <returns>True if the mole is hittable, false otherwise.</returns>
	public bool IsHittable()
	{
		return isHittable;
	}

	/// <summary>
	/// Sets the answer displayed by the mole and specifies if it is correct.
	/// </summary>
	/// <param name="answer">The numeric answer to display.</param>
	/// <param name="isCorrect">A flag indicating whether the answer is correct.</param>
	public void SetAnswer(string answer, bool isCorrect)
	{
		this.isCorrect = isCorrect;
		label.Text = answer;
	}

	public void RecomputeCorrectness(int answer)
	{
		bool oldIsCorrect = isCorrect;
		isCorrect = Convert.ToInt32(new DataTable().Compute(label.Text, null)) == answer;
		if (isCorrect && !oldIsCorrect)
		{
			CorrectMoleAppeared?.Invoke();
		}
		else if (!isCorrect && oldIsCorrect)
		{
			CorrectMoleDisappeared?.Invoke();
		}
	}

	public void SetActive(bool activity)
	{
		if (!activity)
		{
			MoveDown();
		}
		this.isActive = activity;
	}

	public bool GetCorrectness()
	{
		return isCorrect;
	}
	public void PauseGame()
	{
		timer.Stop();
		timer2.Stop();
		label.Visible = false;
		sprite.Pause();
	}
	public void ResumeGame()
	{
		timer.Start();
		timer2.Start();
		label.Visible = true;
		sprite.Play();
	}
}
