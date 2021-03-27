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
        for (int i = 0; i < 4; i++)
        {
            Button tempButton = simonPage.GetChild(i) as Button;
            buttons.Add(tempButton);
            tempButton.Disabled = true;
        }
    }

    public void startMinigame()
    {
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
        if(userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button2_pressed()
    {
        userAnswer.Add(2);
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button3_pressed()
    {
        userAnswer.Add(3);
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }

    public void _on_Button4_pressed()
    {
        userAnswer.Add(4);
        if (userAnswer.Count == 4)
        {
            checkAnswer();
        }
    }


}
