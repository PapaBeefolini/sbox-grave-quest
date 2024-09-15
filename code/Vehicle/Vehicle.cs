using Sandbox;
using System;

namespace MightyBrick.GraveQuest;

public sealed class Vehicle : Component
{
	public static Vehicle Local { get; private set; }

	[RequireComponent]
	public Rigidbody Rigidbody { get; private set; }
	public Wheel[] Wheels { get; private set; }
	public Papa Papa { get; private set; }

	public bool Grounded { get; private set; } = false;
	public Vector3 LocalVelocity => Rigidbody.IsValid ? Rigidbody.Velocity * Transform.Rotation.Inverse : Vector3.Zero;

	[Property]
	public float MaxSpeed { get; set; } = 1200.0f;
	[Property]
	public float Power { get; set; } = 1500000.0f;
	[Property]
	public float PowerLerpSpeed { get; set; } = 14.0f;
	[Property]
	public float SteerLerpSpeed { get; set; } = 12.0f;
	[Property]
	public float AngularDampingGrounded { get; set; } = 4.0f;
	[Property]
	public float AngularDampingAirborne { get; set; } = 0.5f;
	[Property]
	public float KeepUprightAngle { get; set; } = 55.0f;

	[Property, Category( "Pizza Throwing" )]
	public GameObject PizzaPrefab { get; private set; }
	[Property, Category( "Pizza Throwing" )]
	public Vector3 ThrowForce { get; private set; } = new Vector3( 600.0f, 0.0f, 300.0f );
	[Property, Category( "Pizza Throwing" )]
	public Vector3 ThrowOffset { get; private set; } = new Vector3( 50.0f, 0.0f, 50.0f );

	public float InputForward { get; set; } = 0.0f;
	public float InputRight { get; set; } = 0.0f;

	protected override void OnStart()
	{
		if ( IsProxy )
			return;

		Local = this;
		FollowCamera.Instance.Target = this;
		Wheels = GameObject.Components.GetAll<Wheel>( FindMode.EverythingInChildren ).ToArray();
		Papa = GameObject.Components.GetInChildren<Papa>();
		Papa?.SetIsDriving( true );
	}

	protected override void OnFixedUpdate()
	{
		if ( !Rigidbody.IsValid )
			return;

		UpdateInputs();
		UpdateGrounded();
		KeepUpright();

		//if ( Input.Pressed( "Jump" ) )
		//	Rigidbody.PhysicsBody.ApplyImpulse( Vector3.Up * 400000.0f );

		if ( Input.Pressed( "attack1" ) || Input.Pressed( "jump" ) )
			ThrowPizza();
	}

	protected override void OnUpdate()
	{
		//Gizmo.Draw.ScreenText( InputRight.ToString(), Vector2.Zero );
		//Gizmo.Draw.ScreenText( Rigidbody.AngularDamping.ToString(), new Vector2( 0, 20 ) );
		//Gizmo.Draw.ScreenText( Rigidbody.Velocity.Length.ToString(), new Vector2( 0, 40 ) );
	}

	private void UpdateInputs()
	{
		float forward = Input.AnalogMove.x + Input.GetAnalog( InputAnalog.RightTrigger ) - Input.GetAnalog( InputAnalog.LeftTrigger );
		InputForward = InputForward.LerpTo( forward, PowerLerpSpeed * Time.Delta );
		InputRight = InputRight.LerpTo( Input.AnalogMove.y, SteerLerpSpeed * Time.Delta );
	}

	private void UpdateGrounded()
	{
		Grounded = false;

		foreach ( Wheel wheel in Wheels )
		{
			if ( wheel.Grounded )
			{
				Grounded = true;
				break;
			}
		}

		Rigidbody.AngularDamping = Grounded ? AngularDampingGrounded : AngularDampingAirborne;
		Rigidbody.OverrideMassCenter = Grounded;
	}

	public Vector3 GetVelocityAtPoint( Vector3 point )
	{
		if ( !Rigidbody.IsValid )
			return Vector3.Zero;
		return Rigidbody.GetVelocityAtPoint( point );
	}

	private void KeepUpright()
	{
		Angles angles = Rigidbody.PhysicsBody.Rotation.Angles();
		angles.pitch = angles.pitch.Clamp( -KeepUprightAngle, KeepUprightAngle );
		angles.roll = angles.roll.Clamp( -KeepUprightAngle, KeepUprightAngle );
		Rigidbody.PhysicsBody.Rotation = angles.ToRotation();
	}

	private void ThrowPizza()
	{
		Papa?.Throw();
		GameObject pizza = PizzaPrefab.Clone( Papa.Transform.Position + Transform.Rotation * ThrowOffset, Transform.Rotation );
		Rigidbody pizzaRigidbody = pizza.Components.Get<Rigidbody>();
		if ( !pizzaRigidbody.IsValid )
			return;
		pizzaRigidbody.Velocity = Rigidbody.Velocity + Transform.Rotation * ThrowForce;
	}
}
