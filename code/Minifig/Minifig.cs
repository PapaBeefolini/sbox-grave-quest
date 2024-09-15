using Sandbox;

namespace MightyBrick.GraveQuest;

public class Minifig : Component
{
	[RequireComponent]
	public SkinnedModelRenderer Renderer { get; private set; }
	public SkinnedModelRenderer HatRenderer { get; private set; }
	public ModelCollider HatCollider { get; private set; }
	public Rigidbody HatRigidbody { get; private set; }

	protected override void OnAwake()
	{
		HatRenderer = Components.GetInChildren<SkinnedModelRenderer>( true );
		HatCollider = Components.GetInChildren<ModelCollider>( true );
		HatRigidbody = Components.GetInChildren<Rigidbody>( true );
	}
}
