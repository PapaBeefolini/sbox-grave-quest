namespace MightyBrick.GraveQuest;

public sealed class Wheel : Component
{
	public Vehicle Vehicle { get; set; }
	public ModelRenderer WheelModel { get; set; }

	public bool Grounded { get; private set; }
	public Vector3 Velocity => Vehicle.IsValid() ? Vehicle.GetVelocityAtPoint( WorldPosition ) : Vector3.Zero;
	public Vector3 LocalVelocity => Velocity * WorldRotation.Inverse;

	[Property]
	public float SuspensionRestLength { get; set; } = 20.0f;
	[Property]
	public float SpringStrength { get; set; } = 100000.0f;
	[Property]
	public float SpringDamper { get; set; } = 2500.0f;
	[Property]
	public float WheelRadius { get; set; } = 12.0f;
	[Property]
	public float WheelGrip { get; set; } = 16.0f;
	[Property]
	public bool Inverted { get; set; } = false;
	[Property, ToggleGroup( "Steering", Label = "Enable Steering")]
	public bool Steering { get; set; } = false;
	[Property, ToggleGroup( "Steering" )]
	public float SteerAngle { get; set; } = 35.0f;
	[Property]
	public GameObject DirtParticleEffectPrefab { get; set; }

	public Vector3 SurfaceImpactPoint { get; private set; }
	public Vector3 SurfaceImpactNormal { get; private set; }
	public float SurfaceImpactDistance { get; private set; }
	public ParticleEffect DirtParticleEffect { get; private set; }

	private Vector3 liftForce = Vector3.Zero;
	private Vector3 tractionForce = Vector3.Zero;
	private Vector3 driveForce = Vector3.Zero;
	private Vector3 brakeForce = Vector3.Zero;
	private float rotationSpeed = 0.0f;

	protected override void OnAwake()
	{
		Vehicle = GameObject.Components.GetInParent<Vehicle>();
		WheelModel = GameObject.Components.GetInChildren<ModelRenderer>();

		GameObject particles = DirtParticleEffectPrefab.Clone( WorldPosition );
		particles.SetParent( GameObject );
		DirtParticleEffect = particles.Components.Get<ParticleEffect>();
	}

	protected override void OnFixedUpdate()
	{
		if ( !Vehicle.IsValid() )
			return;

		Raycast();
		Lift();
		Traction();
		Drive();
		Steer();
		AutoBraking();
		UpdateModel();
		UpdateParticles();
		ApplyForce();
	}

	protected override void OnUpdate()
	{
		//Gizmo.Draw.LineCircle( WheelModel.Transform.Position, WheelModel.Transform.Rotation.Right, WheelRadius );
	}

	private void Raycast()
	{
		Vector3 tracePosition = WorldPosition + WorldRotation.Up * 1.5f;
		SceneTraceResult trace = Scene.Trace.Ray( tracePosition, tracePosition + Vector3.Down * SuspensionRestLength )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "Player", "Car", "Pizza" )
			.Run();

		if ( trace.Hit == false )
		{
			Grounded = false;
			return;
		}

		Grounded = true;
		SurfaceImpactPoint = trace.EndPosition;
		SurfaceImpactNormal = trace.Normal;
		SurfaceImpactDistance = trace.Distance;
	}

	private void Lift()
	{
		if ( !Grounded )
		{
			liftForce = Vector3.Zero;
			return;
		}

		Vector3 projectedDirection = WorldRotation.Up.ProjectOnNormal( SurfaceImpactNormal );
		float offset = SuspensionRestLength - SurfaceImpactDistance;
		float velocity = Vector3.Dot( projectedDirection, Velocity );
		float force = (offset * SpringStrength) - (velocity * SpringDamper);
		liftForce = Vector3.Up * force;
	}

	private void Traction()
	{
		if ( !Grounded )
		{ 
			tractionForce = Vector3.Zero;
			return;
		}

		float sidewaysVelocity = Vector3.Dot( WorldRotation.Right, Velocity );
		float desiredSidewaysVelocity = -sidewaysVelocity * WheelGrip;
		tractionForce = WorldRotation.Right * desiredSidewaysVelocity * 100.0f;
	}

	private void Drive()
	{
		if ( !Grounded || Vehicle.LocalVelocity.Length > Vehicle.MaxSpeed )
		{
			driveForce = Vector3.Zero;
			return;
		}

		float power = Vehicle.Power / Vehicle.Wheels.Count();
		if ( Velocity.Length < 25 )
			power *= 0.5f;
		Vector3 desiredPower = Inverted ? -power : power;
		driveForce = Vehicle.InputForward * WorldRotation.Forward * desiredPower;
	}

	private void Steer()
	{
		if ( !Steering )
			return;

		float angle = Steering ? (Vehicle.InputRight * SteerAngle) : 0;
		if ( Inverted )
			angle += 180.0f;
		LocalRotation = Rotation.FromAxis( Vector3.Up, angle );
	}

	private void AutoBraking()
	{
		brakeForce = 0.0f;

		if ( Vehicle.LocalVelocity.Length <= 100.0f )
		{
			brakeForce = WorldRotation.Backward * LocalVelocity.x * 700;
		}
	}

	private void UpdateModel()
	{
		if ( !WheelModel.IsValid() )
			return;

		float zOffset = Grounded ? (-1 * SurfaceImpactDistance + WheelRadius) : (-1 * SuspensionRestLength + WheelRadius);
		WheelModel.WorldPosition = WorldPosition + WorldRotation.Up * zOffset;

		float circumference = 2 * MathF.PI * WheelRadius;
		rotationSpeed = (Vehicle.LocalVelocity.x + Vehicle.InputForward * 200) / circumference * (Inverted ? -360 : 360);
		WheelModel.WorldRotation *= Rotation.FromAxis( Vector3.Left, rotationSpeed * Time.Delta );
	}

	private void UpdateParticles()
	{
		if ( !Grounded || float.Abs( rotationSpeed ) < 600.0f || Vehicle.LocalVelocity.Length > 800.0f )
			return;

		DirtParticleEffect.WorldPosition = SurfaceImpactPoint + SurfaceImpactNormal * 4.0f;

		for ( int i = 0; i < 3; i++ )
		{
			Particle particle = DirtParticleEffect.Emit( DirtParticleEffect.WorldPosition, Time.Delta );
			particle.Velocity += Vehicle.WorldRotation.Backward * Vehicle.LocalVelocity.x;
		}
	}

	private void ApplyForce()
	{
		Vehicle.Rigidbody.ApplyForceAt( WorldPosition, liftForce + tractionForce + driveForce + brakeForce );
	}
}
