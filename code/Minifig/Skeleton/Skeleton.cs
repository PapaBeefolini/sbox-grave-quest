using Sandbox.Physics;

namespace MightyBrick.GraveQuest;

public record SkeletonDiedEvent() : IGameEvent;

public partial class Skeleton : Minifig, Component.ITriggerListener
{
	[RequireComponent] public CapsuleCollider Collider { get; private set; }
	[Property, Category( "Sounds" )] public SoundEvent DeathSound { get; private set; }
	[Property, Category( "Sounds" )] public SoundEvent BreakSound { get; private set; }

	public bool IsDead { get; private set; }

	protected override void OnAwake()
	{
		base.OnAwake();

		// 50% chance at wearing a hat
		if ( Game.Random.Int( 1 ) == 0 )
			WearRandomHat();

		// 25% chance at being a dark skeleton
		if ( Game.Random.Int( 3 ) == 0 )
			Renderer.MaterialGroup = "dark";

		SetRandomMoveSpeed();

		Collider.Enabled = false;
		Invoke( 1.75f, () => { Collider.Enabled = true; } );
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
		Vehicle vehicle = collider.GameObject.GetComponent<Vehicle>();
		if ( !vehicle.IsValid() )
			return;

		float collisionVelocity = vehicle.LocalVelocity.Length;
		Vector3 direction = Vector3.Direction( collider.WorldPosition, WorldPosition );
		Vector3 force = direction * collisionVelocity * 4.0f + Vector3.Up * 2000.0f;

		Kill( force, collisionVelocity );
		vehicle.DoCrashEffects();
	}

	public void Kill( Vector3 force, float hitSpeed )
	{
		if ( IsDead )
			return;

		GameObject.BreakFromPrefab();
		Renderer.UseAnimGraph = false;
		Collider.Destroy();
		Agent.Destroy();

		Ragdoll( force, hitSpeed > 1000.0f );
		DropHat();

		Sound.Play( DeathSound, WorldPosition );

		IsDead = true;
		Scene.Dispatch( new SkeletonDiedEvent() );

		HatRenderer.GameObject.DestroyDelayed( 10.0f, true );
		GameObject.DestroyDelayed( 10.0f, true );
	}


	private void Ragdoll( Vector3 force, bool breakApart )
	{
		ModelPhysics physics = Components.Create<ModelPhysics>();
		physics.Renderer = Renderer;
		physics.Model = Renderer.Model;
		foreach ( ModelPhysics.Body body in physics.Bodies )
		{
			body.Component.MassOverride = 10.0f;
			body.Component.ApplyImpulse( force );
			body.Component.AngularVelocity = Game.Random.Float( -100.0f, 100.0f );
		}
		foreach ( Joint joint in physics.GetComponentsInChildren<Joint>() )
		{
			float strength = 4500.0f;
			if ( breakApart )
			{
				strength = 500.0f;
				Sound.Play( BreakSound, WorldPosition );
			}
			joint.BreakForce = strength;
		}
	}
}
