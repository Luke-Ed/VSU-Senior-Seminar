using Godot;

namespace Incorpreal {
  public class Target : StaticBody2D {
    public void Hit() {
      Visible = false;
    }
  }
}
