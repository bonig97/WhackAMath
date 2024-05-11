using Godot;
using System;

public partial class AudioManager : Node
{
    private AudioStreamPlayer buttonSoundPlayer;
    private AudioStreamPlayer sliderSoundPlayer;

    public static AudioManager Singleton { get; private set; }

    public override void _Ready()
    {
        Singleton = this;
        buttonSoundPlayer = new AudioStreamPlayer();
        sliderSoundPlayer = new AudioStreamPlayer();
        AddChild(buttonSoundPlayer);
        AddChild(sliderSoundPlayer);

        buttonSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/tap.wav");
        sliderSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/popup.wav");
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
}
