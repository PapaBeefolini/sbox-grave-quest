namespace MightyBrick.GraveQuest;

public class Minifig : Component
{
	[RequireComponent] public SkinnedModelRenderer Renderer { get; protected set; }
	public SkinnedModelRenderer HatRenderer { get; protected set; }

	protected override void OnAwake()
	{
		HatRenderer = Components.GetInChildren<SkinnedModelRenderer>( true );
	}
}
