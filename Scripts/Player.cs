using Godot;

namespace Game;

public partial class Player : CharacterBody3D
{
	[Signal] public delegate void HitEventHandler();

	[Export] public int Speed { get; set; } = 14;
	[Export] public int FallAcceleration { get; set; } = 75;
	[Export] public int JumpImpulse { get; set; } = 20;
	[Export] public int BounceImpulse { get; set; } = 16;

	Node3D _pivot;
	AnimationPlayer _animationPlayer;

	private Vector3 _targetVelocity = Vector3.Zero;

	public override void _Ready()
	{
		_pivot = GetNode<Node3D>("Pivot");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		var direction = Vector3.Zero;

		if (Input.IsActionPressed("move_right"))
			direction.X += 1f;
		if (Input.IsActionPressed("move_left"))
			direction.X -= 1f;
		if (Input.IsActionPressed("move_back"))
			direction.Z += 1f;
		if (Input.IsActionPressed("move_forward"))
			direction.Z -= 1f;

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			_pivot.LookAt(Position + direction, Vector3.Up);
			_animationPlayer.SpeedScale = 4;
		}
		else _animationPlayer.SpeedScale = 1;

		_targetVelocity.X = direction.X * Speed;
		_targetVelocity.Z = direction.Z * Speed;

		if (!IsOnFloor())
		{
			_targetVelocity.Y -= FallAcceleration * (float)delta;
			_pivot.Rotation = new Vector3(Mathf.Pi / 6.0f * Velocity.Y / JumpImpulse, _pivot.Rotation.Y, _pivot.Rotation.Z);
		}
		else if (Input.IsActionJustPressed("jump"))
			_targetVelocity.Y = JumpImpulse;

		for (var index = 0; index < GetSlideCollisionCount(); index++)
		{
			var collision = GetSlideCollision(index);

			if (collision.GetCollider() is Mob mob)
				if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
				{
					mob.Squash();
					_targetVelocity.Y = BounceImpulse;
				}
		}

		Velocity = _targetVelocity;
		MoveAndSlide();
	}

	void Die()
	{
		EmitSignal(SignalName.Hit);
		QueueFree();
	}

	void OnMobDetectorBodyEntered(Node3D _) => Die();
}