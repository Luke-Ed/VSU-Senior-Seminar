using System; 
using Godot;

namespace Incorpreal.Enemies {
  public class Snake : AbstractEnemy {
    private GlobalPlayer _globalPlayer;

    public Snake() :
      base(75, 4, 10, "Snake", "Poisoned") {
    }

    public override void _PhysicsProcess(float delta) {
      
    }

    public override void _Ready() {
      _globalPlayer = (GlobalPlayer) GetNode("/root/GlobalData");
    }

    protected override void ApplyStatusEffect() {
      if (!_globalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        _globalPlayer.PlayerCharacter.StatusEffect = "Poisoned";
      }
    }
  }
}