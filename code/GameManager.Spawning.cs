﻿using System.Threading.Tasks;

namespace MightyBrick.GraveQuest;

public partial class GameManager
{
	[Property]
	public GameObject SkeletonPrefab { get; set; }
	public TimeUntil TimeUntilNextSkeletonSpawn { get; set; } = 0.0f;

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