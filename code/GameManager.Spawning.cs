namespace MightyBrick.GraveQuest;

public partial class GameManager
{
	[Property, Category( "Spawning" )]
	public GameObject SkeletonPrefab { get; set; }
	public TimeUntil TimeUntilNextSkeletonSpawn { get; set; } = 0.0f;

	private float spawnRadius = 1550.0f;
	private Task skeletonSpawningTask;

	private async Task SpawnInitialSkeletons()
	{
		while ( Scene.NavMesh.IsGenerating )
			await Task.Yield();

		for ( int i = 0; i < 5; i++ )
		{
			SpawnSkeleton();
			await Task.DelaySeconds( Game.Random.Float( 0.1f, 0.2f ) );
		}
	}

	private void SpawnSkeletonOnTimer()
	{
		if ( Scene.NavMesh.IsGenerating || TimeUntilNextSkeletonSpawn > 0.0f )
			return;
		SpawnSkeleton();
		TimeUntilNextSkeletonSpawn = 2.0f;
	}

	private void SpawnSkeleton()
	{
		if ( !SkeletonPrefab.IsValid() )
			return;
		Vector3 spawnPosition = Scene.NavMesh.GetRandomPoint( Vector3.Zero, spawnRadius ) ?? Vector3.Zero;
		GameObject skeletonObject = SkeletonPrefab.Clone( spawnPosition );
	}
}
