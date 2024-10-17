namespace MightyBrick.GraveQuest;

public abstract class ScreenShake
{
	public abstract bool Update( CameraComponent camera );

	public class Random : ScreenShake
	{
		public float Progress => Easing.EaseOut( ((float)LifeTime).LerpInverse( 0, Length ) );

		private float Length { get; set; }
		private float Size { get; set; }
		private TimeSince LifeTime { get; set; } = 0.0f;

		public Random( float length = 2.0f, float size = 4.0f )
		{
			Length = length;
			Size = size;
		}

		public override bool Update( CameraComponent camera )
		{
			Vector3 random = Vector3.Random.WithZ( 0 ).Normal;
			camera.LocalPosition += (camera.LocalRotation.Right * random.x + camera.LocalRotation.Up * random.y) * (1.0f - Progress) * Size;
			return LifeTime < Length;
		}
	}
}
