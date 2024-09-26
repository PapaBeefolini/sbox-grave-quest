namespace MightyBrick.GraveQuest;

public class Minifig : Component
{
	[RequireComponent]
	public SkinnedModelRenderer Renderer { get; private set; }
	public SkinnedModelRenderer HatRenderer { get; private set; }

	private TimeUntil scaleTime = 0.0f;

	protected override void OnAwake()
	{
		HatRenderer = Components.GetInChildren<SkinnedModelRenderer>( true );
	}

	protected override void OnUpdate()
	{
		Transform.Scale = Transform.Scale.LerpTo( Vector3.One, Easing.EaseOut( scaleTime.Fraction ) );
	}

	public void ScaleZ( float value )
	{
		Transform.Scale = Transform.Scale.WithZ( value );
		scaleTime = 1.0f;
	}
}
