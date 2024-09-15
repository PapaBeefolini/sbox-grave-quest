using Sandbox;
using Sandbox.Navigation;
using Sandbox.Physics;
using System.Numerics;

namespace MightyBrick.GraveQuest;


public sealed class Skeleton : Minifig
{
	public ModelPhysics ModelPhysics { get; private set; }
	public NavMeshAgent Agent { get; private set; }

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
	[Property]
	public Hat[] Hats { get; private set; }

	private TimeSince timeSinceLastMove = 0.0f;


	protected override void OnAwake()
	{
		base.OnAwake();
		ModelPhysics = Components.Get<ModelPhysics>( true );
		Agent = Components.Get<NavMeshAgent>( true );
	}


	protected override void OnStart()
	{
		if ( ModelPhysics.IsValid )
			ModelPhysics.PhysicsGroup.Mass = 150.0f;

		// 50/50 chance at wearing a hat
		if ( Game.Random.Int( 1 ) == 0 )
			WearRandomHat();

		//BreakApart();
	}


	protected override void OnUpdate()
	{
		if ( !Agent.IsValid() )
			return;

		Agent.MoveTo( Vehicle.Local?.Transform.Position ?? Vector3.One );

		if ( timeSinceLastMove > 0.5f )
		{

			timeSinceLastMove = 0.0f;
		}
	}


	public void WearRandomHat()
	{
		if ( Hats.Length <= 0 )
			return;
		Hat chosenHat = Hats[Game.Random.Int( Hats.Length - 1 )];
		HatRenderer.Model = chosenHat.Model;
		HatRenderer.Tint = chosenHat.GetColor();
		HatRenderer.Enabled = true;
	}


	public void BreakApart()
	{
		foreach ( PhysicsJoint joint in ModelPhysics.PhysicsGroup.Joints )
		{
			joint.OnBreak += () => Log.Info( "Fart" );
			joint.Remove();
		}	
	}
}
