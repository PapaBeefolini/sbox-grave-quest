using Sandbox;
using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace MightyBrick.GraveQuest;


public sealed class Wheel : Component
{
	public Vehicle Vehicle { get; set; }
	public ModelRenderer WheelModel { get; set; }

	public bool Grounded { get; private set; }
	public Vector3 LocalVelocity => Vehicle.IsValid ? Vehicle.GetVelocityAtPoint( Transform.Position ) : Vector3.Zero;

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

	public Vector3 SurfaceImpactPoint { get; private set; }
	public Vector3 SurfaceImpactNormal { get; private set; }
	public float SurfaceImpactDistance { get; private set; }

	private Vector3 liftForce = Vector3.Zero;
	private Vector3 tractionForce = Vector3.Zero;
	private Vector3 driveForce = Vector3.Zero;


	protected override void OnAwake()
	{
		Vehicle = GameObject.Components.GetInParent<Vehicle>();
		WheelModel = GameObject.Components.GetInChildren<ModelRenderer>();
	}


	protected override void OnFixedUpdate()
	{
		if ( !Vehicle.IsValid )
			return;

		Raycast();
		Lift();
		Traction();
		Drive();
		Steer();
		UpdateModel();
		ApplyForce();
	}


	protected override void OnUpdate()
	{
		//Gizmo.Draw.LineCircle( WheelModel.Transform.Position, WheelModel.Transform.Rotation.Right, WheelRadius );
	}


	private void Raycast()
	{
		Vector3 tracePosition = Transform.Position + Transform.Rotation.Up * 1.5f;
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

		Vector3 projectedDirection = Transform.Rotation.Up.ProjectOnNormal( SurfaceImpactNormal );
		float offset = SuspensionRestLength - SurfaceImpactDistance;
		float velocity = Vector3.Dot( projectedDirection, LocalVelocity );
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

		float sidewaysVelocity = Vector3.Dot( Transform.Rotation.Right, LocalVelocity );
		float desiredSidewaysVelocity = -sidewaysVelocity * WheelGrip;
		tractionForce = Transform.Rotation.Right * desiredSidewaysVelocity * 100.0f;
	}


	private void Drive()
	{
		if ( !Grounded || Vehicle.LocalVelocity.Length > Vehicle.MaxSpeed )
		{
			driveForce = Vector3.Zero;
			return;
		}

		float power = Vehicle.Power / Vehicle.Wheels.Count();
		if ( LocalVelocity.Length < 25 )
			power *= 0.5f;
		Vector3 desiredPower = Inverted ? -power : power;
		driveForce = Vehicle.InputForward * Transform.Rotation.Forward * desiredPower;
	}


	private void Steer()
	{
		if ( !Steering )
			return;

		float angle = Steering ? (Vehicle.InputRight * SteerAngle) : 0;
		if ( Inverted )
			angle += 180.0f;
		Transform.LocalRotation = Rotation.FromAxis( Vector3.Up, angle );
	}


	private void UpdateModel()
	{
		if ( !WheelModel.IsValid )
			return;

		float zOffset = Grounded ? (-1 * SurfaceImpactDistance + WheelRadius) : (-1 * SuspensionRestLength + WheelRadius);
		WheelModel.Transform.Position = Transform.Position + Transform.Rotation.Up * zOffset;

		float circumference = 2 * MathF.PI * WheelRadius;
		float rotationSpeed = (Vehicle.LocalVelocity.x + Vehicle.InputForward * 200) / circumference * (Inverted ? -360 : 360);
		WheelModel.Transform.Rotation *= Rotation.FromAxis( Vector3.Left, rotationSpeed * Time.Delta );
	}


	private void ApplyForce()
	{
		Vehicle.Rigidbody.ApplyForceAt( Transform.Position, liftForce + tractionForce + driveForce );
	}
}
