using System;
using Godot;

namespace Incorpreal.Battle {
  public class Battle : Node {
    private Label _enemyHp;
    private TurnQueue _turnQueue;
    private GlobalPlayer _globalPlayer;
    private int _currentFighterPos;
    private Label _playerHp;
    private Node _player;
    public Node enemy;
    public RichTextLabel rtl;
    public Boolean fightOver = false;
    private Button attackbtn, spellbtn, _defendBtn;
    public Boolean playerActed = true;
    public ColorRect battlePage;
    public ColorRect SimonPage;
    public Simon s;
    public Godot.Timer timer;
    public HitTheTarget_Engan HitTheTarget;

    public override void _Ready() {
      s = (Simon)GetNode("SimonGame");
      HitTheTarget = (HitTheTarget_Engan)GetNode("HitTheTarget_Engan");
      battlePage = GetNode<ColorRect>("BattlePage");
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
      _turnQueue = (TurnQueue)GetNode("/root/Tq");
      _turnQueue.combatants = _turnQueue.getCombatants();
      _turnQueue.setStats();
      _player = (Node)_turnQueue.combatants[0] as KinematicBody2D;
      enemy = (Node)_turnQueue.combatants[1] as KinematicBody2D;
      _currentFighterPos = 0;
      _playerHp = battlePage.GetNode<Label>("HealthLabel") as Label;
      _enemyHp = battlePage.GetNode<Label>("EnemyHealth") as Label;
      rtl = battlePage.GetNode<RichTextLabel>("RichTextLabel") as RichTextLabel;
      rtl.Text = "You have encountered a(n): " + _turnQueue.enemyName + "\n";
      attackbtn = battlePage.GetNode<Button>("Attackbtn") as Button;
      spellbtn = battlePage.GetNode<Button>("Spellbtn") as Button;
      _defendBtn = battlePage.GetNode<Button>("DefendBtn") as Button;
      timer = battlePage.GetNode<Godot.Timer>("Timer") as Godot.Timer;
      timer.WaitTime = 20;
      timer.Connect("timeout", this, "onTimeout");
      _globalPlayer.updateHealthLabel(_playerHp);
      UpdateEnemyHealth();
    }

    public void UpdateEnemyHealth() {
      String text = "Enemy Health: " + _turnQueue.enemyCurrentHP + "/" + _turnQueue.enemyMaxHP;
      if (_enemyHp != null) {
        _enemyHp.Text = text;
      }
    }

    public void fight() {
      if (!fightOver) {
        if (_currentFighterPos == 0) {
          _globalPlayer.isDefending = false;
          _globalPlayer.didBlock = false;
          playerActed = false;
          displayPlayerOptions();
          rtl.Text += "Choose an action \n";
          timer.Start();
        }
        else {
          Boolean didHit = (bool)enemy.Call("playTurn");
          if (didHit) {
            rtl.Text += "You got hit by the " + _turnQueue.enemyName + "\n";
            if (_globalPlayer.didBlock) {
              rtl.Text += "but you blocked perfectly!" + "\n";
            }
          }
          else {
            rtl.Text += "The attack passed through you" + "\n";
          }
          _globalPlayer.updateHealthLabel(_playerHp);
        }
        changeCurrentFighter();
        if (_globalPlayer.CurrentHealth <= 0 || _turnQueue.enemyCurrentHP <= 0) {
          if (_globalPlayer.CurrentHealth <= 0) {
            rtl.Text += "You have lost the fight";
            playerActed = false;
            GetNode<ColorRect>("DeathScreen").Visible = true;
          }
          else {
            rtl.Text += "You have won the fight";
            _globalPlayer.Experience += 10;
            if(_globalPlayer.Experience >= _globalPlayer.ExperienceToNextLevel)
            {
              GetTree().ChangeScene("res://LevelUp.tscn");
            }
          }
          fightOver = true;
        }
      }
      else
      {
        GetTree().ChangeScene(_globalPlayer.lastScene);
      }
    }

    private void displayPlayerOptions() {
      attackbtn.Visible = !attackbtn.Visible;
      spellbtn.Visible = !spellbtn.Visible;
      if (_globalPlayer.currentPoints < 5) {
        spellbtn.Disabled = true;
      }
      else {
        spellbtn.Disabled = false;
      }
      _defendBtn.Visible = !_defendBtn.Visible;
    }

    public void changeCurrentFighter() {
      _currentFighterPos = (_currentFighterPos + 1) % _turnQueue.combatants.Count;
    }

    public override void _Input(InputEvent @event) {
      if (playerActed && battlePage.Visible) {
        if (@event.IsActionPressed("Continue")) {
          fight();
        }
      }
    }

    public void _on_Attackbtn_pressed() {
      timer.Stop();
      Boolean didHit = (bool)_player.Call("attackEnemy");
      if (didHit) {
        rtl.Text += "You hit the " + _turnQueue.enemyName + "\n";
      }
      else {
        rtl.Text += "You missed the " + _turnQueue.enemyName + "\n";
      }
      playerActed = true;
      UpdateEnemyHealth();
      displayPlayerOptions();
    }

    public void _on_Spellbtn_pressed() {
      timer.Stop();
      _player.Call("castSpell");
      rtl.Text += "You cast a spell at the " + _turnQueue.enemyName + "\n";
      playerActed = true;
      HitTheTarget.minigameStart();
      UpdateEnemyHealth();
      displayPlayerOptions();
    }

    public void _on_DefendBtn_Pressed() {
      timer.Stop();
      _globalPlayer.isDefending = true;
      rtl.Text += "You enter a defending stance" + "\n";
      playerActed = true;
      s.startMinigame();
      UpdateEnemyHealth();
      displayPlayerOptions();
    }

    public void onTimeout() {
      displayPlayerOptions();
      rtl.Text += "You spent too long and the " + _turnQueue.enemyName + " attacks!\n";
      playerActed = true;
    }

    public void _on_Resetbtn_pressed() {
      _globalPlayer.playerCharacter = null;
      _globalPlayer.playerLocation = new Vector2(324, 179);
      _globalPlayer.enemyFought.Clear();
      _globalPlayer.createPlayer();
      GetTree().ChangeScene(_globalPlayer.lastScene);
    }

    public void _on_Quitbtn_pressed() {
      GetTree().Quit();
    }
  }
}
