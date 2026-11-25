using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private bool isAttacking;
	private bool attackHitboxActive; 
	private bool attackAnimationStarted;
	private bool facingRight = true;
	private FighterState fighterState = FighterState.Idle;
	
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback animationStateMachine;
	
	[Export] public float Speed { get; set; } = 250.0f;
	[Export] public CollisionShape2D AttackHitBoxRight;
	[Export] public CollisionShape2D AttackHitBoxLeft;
	//[Export] public AnimatedSprite2D PlayerSprite;
	
	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationStateMachine = 
		(AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		
		animationTree.Active = true;
		animationStateMachine.Start("Idle_Right");

		DisableAttackHitboxes();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(isAttacking)
		{
			StringName currentAnimationState = animationStateMachine.GetCurrentNode();
			bool inAttackState = 
				currentAnimationState == "Attack_Right" ||
				currentAnimationState == "Attack_Left";
			
			if(inAttackState)
			{
				attackAnimationStarted = true;
			}
			if(attackAnimationStarted && !inAttackState)
			{
				DisableAttackHitboxes();
				isAttacking = false;
				attackAnimationStarted = false;
				fighterState = FighterState.Idle;
			}
			else
			{
				Velocity = Vector2.Zero;
				MoveAndSlide();
				return;	
			}
		}
		
		Vector2 direction = Input.GetVector(
			"move_left",
			"move_right",
			"move_up",
			"move_down"
		);
		
		if(direction.X > 0)
		{
			facingRight = true;
		}
		else if(direction.X < 0)
		{
			facingRight = false;
		}
		
		Velocity = direction * Speed;
		MoveAndSlide();
		
		if(direction != Vector2.Zero)
		{
			animationStateMachine.Travel(
				facingRight ? "Walk_Right" : "Walk_Left");
		}
		else
		{
			animationStateMachine.Travel(
				facingRight? "Idle_Right" : "Idle_Left");
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("attack"))
		{
			Attack();
		}
	}
	
	public void Attack()
	{
		if(isAttacking || fighterState == FighterState.Dead) return;
		
		isAttacking = true;
		attackAnimationStarted = false;
		fighterState = FighterState.Attacking;
		
		animationStateMachine.Travel(
			facingRight ? "Attack_Right" : "Attack_Left");
	}

	public void DisableAttackHitboxes()
	{
		AttackHitBoxRight.SetDeferred("disabled", true);
		AttackHitBoxLeft.SetDeferred("disabled", true);
	}

	public void EnableAttackHitbox()
	{
		DisableAttackHitboxes();

		if (!isAttacking)
		{
			return;
		}

		if (facingRight)
		{
			AttackHitBoxRight.SetDeferred("disabled", false);
		}

		else
		{
			AttackHitBoxLeft.SetDeferred("disabled", false);
		}
	}
}
