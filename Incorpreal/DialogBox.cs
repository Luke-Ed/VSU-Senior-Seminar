using Godot;
using System;
using System.Diagnostics;

public class DialogBox : CanvasLayer
{
    //Code inspired by Emilio on YouTube 
    
    //String path to dialogue JSON file
    [Export] public String DialogPath = "res://Dialogues/Dialog0.txt";
    
    //A string array of all text for current situation
    protected String[] _dialog;

    //An index for the above array
    protected int dialogIndex = 1;

    //A boolean to tell when all text has been displayed
    //and there is nothing left to show
    private Boolean _finished = false;

    //Speaker name box
    private RichTextLabel _speaker;

    //The actual text box itself
    private RichTextLabel _textBox;

    //The little corner indicator for when text is ready to transition
    private Sprite _nextIndicator;

    //The tween that handles animating our text
    private Tween _textAnimator;
    //The name tween comes from in-betweening, an animation technique where you specify keyframes and the computer interpolates the frames that appear between them -GodotDocumentation

    //Empty constructor for other classes
    public DialogBox() {

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _dialog = GetDialogue();
        _speaker = (RichTextLabel)GetNode("DialogBox/SpeakerBox");
        _textBox = (RichTextLabel)GetNode("DialogBox/TextBox");
        _textAnimator = (Tween)GetNode("DialogBox/Tween");
        _nextIndicator = (Sprite)GetNode("DialogBox/Next-Indicator");
        LoadDialogue();
    }

    //Pull the next line of text and play it inside the text box
    public void LoadDialogue() {
        if(dialogIndex < _dialog.Length) {
            _finished = false;
            _speaker.BbcodeText = _dialog[0];
            _textBox.BbcodeText = _dialog[dialogIndex];
            _textBox.PercentVisible = 0;
            _textAnimator.InterpolateProperty(_textBox, "percent_visible", 0, 1, 2, Tween.TransitionType.Linear, Tween.EaseType.InOut);
            //InterpolateProperty(Object @object, NodePath property, object initialVal, object finalVal, float duration, TransitionType transType, EaseType easeType, float delay = 0f)
            _textAnimator.Start();
        }

        else {
            QueueFree();
        }

        dialogIndex++;
    }

    // Called every frame. The value 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _nextIndicator.Visible = _finished;
        if(Input.IsActionJustPressed("Continue")){
            LoadDialogue();
        }
    }

    public void onTweenCompleted(Godot.Object obj, NodePath key) {
        _finished = true;
    }

    public String[] GetDialogue() {
        try {
            File textFile = new File();
            Debug.Assert(textFile.FileExists(DialogPath), "File not found");

            textFile.Open(DialogPath, File.ModeFlags.Read);

            var inFile =  textFile.GetAsText();

            return inFile.Split(',');
        }

        catch(Exception EX) {
            Console.WriteLine(EX.ToString());
            return new String[1]{"Hey! Your file isn't there!"};
        }
    }
}
