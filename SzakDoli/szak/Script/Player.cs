using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private bool isAttacking;
	private bool attackHitboxActive; 
	private FighterState fighterState = FighterState.Idle;
	[Export] public float Speed { get; set; } = 250.0f;
	[Export] public CollisionShape2D AttackHitBox;
	[Export] public AnimatedSprite2D PlayerSprite;
	
	
	public override void _Ready()
	{
		if (PlayerSprite != null)
		{
		 	PlayerSprite.Play("Idle1");
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector(
			"move_left",
			"move_right",
			"move_up",
			"move_down"
		);
		
		Velocity = direction * Speed;
		MoveAndSlide();
	}
	
	public void Attack()
	{
		if(isAttacking || fighterState == FighterState.Dead) return;
		
		isAttacking = true;
		fighterState = FighterState.Attacking;
		AttackHitBox.SetDeferred("monitoring", false);
		
	}
}
