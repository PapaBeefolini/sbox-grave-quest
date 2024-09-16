using Sandbox;
using System;

namespace MightyBrick.GraveQuest;

public partial class Vehicle
{
	[Property, Category( "Pizza Throwing" )]
	public GameObject PizzaPrefab { get; private set; }
	[Property, Category( "Pizza Throwing" )]
	public Vector3 ThrowForce { get; private set; } = new Vector3( 600.0f, 0.0f, 300.0f );
	[Property, Category( "Pizza Throwing" )]
	public Vector3 ThrowOffset { get; private set; } = new Vector3( 50.0f, 0.0f, 50.0f );
	[Property, Category( "Pizza Throwing" )]
	public float ThrowCooldown { get; private set; } = 1.0f;

	private TimeUntil timeUntilCanThrow = 0.0f;

	private void ThrowPizza()
	{
		if ( !timeUntilCanThrow )
			return;
		timeUntilCanThrow = ThrowCooldown;

		Papa?.Throw();
		GameObject pizza = PizzaPrefab.Clone( Papa.Transform.Position + Transform.Rotation * ThrowOffset, Transform.Rotation );
		Rigidbody pizzaRigidbody = pizza.Components.Get<Rigidbody>();
		if ( !pizzaRigidbody.IsValid )
			return;
		pizzaRigidbody.Velocity = Rigidbody.Velocity + Transform.Rotation * ThrowForce;
	}
}
