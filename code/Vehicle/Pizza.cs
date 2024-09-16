using Sandbox;
using System;

namespace MightyBrick.GraveQuest;

public sealed class Pizza : Component, Component.ITriggerListener
{
	[Property]
	public GameObject ExplosionPrefab { get; set; }
	[RequireComponent]
	public Rigidbody Rigidbody { get; private set; }

	protected override void OnStart()
	{
		Rigidbody.PhysicsBody.SpeculativeContactEnabled = true;
	}

	public void OnTriggerEnter( Collider collider )
	{
		Explode();
	}

	private void Explode()
	{
		GameObject explosion = ExplosionPrefab.Clone( Transform.Position, Transform.Rotation );
		GameObject.Destroy();
	}
}
