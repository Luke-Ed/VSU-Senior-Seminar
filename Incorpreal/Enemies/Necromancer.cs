using System;

namespace Incorpreal.Enemies {
  public class Necromancer : AbstractEnemy {
    public Necromancer() : base(100, 5, 100, "Necromancer", String.Empty) {
    }
    
    protected override void ApplyStatusEffect() {
      
    }
  }
}