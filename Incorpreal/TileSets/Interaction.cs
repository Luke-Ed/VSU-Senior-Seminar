using Godot;
using System;

public class Interaction : CanvasLayer
{
    //Code inspired by GameDevelopmentCenter on YouTube

    //The dialogue box scene for use in code
    PackedScene dialogueBoxes = GD.Load<PackedScene>("res://DialogueBoxV2.tscn");

    GlobalPlayer gp;

    //CanvasLayer to hold dialogue box
    DialogBox diagBox;

    //Our "state" for player interaction with map objects,
    //Will likely be replaced by an Enum as code grows more complex
    //Enum states = {Idle, Looting, Reading, Puzzle, Secret}
    protected String actionState = "off";

    private Vector2 _cavePos = new Vector2(1232,0);

    //The loot areas to be disabled after the chest opens
    private Godot.Collections.Array<Area2D> _lootAreas = new Godot.Collections.Array<Area2D>();

    //The collsion shape to turn off
    private CollisionShape2D _currentChest; 
    
    /*Chests are loaded from left to right, top to bottom. 
    Chests in the upper left corner are loaded before lower right.
    Tracking number of chests. */
    private int _chestIndex = 0;

    //Variables to hold current tilemap, tile texture and tile location
    private TileMap _interactiveTilemap;
    private Vector2 _currentTileLocation;
    private Vector2 _currentTileTexture;
    private Vector2 _openChest = new Vector2(2,0);
    private Godot.Collections.Array<Vector2> _allTileLocations = new Godot.Collections.Array<Vector2>();
    private Godot.Collections.Array<Vector2> _allTileTextures = new Godot.Collections.Array<Vector2>();

    //Empty constructor for use in other scripts
    public Interaction() {

    }

    //Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        _interactiveTilemap = (TileMap)GetNode("../Interactables");
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
    }

    public override void _Process(float delta) {
        if(Input.IsActionJustPressed("interact")) {
            //Respond based on the actionState
            switch(actionState) {
                case "off":
                    GD.Print("I got nothing to do here. \n");
                    break;

                case "on": 
                    //Add some function or load a scene/panel for gold, etc.
                    diagBox = dialogueBoxes.Instance() as DialogBox;
                    diagBox.DialogPath = "res://Dialogues/Chest.txt";
                    GetTree().Root.GetNode("Node2D/Player/Camera2D/").AddChild(diagBox);
                    _interactiveTilemap.SetCell((int)_currentTileLocation[0], (int)_currentTileLocation[1], 22, false, false, false, _currentTileTexture);
                    //SetCell(int x, int y, int tile, boolean flip_x, boolean flip_y, boolean transpose, Vector2 autotileCoordinates)

                    _currentChest = (CollisionShape2D)(GetNode(((String)(_lootAreas[_chestIndex].GetPath() + "/CollisionShape2D"))));
                    _currentChest.Disabled = true;
                    //Disable the collision box for this chest

                    _chestIndex++;
                    //Switch to next chest
                    break;

                case "sign":
                    diagBox = dialogueBoxes.Instance() as DialogBox;
                    diagBox.DialogPath = "res://Dialogues/Sign.txt";
                    GetTree().Root.GetNode("Node2D/Player/Camera2D/").AddChild(diagBox);
                    break;

                case "grave":
                    diagBox = dialogueBoxes.Instance() as DialogBox;
                    diagBox.DialogPath = "res://Dialogues/Grave.txt";
                    GetTree().Root.GetNode("Node2D/Player/Camera2D/").AddChild(diagBox);
                    break;
                
                default:
                    GD.Print("... \n");
                    break;
            }
        }
    }

    //The actual vector2 tile coordiate on the tilemap
    public void AddChestTile(Vector2 used_tile) {
        _allTileLocations.Add(used_tile);
    }

    //The vector2 of tile texture from the tileset
    public void AddChestTextures(Vector2 used_texture) {
        _allTileTextures.Add(used_texture);
    }

    //The loot areas that will be disabled once the chest is opened
    public void AddOpenedLootAreas(Area2D openedChest) {
        _lootAreas.Add(openedChest);
    }
    
    //When the player enters a loot_area, print "Looting"
    public void OnLootAreaEntered(Area2D area) {
        actionState = "on";
        _currentTileLocation = (Vector2)_allTileLocations[_chestIndex]; //Pass the current tile ingame to the process method
        _currentTileTexture = (Vector2)_allTileTextures[_chestIndex] + _openChest; //Pass the Vector2 and change the texture to the "open chest" texture
    }

    //When the player leaves a loot_area, print "Leaving"
    //I'll probably change all the Area_Exited methods to use this one
    //instead of having their own
    public void OnLootAreaExited(Area2D area) {
        actionState = "off";
    }

    public void OnSignAreaEntered(Area2D area) {
        actionState = "sign";
    }

    public void OnSignAreaExited(Area2D area) {
        actionState = "off";
    }

    public void OnGraveAreaEntered(Area2D area) {
        actionState = "grave";
    }

    public void OnGraveAreaExited(Area2D area) {
        actionState = "off";
    }

    public void OnTransitionAreaEntered(Area2D area) {
        //GetNode<KinematicBody2D>("../Player").Position = _cavePos;
        gp.playerLocation = _cavePos;
        GetTree().ChangeScene("res://TileSets/CaveMap.tscn");
    }

    public void OnTransitionAreaExited(Area area) {
        actionState = "off";
    }
}