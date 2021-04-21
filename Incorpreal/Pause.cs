using Godot;
using System;

public class Pause : Control
{
    public Label PauseLabel;
    public Label QuitLabel;
    public Label RestartLabel;
    public Label SaveLabel;
    public Label LoadLabel;
    public Label SaveConfirmedLabel;
    public Control LoadDialog;
    public Button ExitButton;
    public VBoxContainer LoadGameVBox;
    public Boolean quitEntered = false;
    public Boolean restartEntered = false;
    public Boolean saveEntered = false;
    public Boolean loadEntered = false;
    public SaveLoadGame saveLoadGame;

    //Pause listener
    public override void _Input(InputEvent @event) {
        if (Input.IsActionJustPressed("pause")) {
            PauseGame();
        }
    }

    public void PauseGame() {
        var pauseBoolean = !GetTree().Paused; //Reverse current pause status
        GetTree().Paused = pauseBoolean; //Set it
        this.Visible = pauseBoolean; //Make pause screen appear/dissappear
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PauseLabel = (Label)GetNode("PauseLabel"); //Grab pause label
        QuitLabel = (Label)GetNode("QuitLabel"); //Grab quit label
        RestartLabel = (Label)GetNode("RestartLabel"); //Grab restart label
        SaveLabel = (Label)GetNode("SaveLabel"); //Grab save label
        LoadLabel = (Label)GetNode("LoadLabel"); //Grab load label
        SaveConfirmedLabel = (Label)GetNode("SaveConfirmLabel"); //Grab save confirmation label
        LoadDialog = (Control)GetNode("LoadDialog"); //Grab load dialog
        ExitButton = (Button)GetNode("LoadDialog/ExitButton"); //Grab load dialog exit button
        LoadGameVBox = (VBoxContainer)GetNode("LoadDialog/LoadGameVBox"); //Grab VBox for selecting load files
        saveLoadGame = new SaveLoadGame();
    }

    public void _on_QuitLabel_mouse_entered() {
        QuitLabel.AddColorOverride("font_color", Colors.DarkRed);
        quitEntered = true;
    }

    public void _on_RestartLabel_mouse_entered() {
        RestartLabel.AddColorOverride("font_color", Colors.DarkRed);
        restartEntered = true;
    }

    public void _on_SaveLabel_mouse_entered() {
        SaveLabel.AddColorOverride("font_color", Colors.DarkRed);
        saveEntered = true;
    }

    public void _on_LoadLabel_mouse_entered() {
        LoadLabel.AddColorOverride("font_color", Colors.DarkRed);
        loadEntered = true;
    }

    public void _on_QuitLabel_mouse_exited() {
        QuitLabel.AddColorOverride("font_color", Colors.DarkGray);
        quitEntered = false;
    }

    public void _on_RestartLabel_mouse_exited() {
        RestartLabel.AddColorOverride("font_color", Colors.DarkGray);
        restartEntered = false;
    }

    public void _on_SaveLabel_mouse_exited() {
        SaveLabel.AddColorOverride("font_color", Colors.DarkGray);
        saveEntered = false;
    }

    public void _on_LoadLabel_mouse_exited() {
        LoadLabel.AddColorOverride("font_color", Colors.DarkGray);
        loadEntered = false;
    }

    public void _on_QuitLabel_gui_input(InputEvent @event) {
        if (quitEntered && @event is InputEventMouseButton && @event.IsPressed()) {
            GetTree().Quit();
        }
    }

    public void _on_RestartLabel_gui_input(InputEvent @event) {
        if (restartEntered && @event is InputEventMouseButton && @event.IsPressed()) {
            GetTree().ReloadCurrentScene(); //Reload the level
            PauseGame(); //Unpause or it will stay paused without the pause screen
        }
    }

    public void _on_SaveLabel_gui_input(InputEvent @event) {
        if (saveEntered && @event is InputEventMouseButton && @event.IsPressed()) {
            GetTree().Paused = false; //Must unpause or GetTree() will return null
            Godot.Collections.Array saveables = GetTree().GetNodesInGroup("persist"); //Snapshot of game state
            GetTree().Paused = true; //Repause
            if ((Boolean)saveLoadGame.Call("Save", saveables)) { //Call Save method in SaveLoadGame, passing snapshot as argument
                SaveConfirmedLabel.Visible = true;
                Timer timer = new Timer();
                timer.Connect("timeout", this, "_on_timer_timeout");
                AddChild(timer, false);
                timer.Start();
            }
        }
    }

    public void _on_timer_timeout() {
        SaveConfirmedLabel.Visible = false;
    }

    public void _on_ExitButton_pressed() {
        LoadDialog.Visible = false;
    }

    public void _on_LoadLabel_gui_input(InputEvent @event) {
        if (loadEntered && @event is InputEventMouseButton && @event.IsPressed()) {
            LoadDialog.Visible = true;
            PopulateLoadDialog();
            //saveLoadGame.Call("Load", selectedFile); not made yet
            //LoadDialog.Visible = false;
        }
    }

    public void PopulateLoadDialog() {
        var saveFile = new File();

        if (!saveFile.FileExists("user://savegame.save")) {
            GD.Print("Cannot find a savefile!");
            return; //Stop loading
        } else {
            saveFile.Open("user://savegame.save", File.ModeFlags.Read);
            var saveNodes = GetTree().GetNodesInGroup("persist");
            saveLoadGame.Load(saveFile, saveNodes);
        }
        /*May use later for selecting which save file to load
        DirectoryInfo dir = new DirectoryInfo(@"user://"); //Access user:// directory
        FileInfo[] Files = dir.GetFiles(".save"); //Get list of files ending in .save
        foreach(FileInfo saveFile in Files) { //Iterate them
            var newOption = (PackedScene)ResourceLoader.Load("res://assets/LoadSelection.tscn");
            Label optionLabel = (Label)newOption.Instance();
            LoadGameVBox.AddSpacer(false);
            LoadGameVBox.AddChild(optionLabel);
        }*/
    }
}