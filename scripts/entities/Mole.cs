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

	/// <summary>
	/// Event fired when a correct mole appears.
	/// </summary>
	public event Action CorrectMoleAppeared;

	/// <summary>
	/// Event fired when a correct mole disappears.
	/// </summary>
	public event Action CorrectMoleDisappeared;

	private Timer timer;
	private Timer timer2;
	private CollisionShape2D collisionShape;
	private AnimatedSprite2D sprite, spriteChoice0, spriteChoice1, spriteChoice2, spriteChoice3, spriteChoice4, spriteChoice5;
	private Label label;
	private Panel panel;
	private const int BonkHeight = 50;
	private readonly Random random = new();
	private bool isHittable = false;
	private bool isMouseInside = false;
	private Vector2 initialPosition;
	private bool isCorrect = false;
	private bool isActive = true;
	private string answer;
	private Timer responseSoundTimer;
	private static readonly object lockObject = new object();

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

		responseSoundTimer = new Timer
		{
			OneShot = true,
			WaitTime = 0.2f
		};
		AddChild(responseSoundTimer);
		responseSoundTimer.Connect("timeout", new Callable(this, nameof(OnResponseSoundTimerTimeout)));

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
		label.Text = answer;
	}

	/// <summary>
	/// Determines the mole's behavior each time the timer times out. It either pops up or hides the mole.
	/// </summary>
	private void OnTimerTimeout()
	{
		var moleHouse = GetParent<MoleHouse>();

		if (moleHouse.CanMolePopUp())
		{
			int randInt = random.Next(0, 10);
			isHittable = randInt > 2;

			if (isHittable && isActive)
			{
				lock (lockObject)
				{
					moleHouse.RegisterMoleAppearance();
				}
				MoveUp();
			}
			else
			{
				MoveDown();
			}
		}
		else
		{
			MoveDown();
		}
	}

	/// <summary>
	/// Handles the delayed response sound for correct or incorrect mole hits.
	/// </summary>
	private void OnResponseSoundTimerTimeout()
	{
		if (isCorrect)
		{
			AudioManager.Singleton.PlayCorrectSound();
		}
		else
		{
			AudioManager.Singleton.PlayWrongSound();
		}
	}

	/// <summary>
	/// Moves the mole up and makes it visible and hittable.
	/// </summary>
	public void MoveUp()
	{
		if (!isActive)
		{
			return;
		}
		collisionShape.Disabled = false;
		sprite.Visible = true;
		panel.Visible = true;
		timer.Start();

		if (isCorrect)
		{
			lock (lockObject)
			{
				CorrectMoleAppeared?.Invoke();
			}
		}

		sprite.Play("default");
	}

	/// <summary>
	/// Hides the mole by moving it down, making it invisible and not hittable.
	/// </summary>
	private void MoveDown()
	{
		var moleHouse = GetParent<MoleHouse>();

		if (isHittable)
		{
			lock (lockObject)
			{
				moleHouse.RegisterMoleDisappearance();
			}
		}

		collisionShape.Disabled = true;
		sprite.Visible = false;
		panel.Visible = false;
		SwitchAnswers?.Invoke();

		if (isCorrect)
		{
			lock (lockObject)
			{
				CorrectMoleDisappeared?.Invoke();
			}
		}
	}

	/// <summary>
	/// Called when the mole receives an input event, such as a mouse click.
	/// </summary>
	private void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && isHittable && isMouseInside)
		{
			lock (lockObject)
			{
				MoleHit?.Invoke(isCorrect);
			}
			isHittable = false;
			MoveDown();

			AudioManager.Singleton.PlayHitMoleSound();

			responseSoundTimer.Start();
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
		answer = answer.Replace("*", "x");
		this.answer = answer;
	}

	/// <summary>
	/// Recalculates correctness of the mole based on the given answer.
	/// </summary>
	/// <param name="answer">The correct answer to validate against.</param>
	public void RecomputeCorrectness(int answer)
	{
		bool oldIsCorrect = isCorrect;
		string temp = label.Text;
		temp = temp.Replace("x", "*");
		isCorrect = Convert.ToInt32(new DataTable().Compute(temp, null)) == answer;

		if (isCorrect && !oldIsCorrect)
		{
			lock (lockObject)
			{
				CorrectMoleAppeared?.Invoke();
			}
			
		}
		else if (!isCorrect && oldIsCorrect)
		{
			lock (lockObject)
			{
				CorrectMoleDisappeared?.Invoke();
			}
		}
	}

	/// <summary>
	/// Sets the activity state of the mole, controlling its ability to be interacted with.
	/// </summary>
	/// <param name="activity">Whether the mole is active.</param>
	public void SetActive(bool activity)
	{
		if (!activity)
		{
			MoveDown();
		}

		isActive = activity;
	}

	/// <summary>
	/// Returns whether the mole's current answer is correct.
	/// </summary>
	/// <returns>True if the current answer is correct, otherwise false.</returns>
	public bool GetCorrectness()
	{
		return isCorrect;
	}

	/// <summary>
	/// Pauses mole activity, typically used when the game is paused.
	/// </summary>
	public void PauseGame()
	{
		timer.Stop();
		timer2.Stop();
		label.Visible = false;
		sprite.Pause();
	}

	/// <summary>
	/// Resumes mole activity, typically used when the game is resumed.
	/// </summary>
	public void ResumeGame()
	{
		timer.Start();
		timer2.Start();
		label.Visible = true;
		sprite.Play();
	}
	public void ForceArise()
	{
		var moleHouse = GetParent<MoleHouse>();
		if (moleHouse.CanMolePopUp())
		{
			lock (lockObject)
			{
				moleHouse.RegisterMoleAppearance();
				isHittable = true;
				MoveUp();
				GD.Print("Forced Arise");
			}
		}
		
	}
}
