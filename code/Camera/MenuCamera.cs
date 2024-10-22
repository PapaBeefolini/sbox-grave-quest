namespace MightyBrick.GraveQuest;

public sealed class MenuCamera : Component
{
	[Property]
	public float SwayAmount { get; set; } = 1.0f;
	[Property]
	public float SwaySpeed { get; set; } = 0.5f;
	public Transform InitialTransform { get; set; }
	[Property]
	public GameObject CustomizationTransform { get; set; }
	public Transform CurrentTransform { get; set; }

	private float swayTime = 0.0f;
	private TimeSince timeSinceRandomSway = 0.0f;
	private float currentSwayAmount;
	private float targetSwayAmount;
	private float currentSwaySpeed;
	private float targetSwaySpeed;

	protected override void OnStart()
	{
		InitialTransform = Transform.World;
		SetRandomSway();
	}

	protected override void OnUpdate()
	{
		CurrentTransform = GameManager.Instance.IsCustomizing ? CustomizationTransform.Transform.World : InitialTransform;

		swayTime += Time.Delta * SwaySpeed;
		float swayPitch = float.Sin( swayTime ) * SwayAmount * currentSwayAmount;
		float swayYaw = float.Cos( swayTime ) * SwayAmount * currentSwayAmount * 0.5f;

		currentSwayAmount = currentSwayAmount.LerpTo( targetSwayAmount, Time.Delta );
		currentSwaySpeed = currentSwaySpeed.LerpTo( targetSwaySpeed, Time.Delta );

		Rotation rotation = CurrentTransform.Rotation * Rotation.From( swayPitch, swayYaw, 0 );
		Vector3 position = CurrentTransform.Position;
		CurrentTransform = new Transform( position, rotation );

		Transform.LerpTo( CurrentTransform, 4.0f * Time.Delta );

		Sway();
	}

	private void Sway()
	{
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
