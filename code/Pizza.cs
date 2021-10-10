using Sandbox;

namespace Papa
{
	public class Pizza : ModelEntity
	{
		public override void Spawn()
		{
			base.Spawn();

			Tags.Add( "Pizza" );

			SetModel( "models/pizza.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			CollisionGroup = CollisionGroup.Debris;
		}

		protected override void OnPhysicsCollision( CollisionEventData eventData )
		{
			base.OnPhysicsCollision( eventData );

			if ( !IsServer )
				return;

			Particles.Create( "particles/explosion.vpcf", Position );
			Sound.FromWorld( "roblox_explosion", Position );

			var entities = Physics.GetEntitiesInSphere( Position, 400 );

			foreach ( Entity entity in entities )
			{
				if ( entity is Vehicle )
					((Vehicle)entity).CameraShake( 15 );

				if ( entity is Skeleton )
					((Skeleton)entity).Kill( Position, 2000 );
			}

			Delete();
		}
	}
}
