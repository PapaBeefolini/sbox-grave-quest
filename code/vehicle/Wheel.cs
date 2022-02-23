using Sandbox;
using System;

namespace Papa
{
	public partial class Wheel : Entity
	{
		[Net] public bool Grounded { get; private set; }
		[Net] public Vector3 SurfaceImpactPoint { get; private set; }
		[Net] public Vector3 SurfaceImpactNormal { get; private set; }
		[Net] public float SurfaceImpactDistance { get; private set; }

		//----------//

		private PID PID = new PID( 6.0f, 0.01f, 0.2f );
		private Vehicle vehicle;

		private float height = 32f;
		private float force = 280000;

		private ModelEntity model;
		private float modelOffset = 24.0f;
		private bool usesSteering = true;
		private bool invertSteering = false;
		private float steerAngle = 35.0f;
		private Angles initialRotation;

		//----------//

		public void Setup( Vehicle vehicle, Transform transform, string modelToUse, float height, float offset, bool steering )
		{
			this.vehicle = vehicle;
			SetParent( vehicle );
			Transform = transform;

			model = new ModelEntity( modelToUse, this);
			model.SetParent( this, null, new Transform( Vector3.Zero, Rotation.Identity ) );
			initialRotation = model.LocalPosition.EulerAngles;

			this.height = height;
			modelOffset = offset;
			usesSteering = steering;
		}

		public void Raycast()
		{
			var tr = Trace.Ray( Position, Position + vehicle.Rotation.Down * height )
				.Ignore( vehicle )
				.WithoutTags( "Skeleton" )
				.WithoutTags( "Vehicle" )
				.WithoutTags( "Pizza" )
				.Run();

			if ( tr.Hit )
			{
				Grounded = true;

				SurfaceImpactPoint = tr.EndPosition;
				SurfaceImpactNormal = tr.Normal;
				SurfaceImpactDistance = tr.Distance;

				float PIDValue = PID.Update( height, SurfaceImpactDistance, Time.Delta );

				vehicle.PhysicsBody.ApplyForceAt( SurfaceImpactPoint, Vector3.Up * force * PIDValue );

				if ( Vehicle.debug_car )
					DebugOverlay.Line( tr.StartPosition, tr.EndPosition, Color.Red, 0, false );
			}
			else
			{
				Grounded = false;

				if ( Vehicle.debug_car )
					DebugOverlay.Line( Position, Position + vehicle.Rotation.Down * height, Color.Green, 0, false );
			}
		}

		public void UpdateWheelMesh()
		{
			float zOffset = Grounded ? (-1 * SurfaceImpactDistance + modelOffset) : (-1 * height + modelOffset);
			model.LocalPosition = new Vector3( model.LocalPosition.x, model.LocalPosition.y, zOffset );

			float circumference = height * MathF.PI * 2;
			float steerAngle = usesSteering ? (vehicle.Steer * this.steerAngle) : 0;
			if ( invertSteering == true )
				steerAngle = -steerAngle;

			float flip = 1.0f;
			if ( LocalRotation.z > 0 )
				flip = -1.0f;

			model.LocalRotation = new Angles(
			initialRotation.roll += MathX.RadianToDegree( vehicle.Speed * circumference / 6000 * flip * Time.Delta ),
			initialRotation.pitch + steerAngle,
			initialRotation.yaw ).ToRotation();
		}
	}
}
