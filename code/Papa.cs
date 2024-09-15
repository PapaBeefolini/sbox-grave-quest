using Sandbox;

namespace MightyBrick.GraveQuest;

public sealed class Papa : Minifig
{
	protected override void OnUpdate()
	{
		Renderer?.Set( "Horizontal", -Input.AnalogMove.y );
		Renderer?.Set( "Vertical", Input.AnalogMove.x );
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
}
