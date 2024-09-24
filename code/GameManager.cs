namespace MightyBrick.GraveQuest;

public partial class GameManager : Component
{
	public static GameManager Instance { get; private set; }
	public int Score;

	protected override void OnAwake()
	{
		Instance = this;
	}

	protected override void OnStart()
	{
		_ = SpawnInitialSkeletons();
	}

	protected override void OnUpdate()
	{
		if ( Scene.NavMesh.IsGenerating || TimeUntilNextSkeletonSpawn > 0.0f )
			return;
		SpawnSkeleton();
		TimeUntilNextSkeletonSpawn = 2.0f;
	}
}
