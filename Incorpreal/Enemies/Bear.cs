using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bear : AbstractEnemy {
    public Bear() :
      base(150, 5, 60, "Bear", "Bleeding") {
    }
    
    protected override void ApplyStatusEffect() {
      if (!GlobalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        GlobalPlayer.PlayerCharacter.StatusEffect = "Bleeding";
      }
    }

  }
}
