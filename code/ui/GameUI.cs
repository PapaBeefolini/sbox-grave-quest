using Sandbox.UI;

namespace Papa.UI
{
	public partial class GameUI : Sandbox.HudEntity<RootPanel>
	{
		public GameUI()
		{
			if ( IsClient )
			{
				RootPanel.StyleSheet.Load( "/ui/GameUI.scss" );

				ChatBox chat = RootPanel.AddChild<ChatBox>();
				chat.Style.FontFamily = "FredokaOne-Regular";
				RootPanel.AddChild<VoiceList>();

				RootPanel.AddChild<Timer>();
				RootPanel.AddChild<Score>();
				RootPanel.AddChild<GameOver>();
			}
		}
	}
}
