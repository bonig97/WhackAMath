using Godot;
using System;

public partial class AudioManager : Node
{
    private AudioStreamPlayer buttonSoundPlayer;
    private AudioStreamPlayer sliderSoundPlayer;
    private AudioStreamPlayer cancelSoundPlayer;
    private AudioStreamPlayer confirmSoundPlayer;

    public static AudioManager Singleton { get; private set; }

    public override void _Ready()
    {
        Singleton = this;

        buttonSoundPlayer = new AudioStreamPlayer();
        sliderSoundPlayer = new AudioStreamPlayer();
        cancelSoundPlayer = new AudioStreamPlayer();
        confirmSoundPlayer = new AudioStreamPlayer();

        AddChild(buttonSoundPlayer);
        AddChild(sliderSoundPlayer);
        AddChild(cancelSoundPlayer);
        AddChild(confirmSoundPlayer);

        buttonSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/tap.wav");
        buttonSoundPlayer.Bus = "Effects";

        sliderSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/popup.wav");
        sliderSoundPlayer.Bus = "Effects";

        cancelSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/cancel.wav");
        cancelSoundPlayer.Bus = "Effects";

        confirmSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/confirm.wav");
        confirmSoundPlayer.Bus = "Effects";
    }

    public void PlayButtonSound()
    {
        if (!buttonSoundPlayer.Playing)
        {
            buttonSoundPlayer.Play();
        }
    }

    public void PlaySliderSound(float volume)
    {
        if (!sliderSoundPlayer.Playing)
        {
            sliderSoundPlayer.VolumeDb = volume;
            sliderSoundPlayer.Play();
        }
    }

    public void PlayCancelSound()
    {
        if (!cancelSoundPlayer.Playing)
        {
            cancelSoundPlayer.Play();
        }
    }

    public void PlayConfirmSound()
    {
        if (!confirmSoundPlayer.Playing)
        {
            confirmSoundPlayer.Play();
        }
    }
}
