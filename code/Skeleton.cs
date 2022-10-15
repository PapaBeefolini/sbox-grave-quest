using Sandbox;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Papa
{
	public partial class Skeleton : AnimatedEntity
	{
		[ConVar.Replicated( "debug_skeleton" )]
		public static bool debug_skeleton { get; set; } = false;

		private TraceResult groundTrace;

		private List<Vector3> Points = new List<Vector3>();
		private int CurrentPoint = 0;

		private string chosenHat = string.Empty;

		private bool spawning = true;

		public override void Spawn()
		{
			base.Spawn();

			Tags.Add( "skeleton" );

			SetModel( "models/skeleton.vmdl" );
			SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 100, 16 ) );

			_ = Spawning();

			WearHat();
		}

		async Task Spawning()
		{
			spawning = true;
			CurrentSequence.Name = "spawn";
			await Task.DelaySeconds( 1.0f );
			spawning = false;
			CurrentSequence.Name = "run";
		}

		private void GetPath()
		{
			if ( Points.Count > 0 && CurrentPoint < Points.Count )
			{
				if ( Position.AlmostEqual( Points[CurrentPoint], 30 ) )
					CurrentPoint++;

				return;
			}
			else
			{
				Points.Clear();

				CurrentPoint = 0;

				Vector3 target = NavMesh.GetPointWithinRadius( groundTrace.EndPosition, 1000, 5000 ).Value;
				NavMesh.BuildPath( groundTrace.EndPosition, target, Points );
			}
		}

		[Event.Tick.Server]
		public void Tick()
		{
			if ( spawning )
				return;

			groundTrace = Trace.Ray( Position + Vector3.Up * 10, Position + Vector3.Down * 40 )
				.Ignore( this )
				.WithoutTags( "Skeleton" )
				.WithoutTags( "Vehicle" )
				.WithoutTags( "Pizza" )
				.Run();

			if ( groundTrace.Hit )
			{
				GetPath();

				if ( CurrentPoint >= Points.Count )
					return;

				Position = groundTrace.EndPosition;
				Position += Rotation.Forward * 200 * Time.Delta;

				Vector3 direction = (Points[CurrentPoint] - Position);
				Rotation rotation = direction.EulerAngles.ToRotation();
				rotation.x = 0; rotation.y = 0;
				Rotation = Rotation.Slerp( Rotation, rotation, 6 * Time.Delta );
			}
			else
			{
				Position += Vector3.Down * 200 * Time.Delta;
			}

			//----------//

			if ( debug_skeleton )
			{
				if ( groundTrace.Hit )
					DebugOverlay.Line( groundTrace.StartPosition, groundTrace.EndPosition, Color.Green );
				else
					DebugOverlay.Line( groundTrace.StartPosition, groundTrace.EndPosition, Color.Red );

				foreach ( Vector3 point in Points )
					DebugOverlay.Sphere( point, 25, Color.Blue );

				if ( CurrentPoint < Points.Count )
					DebugOverlay.Sphere( Points[CurrentPoint], 30, Color.Green );
			}
		}

		private void WearHat()
		{
			// Random chance to not even wear a hat.
			if ( Rand.Int( 0, 3 ) > 1 )
				return;

			string[] hats =
			{
				"models/hat_archer.vmdl",
				"models/hat_medieval_01.vmdl",
				"models/hat_medieval_02.vmdl",
				"models/hat_pumpkin.vmdl",
				"models/hat_witch.vmdl",
			};

			chosenHat = hats[Rand.Int( 0, hats.Length -1 )];

			ModelEntity hat = new ModelEntity( chosenHat );
			hat.SetParent( this, true );
		}

		public void Kill(Vector3 hitPosition, float speed)
		{
			Event.Run( "papa.skeletonkilled" );

			float hitVolume = 1.0f + (speed / 750);
			Sound.FromWorld( "crash", hitPosition ).SetPitch( Rand.Float( 0.8f, 1.2f ) ).SetVolume( hitVolume );
			Sound.FromWorld( "skeleton_death", hitPosition ).SetPitch( Rand.Float( 0.8f, 1.2f ) );

			if ( speed < 1400 )
			{
				ModelEntity ragdoll = new ModelEntity( "models/skeleton.vmdl" );
				ragdoll.Tags.Add( "ragdoll" );
				ragdoll.Transform = Transform;
				ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				ragdoll.CopyBonesFrom( this );

				ragdoll.Velocity = (Position - hitPosition) * 20;
				ragdoll.DeleteAsync( 20.0f );
			}
			else
			{
				Sound.FromWorld( "explode", hitPosition ).SetPitch( Rand.Float( 0.8f, 1.2f ) ).SetVolume( hitVolume / 2 );

				string[] mdls = 
				{
					"models/skeleton_head.vmdl",
					"models/skeleton_torso.vmdl",
					"models/skeleton_arm_l.vmdl",
					"models/skeleton_arm_r.vmdl",
					"models/skeleton_leg_l.vmdl",
					"models/skeleton_leg_r.vmdl"
				};

				for ( int i = 0; i < mdls.Length; i++ )
				{
					ModelEntity mdl = new ModelEntity( mdls[i] );
					mdl.Tags.Add( "ragdoll" );
					mdl.Transform = Transform;
					mdl.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );

					mdl.Velocity = (Position - hitPosition) * 4;
					mdl.DeleteAsync( 20.0f );
				}
			}

			if ( chosenHat != string.Empty )
			{
				ModelEntity hat = new ModelEntity( chosenHat );
				hat.Tags.Add( "ragdoll" );
				hat.Transform = Transform;
				hat.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				hat.Velocity = (Position - hitPosition) * 10 + Vector3.Up * 8;
				hat.ApplyLocalAngularImpulse( (Position - hitPosition) * 10 );
				hat.DeleteAsync( 20.0f );
			}

			Delete();
		}
	}
}
