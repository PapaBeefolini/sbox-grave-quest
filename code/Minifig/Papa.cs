namespace MightyBrick.GraveQuest;

public sealed class Papa : Minifig
{
	public static Papa Instance { get; private set; }

	[Property] public Model[] Hats { get; set; }
	[Property] public Color[] HatColors { get; set; }

	private int HatIndex { get; set; } = 0;
	private int HatColorIndex { get; set; } = 0;
	private TimeUntil scaleTime = 0.0f;
	private Vector3 scale;

	protected override void OnAwake()
	{
		base.OnAwake();
		Instance = this;
	}

	protected override void OnStart()
	{
		HatIndex = GameManager.GameSettings.HatIndex;
		HatColorIndex = GameManager.GameSettings.HatColor;
		SetHatModel( HatIndex );
		SetHatColor( HatColorIndex );
	}

	protected override void OnUpdate()
	{
		WorldScale = scale.LerpTo( Vector3.One, Easing.EaseOut( scaleTime.Fraction ) );
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

	public void SetHatColorIndex( int index )
	{
		SetHatColor( index );
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
		{
			HatRenderer.Model = Hats[index];
			HatRenderer.Enabled = Hats[index].IsValid();
		}
		GameManager.GameSettings.HatIndex = index;
	}

	private void SetHatColor( int index )
	{
		HatColorIndex = index;
		if ( HatRenderer.IsValid() )
			HatRenderer.Tint = HatColors[HatColorIndex];
		GameManager.GameSettings.HatColor = HatColorIndex;
	}

	private void ScaleZ( float value )
	{
		scale = WorldScale.WithZ( value );
		scaleTime = 0.25f;
	}
}
