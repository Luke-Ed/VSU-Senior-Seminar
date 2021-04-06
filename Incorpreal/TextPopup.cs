using Godot;
using System;

public class TextPopup : Popup
{
    protected String npc_name;

    protected String[] dialogue;

    protected String answers;

    Label speaker_name;

    Label output_text;

    Label continue_options;

    AnimationPlayer animPlay;

    //Empty constructor for use in other classes
    public TextPopup() {
        
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        speaker_name = (Label)GetNode("TextPopup/ColorRect/ObjectName");
        output_text = (Label)GetNode("TextPopup/ColorRect/ObjectText");
        continue_options = (Label)GetNode("TextPopup/ColorRect/PlayerOptions");
        animPlay = (AnimationPlayer)GetNode("TextPopup/DialogueAnimation");
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

    public void setDialogue(String[] inText) {
        dialogue = inText;
    }

    public void setDialogueText(String curText) {
        output_text.Text = curText;
    }

    public void setPlayerResponse(String options) {
        answers = options;
        continue_options.Text = options;
    }

    public void openDialogue() {
        GetTree().Paused = true;
        this.PopupCentered();
        animPlay.PlaybackSpeed = (float)(60.0 / dialogue.Length);
        animPlay.Play("ShowDialogue");
    }

    public void closeDialogue() {
        GetTree().Paused = false;
        this.Hide();
    }

    public void _on_DialogueAnimation_animation_finished() {
        this.SetProcessInput(true);
    }

    public void loadDialogue() {
        if(Input.IsActionJustPressed("Continue")) {
            
            this.SetProcessInput(false);
        }

        // else if(Input.IsActionJustPressed("Back")) {
        //     this.SetProcessInput(false);
        // }

        // else {

        // }
    }
}