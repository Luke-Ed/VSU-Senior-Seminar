using Godot;
using System;
using Incorpreal;

public class TimingGame : Node2D {

    //private Boolean _isGood = false;
    //private Boolean _isPerfect = false;
    //private Timer _timer;
    private ColorRect _gamePage;
    private ColorRect _battlePage;
    private RichTextLabel _battleText;
    private GlobalPlayer _globalPlayer;
    private Boolean _playingMinigame = false;
    private TurnQueue _turnQueue;
    private Path2D _rythmPath;
    private PathFollow2D _rythmFollowPath;
    private Label _instructionLabel;
    private Tween _fadeTween;

    public override void _Ready() {
        _gamePage = GetNode<ColorRect>("GamePage");
        _battlePage = GetParent().GetNode<ColorRect>("BattlePage");
        _battleText = _battlePage.GetNode<RichTextLabel>("RichTextLabel");
        _globalPlayer = (GlobalPlayer)GetNode("/root/GlobalData");
        _turnQueue = (TurnQueue)GetNode("/root/Tq");
        _rythmPath = _gamePage.GetNode<Path2D>("Path2D");
        _rythmFollowPath = _rythmPath.GetNode<PathFollow2D>("PathFollow2D");
        _instructionLabel = _gamePage.GetNode<Label>("Instruction");
        _fadeTween = _instructionLabel.GetNode<Tween>("Tween");
        _fadeTween.InterpolateProperty(_instructionLabel, "modulate", Color.Color8(255, 255, 255, 255), Color.Color8(255, 255, 255, 0), 3, Tween.TransitionType.Linear, Tween.EaseType.Out);

    }

    public void startMinigame() {
        _fadeTween.Start();
        _gamePage.Visible = true;
        _battlePage.Visible = false;
        _playingMinigame = true;
        _rythmFollowPath.Offset = 0;
    }


    public void _on_Good_body_entered(Node body) {
        _globalPlayer.GoodHit = true;
    }

    public void _on_Good_body_exited(Node body) {
        _globalPlayer.GoodHit = false;
    }


    public void _on_Perfect_body_entered(Node body) {
        _globalPlayer.PerfectHit = true;
    }

    public void _on_Perfect_body_exited(Node body) {
        _globalPlayer.PerfectHit = false;
    }

    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("left_click") && _playingMinigame){
            Boolean _didHit = _globalPlayer.AttackEnemy();
            if (_didHit) {
                _battleText.Text += "You hit the " + _turnQueue.EnemyType + "\n";
                if (_globalPlayer.GoodHit) {
                    if (_globalPlayer.PerfectHit) {
                        _battleText.Text += "You timed your hit perfectly. \n";
                    }
                    else {
                        _battleText.Text += "You timed your hit well. \n";
                    }
                }
            }
            else {
                _battleText.Text += "You missed the " + _turnQueue.EnemyType + "\n";
            }
            _playingMinigame = false;
            _globalPlayer.GoodHit = false;
            _globalPlayer.PerfectHit = false;
            _gamePage.Visible = false;
            _battlePage.Visible = true;
        }
    }
}



