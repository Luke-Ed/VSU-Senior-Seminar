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

    public override void _Ready() {
      _simon = (Simon)GetNode("SimonGame");
      _hitTheTarget = (HitTheTarget_Engan)GetNode("HitTheTarget_Engan");
      _battlePage = GetNode<ColorRect>("BattlePage");
      _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
      _turnQueue = (TurnQueue)GetNode("/root/Tq");
      _turnQueue.combatants = _turnQueue.getCombatants();
      _turnQueue.setStats();
      _player = (Node)_turnQueue.combatants[0] as KinematicBody2D;
      _enemy = (Node)_turnQueue.combatants[1] as KinematicBody2D;
      _playerHp = _battlePage.GetNode<Label>("HealthLabel");
      _enemyHp = _battlePage.GetNode<Label>("EnemyHealth");
      _battleSequenceRtl = _battlePage.GetNode<RichTextLabel>("RichTextLabel");
      _battleSequenceRtl.Text = "You have encountered a(n): " + _turnQueue.enemyName + "\n";
      _attackBtn = _battlePage.GetNode<Button>("Attackbtn");
      _spellBtn = _battlePage.GetNode<Button>("Spellbtn");
      _defendBtn = _battlePage.GetNode<Button>("DefendBtn");
      _battleTimer = _battlePage.GetNode<Timer>("Timer");
      _battleTimer.WaitTime = 20;
      _battleTimer.Connect("timeout", this, "onTimeout");
      _globalPlayer.updateHealthLabel(_playerHp);
      UpdateEnemyHealth();
    }

    private void UpdateEnemyHealth() {
      String text = "Enemy Health: " + _turnQueue.enemyCurrentHP + "/" + _turnQueue.enemyMaxHP;
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
          default:
            // If the current fighter is the enemy.
            Boolean didHit = (bool)_enemy.Call("playTurn");
            if (didHit) {
              _battleSequenceRtl.Text += "You got hit by the " + _turnQueue.enemyName + "\n";
              if (_globalPlayer.didBlock) {
                _battleSequenceRtl.Text += "but you blocked perfectly!" + "\n";
              }
            }
            else {
              _battleSequenceRtl.Text += "The attack passed through you" + "\n";
            }
            _globalPlayer.updateHealthLabel(_playerHp);
            break;
        }
        
        ChangeActiveFighter();
        if (_globalPlayer.CurrentHealth <= 0 || _turnQueue.enemyCurrentHP <= 0) {
          if (_globalPlayer.CurrentHealth <= 0) {
            _battleSequenceRtl.Text += "You have lost the fight";
            _playerActed = false;
            GetNode<ColorRect>("DeathScreen").Visible = true;
          }
          else {
            _battleSequenceRtl.Text += "You have won the fight";
            _globalPlayer.Experience += 10;
            if(_globalPlayer.Experience >= _globalPlayer.ExperienceToNextLevel)
            {
              GetTree().ChangeScene("res://LevelUp.tscn");
            }
          }
          _fightOver = true;
        }
      }
      else
      {
        GetTree().ChangeScene(_globalPlayer.lastScene);
      }
    }

    private void StartPlayerTurn() {
      _globalPlayer.isDefending = false;
      _globalPlayer.didBlock = false;
      _playerActed = false;
      DisplayPlayerOptions();
      _battleSequenceRtl.Text += "Choose an action \n";
      _battleTimer.Start();
    }

    private void DisplayPlayerOptions() {
      _attackBtn.Visible = !_attackBtn.Visible;
      _spellBtn.Visible = !_spellBtn.Visible;
      if (_globalPlayer.currentPoints < 5) {
        _spellBtn.Disabled = true;
      }
      else {
        _spellBtn.Disabled = false;
      }
      _defendBtn.Visible = !_defendBtn.Visible;
    }

    private void ChangeActiveFighter() {
      _activeFighter = (_activeFighter + 1) % _turnQueue.combatants.Count;
    }

    public override void _Input(InputEvent @event) {
      if (_playerActed && _battlePage.Visible) {
        if (@event.IsActionPressed("Continue")) {
          Fight();
        }
      }
    }

    public void _on_Attackbtn_pressed() {
      _battleTimer.Stop();
      Boolean didHit = (bool)_player.Call("attackEnemy");
      if (didHit) {
        _battleSequenceRtl.Text += "You hit the " + _turnQueue.enemyName + "\n";
      }
      else {
        _battleSequenceRtl.Text += "You missed the " + _turnQueue.enemyName + "\n";
      }
      _playerActed = true;
      UpdateEnemyHealth();
      DisplayPlayerOptions();
    }

    public void _on_Spellbtn_pressed() {
      _battleTimer.Stop();
      _player.Call("castSpell");
      _battleSequenceRtl.Text += "You cast a spell at the " + _turnQueue.enemyName + "\n";
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

    public void onTimeout() {
      DisplayPlayerOptions();
      _battleSequenceRtl.Text += "You spent too long and the " + _turnQueue.enemyName + " attacks!\n";
      _playerActed = true;
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
