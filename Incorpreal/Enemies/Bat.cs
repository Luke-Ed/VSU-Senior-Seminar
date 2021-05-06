using System;
using Godot;

namespace Incorpreal.Enemies {
  public class Bat : AbstractEnemy {

    public Bat() :
      base(50, 2, 30, "Bat", "Leeching") {
    }
    
    protected override void ApplyStatusEffect() {
      if (CurrentHealth + Attack <= Health) {
        CurrentHealth += Attack;
      }
    }
    
  }
}
