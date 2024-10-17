namespace MightyBrick.GraveQuest;

public sealed class EnemySpawner : Component
{
	[Property, Category( "Spawning" )]
	public GameObject SkeletonPrefab { get; set; }
	public TimeUntil TimeUntilNextSpawn { get; set; } = 0.0f;

	private float spawnRadius = 1550.0f;

	protected override void OnUpdate()
	{
		if ( !Active || Scene.NavMesh.IsGenerating || TimeUntilNextSpawn > 0.0f )
			return;
		SpawnEnemy();
		TimeUntilNextSpawn = 2.0f;
	}

	public void SpawnEnemies( int amount, float delayMin = 0.1f, float delayMax = 0.2f )
	{
		if ( !SkeletonPrefab.IsValid() )
			return;
		for ( int i = 0; i < amount; i++ )
		{
			float delay = (i * 0.25f) + Game.Random.Float( delayMin, delayMax );
			Invoke( delay, () => SpawnEnemy() );
		}
	}

	public void SpawnEnemy()
	{
		if ( !SkeletonPrefab.IsValid() )
			return;
		Vector3 spawnPosition = Scene.NavMesh.GetRandomPoint( Vector3.Zero, spawnRadius ) ?? Vector3.Zero;
		GameObject enemy = SkeletonPrefab.Clone( spawnPosition );
	}
}
