namespace MightyBrick.GraveQuest;

public partial class Skeleton
{
	private void Animate()
	{
		Renderer.Set( "Velocity", Agent.Velocity.Length );
		Renderer.Set( "SpeedMultiplier", SpeedMultiplier );
	}
}
