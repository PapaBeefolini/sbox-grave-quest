namespace MightyBrick.GraveQuest;

public sealed class CameraSway : Component
{
	[Property] public float SwayAmount { get; set; } = 1.0f;
	[Property] public float SwaySpeed { get; set; } = 0.5f;

	private Rotation initialRotation;
	private float time = 0.0f;
	private TimeSince timeSinceRandomSway = 0.0f;
	private float currentSwayAmount;
	private float targetSwayAmount;
	private float currentSwaySpeed;
	private float targetSwaySpeed;

	protected override void OnStart()
	{
		initialRotation = Transform.Rotation;
		SetRandomSway();
	}

	protected override void OnUpdate()
	{
		time += Time.Delta * SwaySpeed;

		float swayPitch = float.Sin( time ) * SwayAmount * currentSwayAmount;
		float swayYaw = float.Cos( time ) * SwayAmount * currentSwayAmount * 0.5f;

		Transform.Rotation = initialRotation * Rotation.From( swayPitch, swayYaw, 0 );

		currentSwayAmount = currentSwayAmount.LerpTo( targetSwayAmount, Time.Delta );
		currentSwaySpeed = currentSwaySpeed.LerpTo( targetSwaySpeed, Time.Delta );

		if ( timeSinceRandomSway > 0.5f )
			SetRandomSway();
	}

	private void SetRandomSway()
	{
		timeSinceRandomSway = 0.0f;
		targetSwayAmount = SwayAmount + Game.Random.Float( -0.5f, 0.5f );
		targetSwaySpeed = SwaySpeed + Game.Random.Float( -0.5f, 0.5f );
	}
}
