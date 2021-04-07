using Godot;
using System;

public class DialogBox : Control
{
    //A string array of all text for current situation
    protected String[] dialog;

    //An index for the above array
    protected int dialog_index = 0;

    //A boolean to tell when all text has been displayed
    //and there is nothing left to show
    private Boolean finished = false;

    //The actual text box itself
    protected RichTextLabel text_box;

    //The tween that handles animating our text
    protected Tween text_animator;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        dialog = new String[3]{"Hello there, this tutorial from Emillio on YouTube", "Has been nothing but helpful. I tried a more elaborate way.", "That didn't turn out well."};
        text_box = GetNode("TextBox");
        text_animator = GetNode("Tween");
        loadDialogue();        
    }

    public void loadDialogue() {
        if(dialog_index < dialog.Length) {
            text_box.BbcodeText = dialog[dialog_index];
            text_box.PercentVisible = 0;

        }

        else {
            QueueFree();
        }

        dialog_index++;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("interact")){
            loadDialogue();
        }
    }
}
