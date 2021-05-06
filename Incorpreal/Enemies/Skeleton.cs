using System;

namespace Incorpreal.Enemies {
  public class Skeleton : AbstractEnemy {

    public Skeleton() : 
      base(25, 5, 10, "Skeleton", String.Empty) {
    }

    protected override void ApplyStatusEffect() {
      if (!GlobalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        GlobalPlayer.PlayerCharacter.StatusEffect = "Slowing";
      }
    }
  }
}