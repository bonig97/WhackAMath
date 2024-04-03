using Godot;
using System;

public partial class MusicEffectsUI : Control
{
    private Button tutorialButton;
    private HSlider musicSlider;
    private HSlider effectsSlider;

    public override void _Ready()
    {
        tutorialButton = GetNode<Button>("TutorialButton");
        musicSlider = GetNode<HSlider>("HBoxContainer/MusicSlider");
        effectsSlider = GetNode<HSlider>("HBoxContainer/EffectsSlider");

        tutorialButton.Pressed += OnTutorialButtonPressed;
        // musicSlider.ValueChanged += OnMusicSliderValueChanged;
        // effectsSlider.ValueChanged += OnEffectsSliderValueChanged;
    }

    private void OnTutorialButtonPressed()
    {
        // Handle tutorial button press
        GD.Print("Tutorial button pressed");
    }

    private void OnMusicSliderValueChanged(float value)
    {
        // Handle music slider value change
        GD.Print($"Music volume: {value}");
    }

    private void OnEffectsSliderValueChanged(float value)
    {
        // Handle effects slider value change
        GD.Print($"Effects volume: {value}");
    }
}