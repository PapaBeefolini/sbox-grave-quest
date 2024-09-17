using Sandbox;
using Sandbox.Events;
using Sandbox.Physics;
using System;

namespace MightyBrick.GraveQuest;

public partial class Skeleton : Minifig, Component.ITriggerListener
{
	[RequireComponent]
	public ModelPhysics ModelPhysics { get; private set; }

	public bool IsDead { get; private set; }

	protected override void OnStart()
	{
		ModelPhysics.Enabled = false;

		// 50/50 chance at wearing a hat
		if ( Game.Random.Int( 1 ) == 0 )
			WearRandomHat();

		//Renderer.SceneObject.Attributes.Set( "SkeletonTint", Color.Random );
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
		Vector3 hitForce = Vector3.Direction( Transform.Position, hitVehicle.Transform.Position ) * -5000.0f + Vector3.Up * 5000.0f;
		Die( hitForce, hitVehicle.LocalVelocity.Length );
	}

	private void Die( Vector3 force, float hitSpeed )
	{
		if ( IsDead )
			return;

		Renderer.UseAnimGraph = false;
		Ragdoll( force, hitSpeed > 900.0f );
		Agent.Destroy();

		IsDead = true;
		Scene.Dispatch( new SkeletonDiedEvent() );
	}


	private void Ragdoll( Vector3 force, bool breakApart )
	{
		ModelPhysics.Enabled = true;
		ModelPhysics.MotionEnabled = true;
		ModelPhysics.PhysicsGroup.Mass = 150.0f;
		ModelPhysics.PhysicsGroup.ApplyImpulse( force );
		HatRenderer.GameObject.SetParent( null );
		HatRenderer.BoneMergeTarget = null;
		HatCollider.Enabled = true;
		HatRigidbody.MotionEnabled = true;

		if ( breakApart )
		{
			foreach ( PhysicsJoint joint in ModelPhysics.PhysicsGroup.Joints )
			{
				joint.Strength = 5000.0f;
			}
		}
	}
}
