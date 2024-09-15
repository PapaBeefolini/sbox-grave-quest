using Sandbox;

namespace MightyBrick.GraveQuest;

public class Minifig : Component
{
	public SkinnedModelRenderer Renderer { get; private set; }
	public SkinnedModelRenderer HatRenderer { get; private set; }

	protected override void OnAwake()
	{
		Renderer = Components.Get<SkinnedModelRenderer>( true );
		HatRenderer = Components.GetInChildren<SkinnedModelRenderer>( true );
	}
}
