using Sandbox;
using System;

namespace Papa
{
	public partial class Vehicle : ModelEntity
	{
		[ConVar.Replicated( "debug_car" )]
		public static bool debug_car { get; set; } = false;

		[Net, Predicted] public VehicleController Controller { get; set; }

		[Net] public float Speed { get; private set; }
		[Net] public float SpeedAbsolute { get; private set; }
		[Net] public bool Grounded { get; private set; }

		// Wheels
		[Net] public Wheel FrontLeft { get; private set; }
		[Net] public Wheel FrontRight { get; private set; }
		[Net] public Wheel BackLeft { get; private set; }
		[Net] public Wheel BackRight { get; private set; }

		//----------//

		// Main Properties
		private float gravity => 900.0f;

		private float forwardSpeed => 1500.0f;
		private float maximumSpeed => 1600.0f;
		private float turnSpeed => 20.0f;
		private float forwardGrip => 0.4f;
		private float sidewaysGrip => 4.0f;
		private float wheelSpinTraction => 8.0f;

		// Entities
		private SpotLightEntity headlights;
		private AnimatedEntity papa;

		//----------//

		private float lastPizza;

		public override void Spawn()
		{
			base.Spawn();

			Tags.Add( "vehicle" );

			string modelName = "models/car.vmdl";

			SetModel( modelName );
			SetMaterialGroup( Rand.Int( 0, MaterialGroupCount ) );
			SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			EnableSelfCollisions = false;

			PhysicsBody.Mass = 20000;

			Components.Create<Camera>();
			Controller = new VehicleController();

			EyeLocalPosition = new Vector3( -6, 0, 100 );

			// Headlights so papa b can see
			headlights = new SpotLightEntity
			{
				Parent = this,
				Transform = new Transform( new Vector3( 110, 0, 52 ), Rotation.Identity ),
				Color = new Color( 1, 0.92f, 0.74f, 1 ),
				Range = 1200,
				OuterConeAngle = 70,
				DynamicShadows = true,
			};

			// The man himself
			papa = new AnimatedEntity
			{
				Parent = this,
				Transform = new Transform( new Vector3( -22, 0, 3.5f ), Rotation.Identity ),
			};
			papa.SetModel( "models/papa.vmdl" );

			// Wheels
			FrontLeft = new Wheel();
			FrontLeft.Setup( this, new Transform( new Vector3( 95.5f, 52, 33 ), Rotation.From( 0, -90, 0 ) ), "models/wheel_small.vmdl", 22, 18, true );
			FrontRight = new Wheel();
			FrontRight.Setup( this, new Transform( new Vector3( 95.5f, -52, 33 ), Rotation.From( 0, 90, 0 ) ), "models/wheel_small.vmdl", 22, 18, true);
			BackLeft = new Wheel();
			BackLeft.Setup( this, new Transform( new Vector3( -64.5f, 56, 24.5f ), Rotation.From( 0, -90, 0 ) ), "models/wheel_big.vmdl", 28, 24, false );
			BackRight = new Wheel();
			BackRight.Setup( this, new Transform( new Vector3( -64.5f, -56, 24.5f ), Rotation.From( 0, 90, 0 ) ), "models/wheel_big.vmdl", 28, 24, false );
		}

		public override void Simulate( Client owner )
		{
			if ( owner == null ) return;
			if ( !IsServer ) return;

			Controller?.Simulate( owner, this, null );

			AnimatePapa();

			lastPizza += Time.Delta;
		}

		[Event.Physics.PreStep]
		public void OnPrePhysicsStep()
		{
			if ( !IsServer )
				return;

			if ( debug_car )
				Log.Info( "Speed: " + Speed );

			PhysicsBody.Velocity += Vector3.Down * gravity * Time.Delta;

			ClampAngles();

			Vector3 localVelocity = PhysicsBody.Rotation.Inverse * PhysicsBody.SelfOrParent.Velocity;
			Speed = localVelocity.x;
			SpeedAbsolute = MathF.Abs( Speed );
			Vector3 localAngularVelocity = PhysicsBody.SelfOrParent.AngularVelocity;
			
			FrontLeft.Raycast();
			FrontRight.Raycast();
			BackLeft.Raycast();
			BackRight.Raycast();

			FrontLeft.UpdateWheelMesh();
			FrontRight.UpdateWheelMesh();
			BackLeft.UpdateWheelMesh();
			BackRight.UpdateWheelMesh();

			Grounded = (FrontLeft.Grounded || FrontRight.Grounded || BackLeft.Grounded || BackRight.Grounded) ? true : false;

			if ( Grounded )
			{
				PhysicsBody.LinearDrag = 20;
				PhysicsBody.AngularDrag = 100;

				// Decrease multiplier if you want to drive slower in reverse
				float force = (Speed < 0) ? forwardSpeed * 1.0f : forwardSpeed;

				// Main Gas
				float acceleration = 1.2f - (SpeedAbsolute / 5000.0f).Clamp( 0.0f, 1.2f );
				if ( Speed <= maximumSpeed && Speed >= -maximumSpeed )
					PhysicsBody.Velocity += Rotation.Forward * Controller.ForwardInput * force * acceleration * Time.Delta;

				// Sideways grip & Traction
				PhysicsBody.Velocity += Rotation.Right * localVelocity.y * sidewaysGrip * Time.Delta;
				PhysicsBody.AngularVelocity += Rotation.Up * -localAngularVelocity.z * wheelSpinTraction * Time.Delta;

				// Auto brakes
				PhysicsBody.Velocity += Rotation.Backward * Speed * forwardGrip * Time.Delta;
				if ( Speed > -250 && Speed < 250 )
					PhysicsBody.Velocity += Rotation.Backward * Speed * forwardGrip * 4 * Time.Delta;

				// Steering
				float zVel = MathX.Clamp( Speed / 1000, -1.0f, 1.0f );
				PhysicsBody.AngularVelocity += Rotation.Up * Controller.TurnInput * turnSpeed * zVel * Time.Delta;
			}
			else
			{
				PhysicsBody.LinearDrag = 1;
				PhysicsBody.AngularDrag = 1;
			}
		}

		public void ThrowPizza()
		{
			if ( lastPizza < 1.5f )
				return;
			lastPizza = 0;

			ThrowPizzaEffects();

			Pizza pizza = new Pizza();
			pizza.Transform = new Transform( Position + Rotation.Up * 80, Rotation );
			pizza.PhysicsBody.Velocity = PhysicsBody.Velocity + (Rotation.Forward * 700) + (Rotation.Up * 350);
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			if ( !IsServer )
				return;

			PhysicsBody body = PhysicsBody;
			if ( !body.IsValid() )
				return;

			body = body.SelfOrParent;
			if ( !body.IsValid() )
				return;

			if ( other is Skeleton skeleton )
			{
				skeleton.Kill( Position, SpeedAbsolute );
				PhysicsBody.ApplyForceAt( skeleton.Position, (Position - skeleton.Position) * PhysicsBody.Mass * 150 );
				CameraShake( To.Single( Client ), 1.0f + (SpeedAbsolute / 250) );
			}
		}

		[ClientRpc]
		public void CameraShake(float amount)
		{
			//_ = new Sandbox.ScreenShake.Perlin(0.5f, 3, amount );
		}

		private void ClampAngles()
		{
			Rotation clampedRotation = Rotation;

			float maxX = 0.3f;
			float maxY = 0.3f;

			clampedRotation.x = clampedRotation.x.Clamp( -maxX, maxX );
			clampedRotation.y = clampedRotation.y.Clamp( -maxY, maxY );

			Rotation = clampedRotation;
		}
	}
}
