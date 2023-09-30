using Godot;
using System;

public partial class ScoreLabel : Label
{
	int _score = 0;

	public void OnMobSquashed() => Text = $"Score: {++_score}";
}
