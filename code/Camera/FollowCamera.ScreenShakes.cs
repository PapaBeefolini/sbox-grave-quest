using System.Collections.Generic;

namespace MightyBrick.GraveQuest;

public partial class FollowCamera
{
	private readonly List<ScreenShake> Shakes = new();

	protected void ApplyScreenShakes()
	{
		for ( var i = Shakes.Count; i > 0; i-- )
		{
			ScreenShake entry = Shakes[i - 1];
			bool keep = entry.Update( Camera );
			if ( keep )
				continue;
			Shakes.RemoveAt( i - 1 );
		}
	}

	public void AddShake( ScreenShake shake )
	{
		Shakes.Add( shake );
	}
}
