using Sandbox;
using System;

namespace Papa
{
	public partial class Vehicle : ModelEntity
	{
		[Net] public float Forward { get; private set; }
		[Net] public float Steer { get; private set; }

		private int lastParticles = 0;
		private int lastEngineSound = 0;

		private void AnimatePapa()
		{
			Forward = MathX.LerpTo( Forward, Controller.ForwardInput, 10 * Time.Delta );
			Steer = MathX.LerpTo( Steer, Controller.TurnInput, 10 * Time.Delta );

			papa.SetAnimParameter( "Speed", Forward );
			papa.SetAnimParameter( "Steering", -Steer );
		}

		[Event.Tick.Client]
		public void OnClientTick()
		{
			lastParticles++;
			if ( lastParticles == 2 )
			{
				lastParticles = 0;

				if ( SpeedAbsolute >= 500 )
				{
					if ( FrontLeft.Grounded )
						Particles.Create( "particles/dirt.vpcf", FrontLeft.SurfaceImpactPoint );
					if ( FrontRight.Grounded )
						Particles.Create( "particles/dirt.vpcf", FrontRight.SurfaceImpactPoint );
					if ( BackLeft.Grounded )
						Particles.Create( "particles/dirt.vpcf", BackLeft.SurfaceImpactPoint );
					if ( BackRight.Grounded )
						Particles.Create( "particles/dirt.vpcf", BackRight.SurfaceImpactPoint );
				}
			}

			lastEngineSound++;
			if ( lastEngineSound == 7 )
			{
				lastEngineSound = 0;

				var snd = Sound.FromEntity( "engine", this );
				snd.SetPitch( 1.0f + (SpeedAbsolute / 3000) );
			}
		}

		private void ThrowPizzaEffects()
		{
			papa.SetAnimParameter( "Throw", true );
			Sound.FromEntity( "throw", this );
		}
	}
}
