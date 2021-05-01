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


    //After the player gets out of range of the enemy and will continue to stay in that spot after 10 seconds if the enemy is no longer on screen then it is returned to the original starting position
    //and follows the original path if it has one by removing it from the "Following" group. If the enemy is still on screen the timer refreshes and will check again in another 10 seconds.
  }
}
