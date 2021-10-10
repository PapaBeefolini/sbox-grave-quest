using Sandbox;
using System.Threading;
using System.Threading.Tasks;

namespace Papa
{
	public partial class GraveQuest : Game
	{
		public static GraveQuest Instance
		{
			get => Current as GraveQuest;
		}

		public enum GameState { Inactive, Active, Over };
		[Net] public GameState State { get; private set; }

		[Net] public float TimeLeft { get; private set; }
		[Net] public int SkeletonsKilled { get; private set; }

		private float lastSkeleton;

		public GraveQuest()
		{
			if ( IsServer )
			{
				new UI.GameUI();
			}

			if ( IsClient )
			{
				Sound.FromScreen( "spooker" );
			}
		}

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			Vehicle vehicle = new Vehicle();
			client.Pawn = vehicle;

			if ( IsServer && client.IsListenServerHost )
			{
				StartNewGame();
			}
		}

		[Event.Tick.Server]
		public void Tick()
		{
			if ( State == GameState.Active )
			{
				TimeLeft -= Time.Delta;
				lastSkeleton += Time.Delta;

				if ( lastSkeleton > 2 )
				{
					SpawnSkeleton();
					lastSkeleton = 0.0f;
				}

				if ( TimeLeft <= 1 )
				{
					_ = EndGame();
				}
			}
		}

		private void StartNewGame()
		{
			State = GameState.Inactive;

			TimeLeft = 12.0f;
			SkeletonsKilled = 0;

			for ( int i = 0; i < 10; i++ )
			{
				SpawnSkeleton();
			}

			_ = StartCountDown();
		}

		async Task EndGame()
		{
			State = GameState.Over;

			await Task.DelaySeconds( 5.0f );

			foreach ( Entity entity in All )
			{
				if ( entity is Skeleton || entity is Pizza || entity.Tags.Has( "Ragdoll" ) )
					entity.Delete();

				if ( entity is Vehicle )
				{
					MoveToSpawnpoint( entity );
					entity.Velocity = Vector3.Zero;
					entity.AngularVelocity = Angles.Zero;
				}
			}

			StartNewGame();
		}

		async Task StartCountDown()
		{
			await Task.DelaySeconds( 2.0f );

			for ( int i = 0; i < 3; i++ )
			{
				Sound.FromScreen( "321" );
				await Task.DelaySeconds( 1.0f );
			}

			Sound.FromScreen( "go" );

			State = GameState.Active;
		}

		private void SpawnSkeleton()
		{
			Skeleton skele = new Skeleton();
			skele.Position = NavMesh.GetPointWithinRadius( Vector3.Zero, 1000, 10000 ).Value;
			skele.Rotation = Rotation.FromYaw( Rand.Float( -180, 180 ) );
		}

		[Event("papa.skeletonkilled")]
		public void OnSkeletonKilled()
		{
			if ( State != GameState.Active )
				return;
			SkeletonsKilled++;
			TimeLeft += 2.0f;
		}
	}
}
