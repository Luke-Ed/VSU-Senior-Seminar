using System; 
using Godot;

namespace Incorpreal.Enemies {
  public class Snake : AbstractEnemy {

    public Snake() :
      base(75, 4, 10, "Snake", "Poisoned") {
    }

    protected override void ApplyStatusEffect() {
      if (!GlobalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        GlobalPlayer.PlayerCharacter.StatusEffect = "Poisoned";
      }
    }
  }
}