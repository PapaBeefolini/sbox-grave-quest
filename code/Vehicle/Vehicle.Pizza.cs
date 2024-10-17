namespace MightyBrick.GraveQuest;

public record PizzaThrownEvent() : IGameEvent;

public partial class Vehicle
{
	[Property, Category( "Pizza Throwing" )]
	public GameObject PizzaPrefab { get; private set; }
	[Property, Category( "Pizza Throwing" )]
	public Vector3 ThrowForce { get; private set; } = new Vector3( 600.0f, 0.0f, 300.0f );
	[Property, Category( "Pizza Throwing" )]
	public Vector3 ThrowOffset { get; private set; } = new Vector3( 50.0f, 0.0f, 50.0f );
	[Property, Category( "Pizza Throwing" )]
	public float ThrowCooldown { get; private set; } = 1.5f;
	[Property, Category( "Pizza Throwing" )]
	public SoundEvent ThrowSound { get; private set; }

	public TimeUntil timeUntilCanThrow = 0.0f;

	private void ThrowPizza()
	{
		if ( !timeUntilCanThrow )
			return;
		timeUntilCanThrow = ThrowCooldown;
		Scene.Dispatch( new PizzaThrownEvent() );

		Papa?.Throw();
		GameObject pizza = PizzaPrefab.Clone( Papa.WorldPosition + WorldRotation * ThrowOffset, WorldRotation );
		Rigidbody pizzaRigidbody = pizza.Components.Get<Rigidbody>();
		if ( !pizzaRigidbody.IsValid() )
			return;
		pizzaRigidbody.Velocity = Rigidbody.Velocity + WorldRotation * ThrowForce;

		Sound.Play( ThrowSound, WorldPosition );
	}

	public float GetThrowCooldown()
	{
		return timeUntilCanThrow.Fraction;
	}
}
