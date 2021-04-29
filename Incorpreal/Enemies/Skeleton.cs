namespace Incorpreal.Enemies {
  public class Skeleton : AbstractEnemy {
    private GlobalPlayer _globalPlayer;
    
    public Skeleton() : 
      base(25, 20, 10, "Skeleton", "Slowing") {
    }

    public override void _Ready() {
      _globalPlayer = (GlobalPlayer) GetNode("/root/GlobalData");
    }

    protected override void ApplyStatusEffect() {
      if (!_globalPlayer.PlayerCharacter.StatusEffect.Equals(StatusEffect)) {
        _globalPlayer.PlayerCharacter.StatusEffect = "Bleeding";
      }
    }
  }
}