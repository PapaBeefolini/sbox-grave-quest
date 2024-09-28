namespace MightyBrick.GraveQuest;

public sealed class Papa : Minifig
{
	public static Papa Instance { get; private set; }
	public static int HatIndex { get; set; } = 0;

	[Property]
	public Model[] Hats { get; set; }

	private TimeUntil scaleTime = 0.0f;

	protected override void OnAwake()
	{
		base.OnAwake();
		Instance = this;
	}

	protected override void OnStart()
	{
		SetHatModel( HatIndex );
	}

	protected override void OnUpdate()
	{
		Transform.Scale = Transform.Scale.LerpTo( Vector3.One, Easing.EaseOut( scaleTime.Fraction ) );
	}

	public void SetInputs(float vertical, float horizontal)
	{
		Renderer?.Set( "Vertical", vertical );
		Renderer?.Set( "Horizontal", -horizontal );
	}

	public void SetIsDriving( bool value )
	{
		Renderer?.Set( "Sitting", value );
	}

	public void Throw()
	{
		Renderer?.Set( "Throw", true );
	}

	public void Crash()
	{
		Renderer?.Set( "Crash", true );
	}

	public void SetHatIndex( int index )
	{
		SetHatModel( index );
		ScaleZ( 0.9f );
	}

	public void OffsetHatIndex( int offset )
	{
		HatIndex = (HatIndex + offset) % Hats.Length;
		if ( HatIndex < 0 )
			HatIndex += Hats.Length;
		SetHatIndex( HatIndex );
	}

	private void SetHatModel( int index )
	{
		if ( Hats.Length <= 0 )
			return;
		if ( HatRenderer.IsValid() )
			HatRenderer.Model = Hats[index];
	}

	public void ScaleZ( float value )
	{
		Transform.Scale = Transform.Scale.WithZ( value );
		scaleTime = 1.0f;
	}
}
