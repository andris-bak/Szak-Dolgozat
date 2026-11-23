using Godot;
using System;

public partial class Player : CharacterBody2D
{
	private bool isAttacking;
	private bool attackHitboxActive; 
	private FighterState fighterState = FighterState.Idle;
	private bool attackAnimationStarted;
	
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback animationStateMachine;
	
	[Export] public float Speed { get; set; } = 250.0f;
	[Export] public CollisionShape2D AttackHitBox;
	[Export] public AnimatedSprite2D PlayerSprite;
	
	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("AnimationTree");
		animationStateMachine = 
		(AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		
		animationTree.Active = true;
		animationStateMachine.Start("Idle_Right");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(isAttacking)
		{
			StringName currentAnimationState = animationStateMachine.GetCurrentNode();
			
			if(currentAnimationState == "Attack_Right")
			{
				attackAnimationStarted = true;
			}
			if(attackAnimationStarted && currentAnimationState != "Attack_Right")
			{
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
		
		Velocity = direction * Speed;
		MoveAndSlide();
		
		if(direction != Vector2.Zero)
		{
			animationStateMachine.Travel("Walk_Right");
		}
		else
		{
			animationStateMachine.Travel("Idle_Right");
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
		
		animationStateMachine.Travel("Attack_Right");
		
	}
}
