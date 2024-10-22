namespace MightyBrick.GraveQuest;

public partial class Skeleton
{
	[Property, Category( "Hats" )]
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
	}

	private void DropHat()
	{
		ModelCollider collider = HatRenderer.Components.Create<ModelCollider>();
		collider.Model = HatRenderer.Model;
		collider.Surface = Surface.FindByName( "plastic" );

		HatRenderer.GameObject.SetParent( null );
		HatRenderer.BoneMergeTarget = null;

		Rigidbody rigidbody = HatRenderer.Components.Create<Rigidbody>();
		rigidbody.MassOverride = 5.0f;
	}
}
