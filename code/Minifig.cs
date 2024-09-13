using Sandbox;
using System;

namespace MightyBrick.GraveQuest;


public sealed class Minifig : Component
{
	public SkinnedModelRenderer SkinnedModelRenderer { get; set; }


	protected override void OnAwake()
	{
		SkinnedModelRenderer = Components.Get<SkinnedModelRenderer>();
	}


	protected override void OnUpdate()
	{
		SkinnedModelRenderer.Set( "Horizontal", -Input.AnalogMove.y );
		SkinnedModelRenderer.Set( "Vertical", Input.AnalogMove.x );
	}
}
