using System;
using Godot;

namespace Incorpreal.Battle {
  public class Battle : Node {
    private Label _enemyHp;
    private TurnQueue _turnQueue;
    private GlobalPlayer _globalPlayer;
    private int _activeFighter;
    private Label _playerHp;
    private Node _player;
    private Node _enemy;
    private RichTextLabel _battleSequenceRtl;
    private Boolean _fightOver;
    private Button _attackBtn, _spellBtn, _defendBtn;
    private Boolean _playerActed = true;
    private ColorRect _battlePage;
    public ColorRect SimonPage;
    private Simon _simon;
    private Timer _battleTimer;
    private HitTheTarget_Engan _hitTheTarget;
    private TimingGame _timingGame;
    private ColorRect _deathScreen;
    private Boolean _resetHovered, _quitHovered;
    private Label _resetLabel, _quitLabel;


    public override void _Ready() {
      _simon = (Simon) GetNode("SimonGame");
      _hitTheTarget = (HitTheTarget_Engan) GetNode("HitTheTarget_Engan");
      _battlePage = GetNode<ColorRect>("BattlePage");
      _globalPlayer = (GlobalPlayer) GetNode("/root/GlobalData");
      _turnQueue = (TurnQueue) GetNode("/root/Tq");
      _turnQueue.Combatants = _turnQueue.GetCombatants();
      _turnQueue.SetStats();
      _player = (Node) _turnQueue.Combatants[0] as KinematicBody2D;
      _enemy = (Node) _turnQueue.Combatants[1] as KinematicBody2D;
      _playerHp = _battlePage.GetNode<Label>("HealthLabel");
      _enemyHp = _battlePage.GetNode<Label>("EnemyHealth");
      _battleSequenceRtl = _battlePage.GetNode<RichTextLabel>("RichTextLabel");
      _battleSequenceRtl.Text = "You have encountered a(n): " + _turnQueue.EnemyType + "\n";
      _attackBtn = _battlePage.GetNode<Button>("AttackBtn");
      _spellBtn = _battlePage.GetNode<Button>("SpellBtn");
      _defendBtn = _battlePage.GetNode<Button>("DefendBtn");
      _battleTimer = _battlePage.GetNode<Timer>("Timer");
      _battleTimer.WaitTime = 20;
      _battleTimer.Connect("timeout", this, "OnTimeout");
      _globalPlayer.hplabel = _playerHp;
      _globalPlayer.updateHealthLabel(_playerHp);
      UpdateEnemyHealth();
      _timingGame = (TimingGame) GetNode("TimingGame_Engan");
      _deathScreen = GetNode("DeathScreen").GetNode("Control").GetNode<ColorRect>("DeathScreen");
      _resetLabel = (Label) _deathScreen.GetNode("Reset");
      _quitLabel = (Label) _deathScreen.GetNode("Quit");
    }

    private void UpdateEnemyHealth() {
      String text = "Enemy Health: " + _turnQueue.EnemyCurrentHp + "/" + _turnQueue.EnemyMaxHp;
      if (_enemyHp != null) {
        _enemyHp.Text = text;
      }
    }

    public void Fight() {
      if (!_fightOver) {
        // If the fight is not over, proceed through the following sequence.
        switch (_activeFighter) {
          case 0:
            // If the player ius the currently selected fighter, start their turn.
            StartPlayerTurn();
            break;
          default: {
            // If the current fighter is the enemy.
            Boolean didHit = (bool) _enemy.Call("PlayTurn");
            if (didHit) {
              _battleSequenceRtl.Text += "You got hit by the " + _turnQueue.EnemyType + "\n";
              if (_globalPlayer.didBlock) {
                _battleSequenceRtl.Text += "but you blocked perfectly!" + "\n";
              }
            }
            else {
              _battleSequenceRtl.Text += "The attack passed through you" + "\n";
            }
          }
            _globalPlayer.updateHealthLabel(_playerHp);
            UpdateEnemyHealth();
            break;
        }

        ChangeActiveFighter();
        // Switch the currently active fighter, to allow both fighters to have turns.

        // Check the player and enemy's health. If either of them are at or below 0 then process them appropriately. 
        if (_globalPlayer.PlayerCharacter.CurrentHealth <= 0) {
          // If the player's health is low, display the death screen, and after informing them the fight is over.
          _battleSequenceRtl.Text += "You have lost the fight";
          _playerActed = false;
          GetNode<ColorRect>("DeathScreen").Visible = true;
          _fightOver = true;
        }

        if (_turnQueue.EnemyCurrentHp <= 0) {
          // If the enemy's helth is low, inform the player that they won.
          _battleSequenceRtl.Text += "You have won the fight";
          _globalPlayer.PlayerCharacter.Experience += 10;
          if (_globalPlayer.PlayerCharacter.Experience >= _globalPlayer.PlayerCharacter.ExperienceToNextLevel) {
            GetTree().ChangeScene("res://LevelUp.tscn");
            // Remove player status effect on level up.
            _globalPlayer.PlayerCharacter.StatusEffect = null;
          }

          _fightOver = true;
        }
      }
      else {
        GetTree().ChangeScene(_globalPlayer.lastScene);
      }
    }

    private void StartPlayerTurn() {
      // This code is ran anytime a player's turn is started.
      // This resets any actions to their default values, displays the players turn options, comments in the chat, and starts the turn timer.
      _globalPlayer.isDefending = false;
      _globalPlayer.didBlock = false;
      _playerActed = false;
      if (_globalPlayer.PlayerCharacter.StatusEffect != String.Empty) {
        _battleSequenceRtl.Text += "You are " + _globalPlayer.PlayerCharacter.StatusEffect + "\n";
        switch (_globalPlayer.PlayerCharacter.StatusEffect) {
          case ("Bleeding"):
            _globalPlayer.PlayerCharacter.CurrentHealth -= 2;
            break;
          default:
            break;
        }
      }

      _globalPlayer.updateHealthLabel(_playerHp);
      DisplayPlayerOptions();
      _battleSequenceRtl.Text += "Choose an action \n";
      _battleTimer.Start();
    }

    private void DisplayPlayerOptions() {
      // Show relevant buttons to the player
      _attackBtn.Visible = !_attackBtn.Visible;
      _spellBtn.Visible = !_spellBtn.Visible;
      _defendBtn.Visible = !_defendBtn.Visible;
      // If the player doesn't have enough magic points, disable the spell's button.
      _spellBtn.Disabled = _globalPlayer.PlayerCharacter.CurrentSpiritPoints < 5;
    }

    private void ChangeActiveFighter() {
      // Switch which fighter is currently active (able to perform an action).
      _activeFighter = (_activeFighter + 1) % _turnQueue.Combatants.Count;
    }

    public override void _Input(InputEvent @event) {
      if (_playerActed && _battlePage.Visible) {
        if (@event.IsActionPressed("Continue")) {
          Fight();
        }
      }
    }

    public void _on_AttackBtn_Pressed() {
      _battleTimer.Stop();
      _timingGame.startMinigame();
      _playerActed = true;
      UpdateEnemyHealth();
      DisplayPlayerOptions();
    }

    public void _on_SpellBtn_Pressed() {
      _battleTimer.Stop();
      _player.Call("CastSpell");
      _battleSequenceRtl.Text += "You cast a spell at the " + _turnQueue.EnemyType + "\n";
      _playerActed = true;
      _hitTheTarget.minigameStart();
      UpdateEnemyHealth();
      DisplayPlayerOptions();
    }

    public void _on_DefendBtn_Pressed() {
      _battleTimer.Stop();
      _globalPlayer.isDefending = true;
      _battleSequenceRtl.Text += "You enter a defending stance" + "\n";
      _playerActed = true;
      _simon.startMinigame();
      UpdateEnemyHealth();
      DisplayPlayerOptions();
    }

    public void OnTimeout() {
      DisplayPlayerOptions();
      _battleSequenceRtl.Text += "You spent too long and the " + _turnQueue.EnemyType + " attacks!\n";
      _playerActed = true;
    }


    private void _on_Reset_gui_input(InputEvent @event) {
      if (_resetHovered && @event is InputEventMouseButton && @event.IsPressed()) {
        _globalPlayer.PlayerCharacter = null;
        _globalPlayer.EnemiesFought.Clear();
        _globalPlayer.Inventory.Clear();
        _globalPlayer.EquippedArmor = null;
        _globalPlayer.EquippedWeapon = null;
        GetTree().ChangeScene(_globalPlayer.lastScene);
      }
    }

    public void _on_Reset_mouse_entered() {
      _resetLabel.AddColorOverride("font_color", Colors.DarkRed);
      _resetHovered = true;
    }

    public void _on_Reset_mouse_exited() {
      _resetLabel.AddColorOverride("font_color", Colors.White);
      _resetHovered = false;
    }

    public void _on_Quit_gui_input(InputEvent @event) {
      if (_quitHovered && @event is InputEventMouseButton && @event.IsPressed()) {
        GetTree().Quit();
      }
    }

    public void _on_Quit_mouse_entered() {
      _quitLabel.AddColorOverride("font_color", Colors.DarkRed);
      _quitHovered = true;
    }

    public void _on_Quit_mouse_exited() {
      _quitLabel.AddColorOverride("font_color", Colors.White);
      _quitHovered = false;
    }
  }
}
