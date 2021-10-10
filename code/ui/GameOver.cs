using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Papa.UI
{
	public class GameOver : Panel
	{
		public Label Label { get; private set; }

		public GameOver()
		{
			Label = Add.Label();
		}

		public override void Tick()
		{
			if ( GraveQuest.Instance.State == GraveQuest.GameState.Over )
				Label.Text = "GAME OVER";
			else
				Label.Text = string.Empty;
		}
	}
}
