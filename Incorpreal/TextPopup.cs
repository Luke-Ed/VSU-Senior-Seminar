using Godot;
using System;

public class TextPopup : Popup
{
    protected String npc_name;

    protected String[] dialogue;

    protected String answers;

    private int dialogueIndex = 0;

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

    //Set the name of the current speaker or object
    public void setSpeaker(String source) {
        npc_name = source;
        speaker_name.Text = source;
    }

    //Set the array of text for the dialogue box
    //to pull from during the chat/reading
    public void setDialogue(String[] inText) {
        dialogue = inText;
    }

    //Set the actual text inside of the box
    public void setDialogueText(String curText) {
        output_text.Text = curText;
    }

    //Show the player a few options depending
    //on what is currently happening
    //E.g. opening a chest, reading a sign, etc.
    public void setPlayerResponse(String options) {
        answers = options;
        continue_options.Text = options;
    }

    //Show the popup box, it will remain
    //hidden until this method is called
    public void openDialogue() {
        GetTree().Paused = true;
        this.PopupCentered();
        animPlay.PlaybackSpeed = (float)(60.0 / dialogue.Length);
        animPlay.Play("ShowDialogue");
    }

    //Hide the popup box
    public void closeDialogue() {
        GetTree().Paused = false;
        this.Hide();
    }

    //Allow the player to give input only when the animation ends
    public void _on_DialogueAnimation_animation_finished() {
        this.SetProcessInput(true);
    }

    //Move through the diaglogue array and load it into the text box,
    //line by line.
    public void loadDialogue() {
        if(dialogueIndex < dialouge.Length) {
            
        }

        else {

        }

        dialogueIndex++;
    }
}