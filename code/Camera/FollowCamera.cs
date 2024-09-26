namespace MightyBrick.GraveQuest;

public sealed class FollowCamera : Component
{
	public static FollowCamera Instance { get; private set; }

	[RequireComponent]
	public CameraComponent Camera { get; private set; }
	[Property]
	public float UpOffset { get; set; } = 50.0f;
	[Property]
	public float MinPitch { get; set; } = -80.0f;
	[Property]
	public float MaxPitch { get; set; } = 80.0f;
	[Property]
	public float ZoomSpeed { get; set; } = 50.0f;
	[Property]
	public float MaxZoom { get; set; } = 600.0f;
	[Property]
	public float MinZoom { get; set; } = 300.0f;
	[Property]
	[Category( "Auto Focus Settings" )]
	public float AutoFocusSpeed { get; set; } = 3.0f;
	[Property]
	[Category( "Auto Focus Settings" )]
	public float AutoFocusTime { get; set; } = 1.0f;
	[Property]
	[Category( "Auto Focus Settings" )]
	public Angles AutoFocusRotationOffset { get; set; } = new Angles( 20.0f, 0.0f, 0.0f );
	[Property]
	[Category( "Auto Focus Settings" )]
	public float AutoFocusVelocityThreshold { get; set; } = 300.0f;
	public Vehicle Target { get; set; }
	public Angles EyeAngles { get; set; }

	private float cameraZoom = 0.0f;
	private float cameraDistance = 0.0f;
	private bool autoFocusing = false;
	private TimeUntil timeUntilAutoFocus = 0.0f;
	private bool isDrivingBackwards = false;

	protected override void OnAwake()
	{
		Instance = this;
	}

	protected override void OnStart()
	{
		cameraZoom = (MinZoom + MaxZoom) / 2.0f;
		cameraDistance = cameraZoom;
		EyeAngles = AutoFocusRotationOffset;
	}

	protected override void OnUpdate()
	{
		if ( !Target.IsValid() )
			return;

		Zoom();
		Look();
		AutoFocus();

		shakeOffset = shakeOffset.LerpTo( Vector3.Zero, 12.0f * Time.Delta );

		//if ( Input.Pressed( "Brake" ) )
		//	Shake( 32 );
	}

	private void Zoom()
	{
		float zoomSpeed = ZoomSpeed;
		if ( Input.UsingController )
			zoomSpeed /= 8.0f;
		cameraZoom -= Input.Down("ZoomIn" ) ? zoomSpeed : 0.0f;
		cameraZoom += Input.Down( "ZoomOut" ) ? zoomSpeed : 0.0f;
		cameraZoom = cameraZoom.Clamp( MinZoom, MaxZoom );
	}

	private void Look()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, MinPitch, MaxPitch ) );
		Transform.Rotation = EyeAngles.ToRotation();

		Vector3 startPosition = Target.Transform.Position + Vector3.Up * UpOffset;
		Vector3 targetPosition = startPosition + cameraZoom * Transform.Rotation.Backward;
		SceneTraceResult trace = Scene.Trace.Ray( startPosition, targetPosition )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "Player", "Car", "Pizza", "Enemy", "Misc", "World-Border" )
			.Radius( 8.0f )
			.Run();

		cameraDistance = cameraDistance.LerpTo( trace.Distance, 4.0f * Time.Delta );
		if ( cameraDistance > trace.Distance && trace.Hit )
			cameraDistance = trace.Distance;

		Transform.LocalPosition = startPosition + Transform.Rotation.Backward * cameraDistance + shakeOffset;
		Transform.Rotation *= new Angles(-8, 0, 0);
	}

	private void AutoFocus()
	{
		if ( Input.AnalogLook.AsVector3().Length > 1.0f )
		{
			autoFocusing = false;
			timeUntilAutoFocus = AutoFocusTime;
		}
		else if ( timeUntilAutoFocus && Target.LocalVelocity.Length > AutoFocusVelocityThreshold )
		{
			autoFocusing = true;
		}

		if ( Target.LocalVelocity.x < -AutoFocusVelocityThreshold )
			isDrivingBackwards = true;
		else if ( Target.LocalVelocity.x > AutoFocusVelocityThreshold )
			isDrivingBackwards = false;

		if ( autoFocusing )
		{
			Angles angles = Target.Transform.Rotation.Angles().WithPitch( 0 ).WithRoll( 0 );
			angles.yaw = isDrivingBackwards ? angles.yaw + 180.0f : angles.yaw;
			EyeAngles = EyeAngles.LerpTo( angles + AutoFocusRotationOffset, AutoFocusSpeed * Time.Delta );
		}
	}
	Vector3 shakeOffset;
	public void Shake(float amount)
	{
		// Define the intensity of the shake
		float shakeIntensity = amount;

		// Create random shake values for X, Y, and Z axis
		float shakeX = Game.Random.Float( -1.0f, 1.0f ) * shakeIntensity;
		float shakeY = Game.Random.Float( -1.0f, 1.0f ) * shakeIntensity;
		float shakeZ = Game.Random.Float( -1.0f, 1.0f ) * shakeIntensity;

		// Apply the shake offset to the camera's local position
		shakeOffset = new Vector3( shakeX, shakeY, shakeZ );
		//Transform.LocalPosition += shakeOffset;

		// Optionally, you could also apply a small rotation shake if desired
		float shakePitch = Game.Random.Float( -1.0f, 1.0f ) * shakeIntensity;
		float shakeYaw = Game.Random.Float( -1.0f, 1.0f ) * shakeIntensity;
		float shakeRoll = Game.Random.Float( -1.0f, 1.0f ) * shakeIntensity;

		// Apply the rotation shake to the camera's rotation
		//Angles shakeRotation = new Angles( shakePitch, shakeYaw, shakeRoll );
		//Transform.Rotation *= shakeRotation.ToRotation();
	}
}
