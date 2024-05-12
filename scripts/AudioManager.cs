using Godot;
using System;

/// <summary>
/// Manages all sound effects and music in the game.
/// </summary>
public partial class AudioManager : Node
{
    private AudioStreamPlayer buttonSoundPlayer;
    private AudioStreamPlayer sliderSoundPlayer;
    private AudioStreamPlayer cancelSoundPlayer;
    private AudioStreamPlayer confirmSoundPlayer;
    private AudioStreamPlayer hitMoleSoundPlayer;
    private AudioStreamPlayer mainMusicPlayer;

    public static AudioManager Singleton { get; private set; }

    /// <summary>
    /// Initializes the AudioManager and loads all necessary sound resources.
    /// </summary>
    public override void _Ready()
    {
        Singleton = this;

        InitializePlayers();
        LoadSounds();
        AssignAudioBuses();
    }

    private void InitializePlayers()
    {
        buttonSoundPlayer = new AudioStreamPlayer();
        sliderSoundPlayer = new AudioStreamPlayer();
        cancelSoundPlayer = new AudioStreamPlayer();
        confirmSoundPlayer = new AudioStreamPlayer();
        hitMoleSoundPlayer = new AudioStreamPlayer();
        mainMusicPlayer = new AudioStreamPlayer();

        AddChild(buttonSoundPlayer);
        AddChild(sliderSoundPlayer);
        AddChild(cancelSoundPlayer);
        AddChild(confirmSoundPlayer);
        AddChild(hitMoleSoundPlayer);
        AddChild(mainMusicPlayer);
    }

    private void LoadSounds()
    {
        buttonSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/tap.wav");
        sliderSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/popup.wav");
        cancelSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/cancel.wav");
        confirmSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/confirm.wav");
        hitMoleSoundPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/sfx/bonk.wav");
        mainMusicPlayer.Stream = GD.Load<AudioStream>("res://assets/audio/music/main.wav");
    }

    private void AssignAudioBuses()
    {
        buttonSoundPlayer.Bus = "Effects";
        sliderSoundPlayer.Bus = "Effects";
        cancelSoundPlayer.Bus = "Effects";
        confirmSoundPlayer.Bus = "Effects";
        hitMoleSoundPlayer.Bus = "Effects";
        mainMusicPlayer.Bus = "Master";
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

    public void PlayHitMoleSound()
    {
        if (!hitMoleSoundPlayer.Playing)
        {
            hitMoleSoundPlayer.Play();
        }
    }

    public void PlayMainMusic(float volume)
    {
        if (!mainMusicPlayer.Playing)
        {
            mainMusicPlayer.VolumeDb = volume;
            mainMusicPlayer.Play();
        }
    }
}
