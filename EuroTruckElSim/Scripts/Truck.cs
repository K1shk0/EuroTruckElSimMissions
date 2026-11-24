using Godot;

public partial class Truck : CharacterBody2D
{
	[Export] public TruckHandling Handling;

	public override void _Ready()
	{
		Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = Handling.Image;
		// Keep the truck artwork inside its existing 200x80 parking footprint.
		if (sprite.Texture != null)
		{
			Vector2 size = sprite.Texture.GetSize();
			float scale = Mathf.Min(200f / size.X, 80f / size.Y);
			sprite.Scale = Vector2.One * scale;
			GetNode<ColorRect>("ColorRect").Visible = false;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float retning = Input.GetAxis("move_left", "move_right");

		Velocity = new Vector2(retning * Handling.Speed, 0);
		MoveAndSlide();
	}
}
