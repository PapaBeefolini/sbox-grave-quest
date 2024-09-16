using Sandbox;

namespace MightyBrick.GraveQuest;

public sealed class Papa : Minifig
{
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
}
