using Sandbox;
using Sandbox.Events;
using Sandbox.Physics;
using System;

namespace MightyBrick.GraveQuest;

public partial class Skeleton : Minifig, Component.ITriggerListener
{
	[RequireComponent]
	public ModelPhysics ModelPhysics { get; private set; }
	[Property]
	public SoundEvent CrashSound { get; private set; }
	[Property]
	public SoundEvent DeathSound { get; private set; }
	[Property]
	public SoundEvent BreakSound { get; private set; }

	public bool IsDead { get; private set; }

	protected override void OnStart()
	{
		if ( !IsDead )
			ModelPhysics.Enabled = false;

		// 50% chance at wearing a hat
		if ( Game.Random.Int( 1 ) == 0 )
			WearRandomHat();

		// 25% chance at being a dark skeleton
		if ( Game.Random.Int( 3 ) == 0 )
			Renderer.MaterialGroup = "dark";
	}

	protected override void OnUpdate()
	{
		if ( !Agent.IsValid() )
			return;

		Move();
		Animate();
	}

	public void OnTriggerEnter( Collider collider )
	{
		Vehicle hitVehicle = collider.GameObject.Components.Get<Vehicle>();
		if ( !hitVehicle.IsValid() )
			return;
		Vector3 direction = Vector3.Direction( hitVehicle.Transform.Position, Transform.Position );
		Vector3 force = direction * hitVehicle.LocalVelocity.Length * 4.0f + Vector3.Up * 2000.0f;
		Die( force, hitVehicle.LocalVelocity.Length );
		Sound.Play( CrashSound, Transform.Position );
	}

	private void Die( Vector3 force, float hitSpeed )
	{
		if ( IsDead )
			return;

		Renderer.UseAnimGraph = false;
		Ragdoll( force, hitSpeed > 1000.0f );
		Agent.Destroy();

		Sound.Play( DeathSound, Transform.Position );

		IsDead = true;
		Scene.Dispatch( new SkeletonDiedEvent() );

		HatRenderer.GameObject.DestroyDelayed( 10.0f, true );
		GameObject.DestroyDelayed( 10.0f, true );
	}


	private void Ragdoll( Vector3 force, bool breakApart )
	{
		ModelPhysics.Enabled = true;
		ModelPhysics.MotionEnabled = true;
		ModelPhysics.PhysicsGroup.Mass = 50.0f;
		ModelPhysics.PhysicsGroup.ApplyImpulse( force );
		HatRenderer.GameObject.SetParent( null );
		HatRenderer.BoneMergeTarget = null;
		HatCollider.Enabled = true;
		HatRigidbody.MotionEnabled = true;

		foreach ( PhysicsJoint joint in ModelPhysics.PhysicsGroup.Joints )
		{
			float strength = 4500.0f;
			if ( breakApart )
			{
				strength = 500.0f;
				Sound.Play( BreakSound, Transform.Position );
			}
			joint.Strength = strength;
		}
	}
}
