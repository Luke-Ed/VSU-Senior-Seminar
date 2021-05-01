using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bear : AbstractEnemy {
    private GlobalPlayer _globalPlayer;

    public Bear() :
      base(150, 5, 60, "Bear", "Bleeding") {
    }



    protected override void ApplyStatusEffect() {
      if (!_globalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        _globalPlayer.PlayerCharacter.StatusEffect = "Bleeding";
      }
    }

  }
}
