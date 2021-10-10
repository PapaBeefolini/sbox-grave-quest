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

				ForwardInput = (Input.Down( InputButton.Forward ) ? 1 : 0) + (Input.Down( InputButton.Back ) ? -1 : 0);
				TurnInput = (Input.Down( InputButton.Left ) ? 1 : 0) + (Input.Down( InputButton.Right ) ? -1 : 0);
				if ( Input.Pressed( InputButton.Jump ) )
					((Vehicle)Pawn).ThrowPizza();
			}
		}
	}
}
