using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SaveLoadGame : Node
{
    public GlobalPlayer gp;
    public Godot.Collections.Dictionary<string, object> nodeData_;
    public override void _Ready()
    {
        gp = (GlobalPlayer)GetNode("/root/GlobalData");
    }

    public Boolean Save(Godot.Collections.Array saveables)
    {
        var saveFile = new File();
        saveFile.Open("user://savegame.save", File.ModeFlags.Write); //Open file in write mode

        foreach (Node saveable in saveables)
        { //Iterate them
            if (saveable.Filename.Empty())
            { //Skip empty nodes
                GD.Print(String.Format("node '{0}' is not an instanced scene ", saveable.Name));
                continue;
            }
            if (!saveable.HasMethod("Save"))
            { //Skip ones without save methods
                GD.Print("node '{0}' has no Save method", saveable.Name);
                continue;
            }
            var saveData = saveable.Call("Save");
            saveFile.StoreLine(JSON.Print(saveData));
        }

        saveFile.Close();
        return true;
    }

    public System.Action Unload(File saveFile, Godot.Collections.Dictionary<string, object> nodeData) 
    {
        this.nodeData_ = nodeData;

        //Check which level its in
        string level = (string) nodeData["currentLevel"];

        //Clear old global values
        gp.enemyFought.Clear();
        gp._inventory.Clear();
        gp._equipedArmor = null;
        gp._equipedArmor = null;

        //Load global values
        string equippedArmor = (string)nodeData_["equipedArmor"];
        string[] equippedArmorData = equippedArmor.Split(",");
        Item equippedArmorItem = new Item();
        equippedArmorItem.giveProperties(equippedArmorData[0], equippedArmorData[1], equippedArmorData[2], Int16.Parse(equippedArmorData[3]));
        equippedArmorItem._spritePath = equippedArmorData[4];
        gp._equipedArmor = equippedArmorItem;
        string equippedWeapon = (string)nodeData_["equipedWeapon"];
        string[] equippedWeaponData = equippedWeapon.Split(",");
        Item equippedWeaponItem = new Item();
        equippedWeaponItem.giveProperties(equippedWeaponData[0], equippedWeaponData[1], equippedWeaponData[2], Int16.Parse(equippedWeaponData[3]));
        equippedWeaponItem._spritePath = equippedWeaponData[4];
        gp._equipedWeapon = equippedWeaponItem;
        string inventory = (string)nodeData_["inventory"];
        string[] inventoryItems = inventory.Split("|");
        foreach(string item in inventoryItems) {
            string[] itemData = item.Split(",");
            Item newItem = new Item();
            newItem.giveProperties(itemData[0], itemData[1], itemData[2], Int16.Parse(itemData[3]));
            newItem._spritePath = itemData[4];
            gp._inventory.Add(newItem);
        }
        gp.currentPoints = (int)((float)nodeData_["currentPoints"]);
        gp.spiritPoints = (int)((float)nodeData_["spiritPoints"]);
        gp.baseStat = (int)((float)nodeData_["baseStat"]);
        gp.ExperienceToNextLevel = (int)((float)nodeData_["ExperienceToNextLevel"]);
        gp.AttackDamage = (int)((float)nodeData_["AttackDamage"]);
        gp.Level = (int)((float)nodeData_["Level"]);
        gp.CurrentHealth = (int)((float)nodeData_["CurrentHealth"]);
        gp.MaxHealth = (int)((float)nodeData_["MaxHealth"]);
        gp.Experience = (int)((float)nodeData_["Experience"]);
        gp.Luck = (int)((float)nodeData_["Luck"]);
        gp.Intelligence = (int)((float)nodeData_["Intelligence"]);
        gp.Vitality = (int)((float)nodeData_["Vitality"]);
        gp.Dexterity = (int)((float)nodeData_["Dexterity"]);
        gp.Strength = (int)((float)nodeData_["Strength"]);
        Vector2 newPosition = new Vector2((float)nodeData_["PosX"], (float)nodeData_["PosY"]);
        gp.playerLocation = newPosition;

        //Load the level
        GetTree().ChangeScene(level);

        saveFile.Close();
        System.Action loadAction = new System.Action(Delay); //Return Load() as the next action to be performed once old nodes are freed
        return loadAction;
    }

    public void Delay() {
        GD.Print("");
    }

    public void Load() {
        //Grab the player node
        Player player = null;
        foreach (Node node in GetTree().CurrentScene.GetChildren()) {
            if (node.Name == "Player") {
                player = (Player)node;
            }
        }

        //Check if the player is possessing someone or not
        if (nodeData_["resPath"] != null) {
            player.resPath = (string)nodeData_["resPath"];
            Texture newTexture = (Texture)ResourceLoader.Load((string)nodeData_["playerSpriteNode.Texture.ResourcePath"]);
            player.playerSpriteNode.Texture = newTexture;
            player.PossesseeName = (string)nodeData_["PossesseeName"];
            gp.isPossesing = true;
            gp.enemyPossessed = (string)nodeData_["enemyPossessed"];
            if (gp.isPossesing && gp.enemyPossessed != null && GetTree().CurrentScene.HasNode("/root/Node2D/Enemies/" + gp.enemyPossessed)) { //Remove the possessed enemy
                GetNode("/root/Node2D/Enemies/" + (string)nodeData_["enemyPossessed"]).QueueFree();
            }
        } else {
            gp.isPossesing = false;
        }

        //Reload all the necessary values
        player.moveSpeed = (int)((float)nodeData_["moveSpeed"]);
        player.stuck = (Boolean)nodeData_["stuck"];
        player.ExperienceToNextLevel = gp.ExperienceToNextLevel;
        player.AttackDamage = gp.AttackDamage;
        player.Level = gp.Level;
        player.CurrentHealth = gp.CurrentHealth;
        player.MaxHealth = gp.MaxHealth;
        player.Experience = gp.Experience;
        player.Luck = gp.Luck;
        player.Intelligence = gp.Intelligence;
        player.Vitality = gp.Vitality;
        player.Dexterity = gp.Dexterity;
        player.Strength = gp.Strength;
        Vector2 newPosition = new Vector2((float)nodeData_["PosX"], (float)nodeData_["PosY"]);
        player.Position = newPosition;
        player.playerSpriteNode.FlipH = (Boolean)nodeData_["facingLeft"];
        string[] enemiesFought = ((string)nodeData_["enemyFought"]).Split(",");
        foreach (string enemy in enemiesFought) { //Remove all enemies already defeated in combat
            if (GetTree().CurrentScene.HasNode("/root/Node2D/Enemies/" + enemy)) {
                gp.enemyFought.Add(enemy);
                GetNode("/root/Node2D/Enemies/" + enemy).QueueFree(); //Level's root node must be named "Node2D" for these to work. Also enemies  must have different names (bat, bat2, etc)
            }
        }
        //Label healthLabel = (Label)GetNode("Player/Camera2D/CanvasLayer/HealthLabel");
        //healthLabel.Text = (string)nodeData["hplabel"];
        this.nodeData_ = null;
    }
}