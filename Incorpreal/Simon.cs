using Godot;
using System;
using System.Collections.Generic;

public class Simon : Node
{
    public Timer timer;
    public List<int> combination = new List<int>();
    public int count = 0;
    public List<Button> buttons = new List<Button>();
    public List<int> userAnswer = new List<int>();
    public ColorRect battlePage;
    public ColorRect simonPage;
    private Label _codeLabel;
    private Label _instructionLabel;
    private Tween _fadeTween;
    public AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();
    const string Path = "res://sounds/Boop.wav";


    public Simon()
    {
    
    }

    public override void _Ready()
    {
        battlePage = GetParent().GetNode<ColorRect>("BattlePage");
        simonPage = GetNode<ColorRect>("HideButtons");
        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, "onTimeout");
        timer.WaitTime = 1;
        _codeLabel = simonPage.GetNode<Label>("CodeReveal");
        _instructionLabel = simonPage.GetNode<Label>("Instruction");
        _fadeTween = _instructionLabel.GetNode<Tween>("Tween");
        _fadeTween.InterpolateProperty(_instructionLabel, "modulate", Color.Color8(255, 255, 255, 255), Color.Color8(255, 255, 255, 0), 3, Tween.TransitionType.Linear, Tween.EaseType.Out);
        for (int i = 0; i < 4; i++)
        {
            Button tempButton = simonPage.GetChild(i) as Button;
            buttons.Add(tempButton);
            tempButton.Disabled = true;
        }
        this.AddChild(audioStreamPlayer);
        AudioStream Background = (AudioStream)GD.Load(Path);
        audioStreamPlayer.Stream = Background;
    }

    public void startMinigame()
    {
        _fadeTween.Start();
        restartCode();
        battlePage.Visible = false;
        simonPage.Visible = true;
        timer.Start();
    }

    public void restartCode()
    {
        combination.Clear();
        userAnswer.Clear();
        count = 0;
        var random = new Random();
        for (int i = 0; i < 4; i++)
        {
            int temp = random.Next(1, 5);
            if (i != 0)
            {
                while (temp == combination[i - 1])
                {
                    temp = random.Next(1, 5);
                }
            }
            combination.Add(temp);
            buttons[i].Disabled = true;
        }
    }

    public void onTimeout()
    {
        _codeLabel.Text = "";
        if (count <= 3)
        {
            foreach (Button b in buttons)
            {
                b.Disabled = true;
            }
            showColor();
        }
        else
        {
            foreach (Button b in buttons)
            {
                b.Disabled = false;
            }
        }
    }

    public void showColor()
    {
        int number = combination[count];
        buttons[number - 1].Disabled = false;
        _codeLabel.Text = number.ToString();
        count++;
        timer.Start();
    }

    public void checkAnswer()
    {
        GlobalPlayer gp = (GlobalPlayer)GetNode("/root/GlobalData");
        Boolean isCorrect = true;
        for (int i = 0; i < 4; i++)
        {
            if (userAnswer[i] != combination[i])
            {
                isCorrect = false;
            }
        }
        if (isCorrect)
        {
            gp.didBlock = true;
        }
        else
        {
            gp.didBlock = false;
        }
        battlePage.Visible = true;
        simonPage.Visible = false;
        timer.Stop();
    }

    public void _on_Button1_pressed()
    {
        userAnswer.Add(1);
        audioStreamPlayer.Play();
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button2_pressed()
    {
        userAnswer.Add(2);
        audioStreamPlayer.Play();
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button3_pressed()
    {
        userAnswer.Add(3);
        audioStreamPlayer.Play();
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button4_pressed()
    {
        userAnswer.Add(4);
        audioStreamPlayer.Play();
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }


}
