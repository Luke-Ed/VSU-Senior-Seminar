namespace Incorpreal.Enemies {
  public class Skeleton : AbstractEnemy {

    public Skeleton() : 
      base(25, 20, 10, "Skeleton", "Slowing") {
    }

    protected override void ApplyStatusEffect() {
      if (!GlobalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        GlobalPlayer.PlayerCharacter.StatusEffect = "Slowing";
      }
    }
  }
}