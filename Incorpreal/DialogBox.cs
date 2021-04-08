using Godot;
using System;
//using System.Diagnostics;

public class DialogBox : Control
{
    //Code inspired by Emilio on YouTube 
    
    //A string array of all text for current situation
    [Export]
    protected String[] dialog;

    //protected String dialogPath = "";

    //protected float textSpeed = 0.05;

    //An index for the above array
    protected int dialog_index = 0;

    //A boolean to tell when all text has been displayed
    //and there is nothing left to show
    private Boolean finished = false;

    //The actual text box itself
    private RichTextLabel text_box;

    //The little corner indicator for when text is ready to transition
    private Sprite next_indicator;

    //The tween that handles animating our text
    private Tween text_animator;
    //The name tween comes from in-betweening, an animation technique where you specify keyframes and the computer interpolates the frames that appear between them -GodotDocumentation

    //Empty constructor for other classes
    public DialogBox() {

    }

    //Set the array of text to pull from
    public void setDialog(String[] inText) {
        dialog = inText;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //text_timer = (Timer)GetNode("TextTime");
        //text_timer.WaitTime = textSpeed;
        //dialog = getDialog();
        //Debug.Assert(dialog, "File path does not exist);
        //nextPhrase();

        text_box = (RichTextLabel)GetNode("TextBox");
        text_animator = (Tween)GetNode("Tween");
        next_indicator = (Sprite)GetNode("Next-Indicator");
        String[] responses = new String[1]{"You found a _______. This will help you on your journey."};
        this.setDialog(responses);
        loadDialogue();
    }

    //Pull the next line of text and play it inside the text box
    public void loadDialogue() {
        if(dialog_index < dialog.Length) {
            finished = false;
            text_box.BbcodeText = dialog[dialog_index];
            text_box.PercentVisible = 0;
            text_animator.InterpolateProperty(text_box, "percent_visible", 0, 1, 5, Tween.TransitionType.Linear, Tween.EaseType.InOut);
            //InterpolateProperty(Object @object, NodePath property, object initialVal, object finalVal, float duration, TransitionType transType, EaseType easeType, float delay = 0f)
            text_animator.Start();
        }

        else {
            QueueFree();
        }

        dialog_index++;
    }

    // Called every frame. The value 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        next_indicator.Visible = finished;
        if(Input.IsActionJustPressed("Continue")){
            loadDialogue();
        }
    }

    public void onTweenCompleted(Godot.Object obj, NodePath key) {
        finished = true;
    }

    // public String[] getDialog() {
    //     try {
    //         File textFile = new File();
    //         Debug.Assert(textFile.Exists(dialogPath), "File not found");

    //         textFile.Open(dialogPath, (int)File.ModeFlags.Read);

    //         var jsonFile =  textFile.ReadAllLines();

    //     }

    //     catch(Exception EX) {
    //         Console.WriteLine(EX.ToString());
    //     }
    // }
}
