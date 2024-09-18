namespace MightyBrick.GraveQuest;

public partial class Skeleton
{
	[Property]
	public Hat[] Hats { get; private set; }

	public struct Hat
	{
		public Model Model { get; set; }
		public Color[] Colors { get; set; }

		public Color GetColor()
		{
			return Colors.Length > 0 ? Colors[Game.Random.Int( Colors.Length - 1 )] : Color.White;
		}

		public override string ToString()
		{
			return Model?.Name;
		}
	}

	private void WearRandomHat()
	{
		if ( Hats.Length <= 0 )
			return;

		Hat chosenHat = Hats[Game.Random.Int( Hats.Length - 1 )];
		HatRenderer.Model = chosenHat.Model;
		HatRenderer.Tint = chosenHat.GetColor();
		HatRenderer.Enabled = true;
		HatCollider.Model = chosenHat.Model;
	}
}
