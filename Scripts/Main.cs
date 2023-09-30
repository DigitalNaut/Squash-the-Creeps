using Godot;

namespace Game;

public partial class Main : Node
{
	PathFollow3D _mobSpawnLocation;
	Timer _mobTimer;
	Player _player;
	ScoreLabel _scoreLabel;
	Control _retryOverlay;

	[Export] public PackedScene MobScene { get; set; }

	public override void _Ready()
	{
		_mobSpawnLocation = GetNode<PathFollow3D>("MobSpawnPath/MobSpawnLocation");
		_mobTimer = GetNode<Timer>("MobTimer");
		_player = GetNode<Player>("Player");
		_scoreLabel = GetNode<ScoreLabel>("UserInterface/ScoreLabel");
		_retryOverlay = GetNode<Control>("UserInterface/Retry");

		_retryOverlay.Hide();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept") && _retryOverlay.Visible)
			GetTree().ReloadCurrentScene();
	}

	void OnMobTimerTimeout()
	{
		var newMob = MobScene.Instantiate<Mob>();
		_mobSpawnLocation.ProgressRatio = GD.Randf();
		var playerPosition = _player.Position;

		newMob.Initialize(_mobSpawnLocation.Position, playerPosition);

		AddChild(newMob);

		newMob.Squashed += _scoreLabel.OnMobSquashed;
	}

	void OnPlayerHit()
	{
		_mobTimer.Stop();
		_retryOverlay.Show();
	}
}