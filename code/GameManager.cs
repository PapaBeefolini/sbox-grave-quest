using Sandbox;
using Sandbox.Navigation;
using System.Threading.Tasks;

namespace MightyBrick.GraveQuest;

public sealed class GameManager : Component
{
	[Property]
	public GameObject SkeletonPrefab { get; set; }
	public TimeUntil TimeUntilNextSkeletonSpawn { get; set; } = 0.0f;

	private float spawnRadius = 1550.0f;

	protected override void OnStart()
	{
		_ = SpawnInitialSkeletons();
	}

	protected override void OnUpdate()
	{
		//Gizmo.Draw.LineSphere( Vector3.Zero, spawnRadius );

		if ( Scene.NavMesh.IsGenerating )
			return;

		if ( TimeUntilNextSkeletonSpawn > 0.0f )
			return;

		SpawnSkeleton();
		TimeUntilNextSkeletonSpawn = 2.0f;
	}

	private async Task SpawnInitialSkeletons()
	{
		while ( Scene.NavMesh.IsGenerating )
			await Task.Yield();

		for ( int i = 0; i < 5; i++ )
			SpawnSkeleton();
	}

	private void SpawnSkeleton()
	{
		if ( !SkeletonPrefab.IsValid() )
			return;
		Vector3 spawnPosition = Scene.NavMesh.GetRandomPoint( Vector3.Zero, spawnRadius ) ?? Vector3.Zero;
		GameObject skeletonObject = SkeletonPrefab.Clone( spawnPosition );
	}
}
