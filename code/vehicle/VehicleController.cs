using Sandbox;

namespace Papa
{
	public class VehicleController : PawnController
	{
		public float ForwardInput { get; private set; }
		public float TurnInput { get; private set; }

		public override void Simulate()
		{
			using ( Prediction.Off() )
			{
				if ( GraveQuest.Instance.State != GraveQuest.GameState.Active )
				{
					ForwardInput = 0;
					TurnInput = 0;
					return;
				}

				ForwardInput = Input.Forward;
				TurnInput = Input.Left;

				if ( Input.UsingController )
				{
					if ( Input.Down( InputButton.PrimaryAttack ) )
						ForwardInput = 1;
					else if ( Input.Down( InputButton.SecondaryAttack ) )
						ForwardInput = -1;
					else
						ForwardInput = 0;
				}

				if ( Input.Pressed( InputButton.Jump ) )
					((Vehicle)Pawn).ThrowPizza();
			}
		}
	}
}
