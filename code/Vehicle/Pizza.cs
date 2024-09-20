namespace MightyBrick.GraveQuest;

public sealed class Pizza : Component, Component.ITriggerListener
{
	[Property]
	public GameObject ExplosionPrefab { get; set; }
	[RequireComponent]
	public Rigidbody Rigidbody { get; private set; }
	[Property]
	public float ExplosionRadius { get; set; } = 250.0f;
	[Property]
	public SoundEvent ExplosionSound { get; set; }

	protected override void OnStart()
	{
		Rigidbody.PhysicsBody.SpeculativeContactEnabled = true;
	}

	public void OnTriggerEnter( Collider collider )
	{
		Explode();
	}

	private void Explode()
	{
		GameObject explosion = ExplosionPrefab.Clone( Transform.Position, Transform.Rotation );

		SceneTraceResult[] hitObjects = Scene.Trace.Sphere( ExplosionRadius, Transform.Position, Transform.Position )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "Player", "Car", "Pizza" )
			.HitTriggers()
			.RunAll()
			.ToArray();

		foreach ( SceneTraceResult hit in hitObjects )
		{
			Vector3 direction = Vector3.Direction( Transform.Position, hit.GameObject.Transform.Position );
			Vector3 force = direction * 5000.0f + Vector3.Up * 2000.0f;
			if ( hit.Body.IsValid() )
				hit.Body.ApplyImpulse( force );

			if ( hit.GameObject.Components.TryGet<Skeleton>( out Skeleton skeleton ) )
				skeleton.Kill( force, 1500.0f );
		}

		Sound.Play( ExplosionSound, Transform.Position );
		GameObject.Destroy();
	}
}
