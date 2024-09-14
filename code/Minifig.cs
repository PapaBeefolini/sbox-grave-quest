using Sandbox;
using System;

namespace MightyBrick.GraveQuest;


public sealed class Minifig : Component
{
	public SkinnedModelRenderer SkinnedModelRenderer { get; private set; }
	public SkinnedModelRenderer Hat { get; private set; }


	protected override void OnAwake()
	{
		SkinnedModelRenderer = Components.Get<SkinnedModelRenderer>();
		Hat = Components.GetInChildren<SkinnedModelRenderer>();
	}


	protected override void OnUpdate()
	{
		SkinnedModelRenderer?.Set( "Horizontal", -Input.AnalogMove.y );
		SkinnedModelRenderer?.Set( "Vertical", Input.AnalogMove.x );
	}


	public void SetIsDriving( bool value )
	{
		SkinnedModelRenderer?.Set( "Sitting", value );
	}


	public void Throw()
	{
		SkinnedModelRenderer?.Set( "Throw", true );
	}


	public void Crash()
	{
		SkinnedModelRenderer?.Set( "Crash", true );
	}
}
