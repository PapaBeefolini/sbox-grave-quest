using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Papa.UI
{
	public class Timer : Panel
	{
		public Label TimerLabel { get; private set; }

		public Timer()
		{
			TimerLabel = Add.Label();
		}

		public override void Tick()
		{
			int min = (int)MathF.Floor( GraveQuest.Instance.TimeLeft / 60 );
			int sec = (int)MathF.Floor( GraveQuest.Instance.TimeLeft % 60 );
			TimerLabel.Text = min.ToString( "00" ) + ":" + sec.ToString( "00" );

			PanelTransform pt = new PanelTransform();
			pt.AddRotation( 0, 0, System.MathF.Sin( 4 * Time.Now ) * 4 );
			Style.Transform = pt;
			Style.Dirty();
		}
	}
}
