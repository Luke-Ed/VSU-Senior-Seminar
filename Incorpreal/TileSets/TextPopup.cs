using Godot;
using System;

public class TextPopup : Popup
{
    protected String npc_name;

    protected String dialogue;

    protected String answers;

    Label speaker_name = (Label)GetNode("Interaction_Console/TextPopup/ColorRect/ObjectName");
    Label output_text = (Label)GetNode("Interaction_Console/TextPopup/ColorRect/ObjectText");
    Label continue_options = (Label)GetNode("Interaction_Console/TextPopup/ColorRect/PlayerOptions");
    AnimationPlayer animPlay = (AnimationPlayer)GetNode("Interaction_Console/TextPopup/DialogueAnimation");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.SetProcessInput(false);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    //public override void _Process(float delta)
    //{
    //
    //}

    public void setSpeaker(String source) {
        npc_name = source;
        speaker_name.Text = source;
    }

    public void setBoxText(String inText) {
        dialogue = inText;
        output_text.Text = inText;
    }

    public void setPlayerResponse(String options) {
        answers = options;
        continue_options.Text = options;
    }

    public void openDialogue() {
        GetTree().Paused = true;
        this.Popup();
        animPlay.PlaybackSpeed = 60.0 / dialogue.Length;
        animPlay.Play("ShowDialogue");
    }

    public void closeDialogue() {
        GetTree().Paused = false;
        this.Hide();
    }

    func _on_DialogueAnimation_animation_finished() {
        this.SetProcessInput(true);
    }

    public void _Input() {
        if(Input.IsActionJustPressed("Continue")) {
            this.SetProcessInput(false);
        }

        else if(Input.IsActionJustPressed("Back")) {
            this.SetProcessInput(false);
        }

        else {

        }
    }
}