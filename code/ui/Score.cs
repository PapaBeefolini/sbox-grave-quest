using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Papa.UI
{
	public class Score : Panel
	{
		public Label ScoreLabel { get; private set; }

		public Score()
		{
			Add.Image( "/ui/skull.png" );
			ScoreLabel = Add.Label();
		}

		public override void Tick()
		{
			PanelTransform pt = new PanelTransform();
			pt.AddRotation( 0, 0, System.MathF.Sin( 4 * Time.Now ) * 8);
			Style.Transform = pt;
			Style.Dirty();
			ScoreLabel.Text = GraveQuest.Instance.SkeletonsKilled.ToString();
		}
	}
}
