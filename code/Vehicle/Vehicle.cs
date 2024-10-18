namespace MightyBrick.GraveQuest;

public partial class Vehicle : Component, Component.ITriggerListener, Component.ICollisionListener
{
	public static Vehicle Local { get; private set; }

	[RequireComponent]
	public Rigidbody Rigidbody { get; private set; }
	public Wheel[] Wheels { get; private set; }
	public Papa Papa { get; private set; }

	public bool Grounded { get; private set; } = false;
	public Vector3 LocalVelocity => Rigidbody.IsValid() ? Rigidbody.Velocity * WorldRotation.Inverse : Vector3.Zero;

	[Property]
	public float MaxSpeed { get; set; } = 1100.0f;
	[Property]
	public float Power { get; set; } = 1400000.0f;
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
	[Property]
	public SoundEvent CrashSound { get; private set; }

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
		if ( !Rigidbody.IsValid() )
			return;

		UpdateInputs();
		UpdateGrounded();
		KeepUpright();

		Papa?.SetInputs( InputForward, InputRight );
	}

	public void OnTriggerEnter( Collider collider )
	{
		Vector3 direction = Vector3.Direction( collider.WorldPosition, WorldPosition );
		Vector3 force = direction * LocalVelocity.Length * 4.0f + Vector3.Up * 2000.0f;

		if ( collider.Components.TryGet<Skeleton>( out Skeleton skeleton ) && LocalVelocity.Length > 100.0f )
		{
			skeleton.Kill( force, LocalVelocity.Length );
			DoCrashEffects();
		}
	}

	public void OnCollisionStart( Collision collision )
	{
		float dot = Vector3.Dot( WorldRotation.Backward, collision.Contact.Normal );
		if ( collision.Contact.Speed.Length >= 700.0f && collision.Other.GameObject.Tags.Has( "wall" ) && dot > 0.7f )
		{
			DoCrashEffects( true );
		}
	}

	private void UpdateInputs()
	{
		float forward = Input.Down( "Throttle" ) ? 1.0f : 0.0f;
		if ( Input.Down( "Brake" ) )
			forward = -1.0f;
		float steer = Input.Down( "Left" ) ? 1.0f : 0.0f;
		if ( Input.Down( "Right" ) )
			steer = -1.0f;
		if ( Input.UsingController )
		{
			forward = Input.GetAnalog( InputAnalog.RightTrigger ) + -Input.GetAnalog( InputAnalog.LeftTrigger );
			steer = -Input.GetAnalog( InputAnalog.LeftStickX );
		}

		InputForward = InputForward.LerpTo( forward, PowerLerpSpeed * Time.Delta );
		InputRight = InputRight.LerpTo( steer, SteerLerpSpeed * Time.Delta );

		if ( !GameManager.Instance.IsGameRunning )
		{
			InputForward = 0.0f;
			InputRight = 0.0f;
			return;
		}

		if ( Input.Pressed( "ThrowPizza" ) )
			ThrowPizza();
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
		if ( !Rigidbody.IsValid() )
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

	public void ShakeCamera( float duration, float intensity )
	{
		FollowCamera.Instance?.AddShake( new ScreenShake.Random( duration, intensity ) );
		Input.TriggerHaptics( HapticEffect.Rumble );
	}

	private void DoCrashEffects(bool animate = false)
	{
		if ( animate )
			Papa?.Crash();
		float normalizedVelocity = MathX.Clamp( LocalVelocity.Length / MaxSpeed, 0.0f, 1.0f );
		float shakeIntensity = MathX.Lerp( 0.25f, 4.0f, normalizedVelocity );
		ShakeCamera( 0.5f, shakeIntensity );
		Sound.Play( CrashSound, WorldPosition );
	}
}
