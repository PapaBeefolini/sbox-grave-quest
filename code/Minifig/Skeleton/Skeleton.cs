using Sandbox.Physics;

namespace MightyBrick.GraveQuest;

public record SkeletonDiedEvent() : IGameEvent;

public partial class Skeleton : Minifig
{
	[RequireComponent]
	public CapsuleCollider Collider { get; private set; }
	[Property, Category( "Sounds" )]
	public SoundEvent DeathSound { get; private set; }
	[Property, Category( "Sounds" )]
	public SoundEvent BreakSound { get; private set; }

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
	}

	protected override void OnUpdate()
	{
		if ( !Agent.IsValid() )
			return;

		Move();
		Animate();
	}

	public void Kill( Vector3 force, float hitSpeed )
	{
		if ( IsDead )
			return;

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
		physics.PhysicsGroup.Mass = 50.0f;
		physics.PhysicsGroup.ApplyImpulse( force );

		foreach ( PhysicsJoint joint in physics.PhysicsGroup.Joints )
		{
			float strength = 4500.0f;
			if ( breakApart )
			{
				strength = 500.0f;
				Sound.Play( BreakSound, WorldPosition );
			}
			joint.Strength = strength;
		}
	}
}
