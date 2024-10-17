namespace MightyBrick.GraveQuest;

public partial class FollowCamera : Component
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

	private float zoom = 0.0f;
	private float distance = 0.0f;
	private Vector3 shakeOffset = Vector3.Zero;
	private bool autoFocusing = false;
	private TimeUntil timeUntilAutoFocus = 0.0f;
	private bool isDrivingBackwards = false;

	protected override void OnAwake()
	{
		Instance = this;
	}

	protected override void OnStart()
	{
		zoom = (MinZoom + MaxZoom) / 2.0f;
		distance = zoom;
		EyeAngles = AutoFocusRotationOffset;
	}

	protected override void OnUpdate()
	{
		if ( !Target.IsValid() )
			return;

		Zoom();
		Look();
		AutoFocus();
		ApplyScreenShakes();
	}

	private void Zoom()
	{
		float zoomSpeed = ZoomSpeed;
		if ( Input.UsingController )
			zoomSpeed /= 8.0f;
		zoom -= Input.Down("ZoomIn" ) ? zoomSpeed : 0.0f;
		zoom += Input.Down( "ZoomOut" ) ? zoomSpeed : 0.0f;
		zoom = zoom.Clamp( MinZoom, MaxZoom );
	}

	private void Look()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( MathX.Clamp( EyeAngles.pitch, MinPitch, MaxPitch ) );
		WorldRotation = EyeAngles.ToRotation();

		Vector3 startPosition = Target.WorldPosition + Vector3.Up * UpOffset;
		Vector3 targetPosition = startPosition + zoom * WorldRotation.Backward;
		SceneTraceResult trace = Scene.Trace.Ray( startPosition, targetPosition )
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( "Player", "Car", "Pizza", "Enemy", "Misc", "World-Border" )
			.Radius( 8.0f )
			.Run();

		distance = distance.LerpTo( trace.Distance, 4.0f * Time.Delta );
		if ( distance > trace.Distance && trace.Hit )
			distance = trace.Distance;

		LocalPosition = startPosition + WorldRotation.Backward * distance + shakeOffset;
		WorldRotation *= new Angles(-8, 0, 0);
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
			Angles angles = Target.WorldRotation.Angles().WithPitch( 0 ).WithRoll( 0 );
			angles.yaw = isDrivingBackwards ? angles.yaw + 180.0f : angles.yaw;
			EyeAngles = EyeAngles.LerpTo( angles + AutoFocusRotationOffset, AutoFocusSpeed * Time.Delta );
		}
	}
}
