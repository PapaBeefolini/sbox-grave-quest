namespace MightyBrick.GraveQuest;

public sealed class Pizza : Component, Component.ITriggerListener
{
	[Property]
	public GameObject ExplosionPrefab { get; set; }
	[RequireComponent]
	public Rigidbody Rigidbody { get; private set; }
	[Property]
	public float ExplosionRadius { get; set; } = 400.0f;
	[Property]
	public float LethalRadius { get; set; } = 200.0f;
	[Property]
	public SoundEvent ExplosionSound { get; set; }

	protected override void OnStart()
	{
		GameObject.DestroyDelayed( 10.0f );
	}

	public void OnTriggerEnter( Collider collider )
	{
		Explode();
	}

	private void Explode()
	{
		GameObject explosion = ExplosionPrefab.Clone( Transform.Position, Rotation.FromPitch( -90 ) );

		SceneTraceResult[] hitObjects = Scene.Trace.Sphere( ExplosionRadius, Transform.Position, Transform.Position )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "Player", "Pizza" )
			.HitTriggers()
			.RunAll()
			.ToArray();

		foreach ( SceneTraceResult hit in hitObjects )
		{
			Vector3 direction = Vector3.Direction( Transform.Position, hit.GameObject.Transform.Position );
			Vector3 force = direction * 5000.0f + Vector3.Up * 2000.0f;
			float distance = Transform.Position.Distance( hit.GameObject.Transform.Position );

			if ( hit.Body.IsValid() )
				hit.Body.ApplyImpulse( force );

			if ( hit.GameObject.Components.TryGet<Skeleton>( out Skeleton skeleton ) && distance < LethalRadius )
				skeleton.Kill( force, 1500.0f );

			if ( hit.GameObject.Components.TryGet<Vehicle>( out Vehicle vehicle ) )
			{
				float normalizedDistance = 0.2f + MathX.Clamp( 1.0f - (distance / ExplosionRadius), 0.0f, 1.0f );
				float shakeIntensity = MathX.Lerp( 2.0f, 8.0f, normalizedDistance );
				vehicle.ShakeCamera( 1.25f, shakeIntensity );
			}
		}

		Sound.Play( ExplosionSound, Transform.Position );
		GameObject.Destroy();
	}
}
