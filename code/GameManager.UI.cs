using MightyBrick.GraveQuest.UI;

namespace MightyBrick.GraveQuest;

public partial class GameManager
{
	[Property, Category( "UI" )]
	public ScreenPanel MainMenuUI { get; private set; }
	[Property, Category( "UI" )]
	public ScreenPanel GameUI { get; private set; }
	[Property, Category( "UI" )]
	public ScreenPanel OptionsMenu { get; private set; }
	[Property, Category( "UI" )]
	public HUD HUDUI { get; private set; }
	[Property, Category( "UI" )]
	public Fader FadeUI { get; private set; }
	public InputHintsHUD InputHintsHUD { get; private set; }
	public EscapeMenu EscapeMenu { get; private set; }
	[Property, Category( "Sounds" )]
	public SoundEvent EscapeMenuSound { get; private set; }

	public void ToggleEscapeMenu()
	{
		if ( OptionsMenu.Enabled )
		{
			ToggleOptionsMenu();
			return;
		}

		if ( !GameUI.Enabled )
			return;

		EscapeMenu.Enabled = !EscapeMenu.Enabled;
		Scene.TimeScale = EscapeMenu.Enabled ? 0.0f : 1.0f;
		if ( EscapeMenuSound.IsValid() )
			Sound.Play( EscapeMenuSound );
	}

	public void ToggleOptionsMenu()
	{
		OptionsMenu.Enabled = !OptionsMenu.Enabled;
	}

	private void RefreshUI()
	{
		MainMenuUI.Enabled = State == GameState.MainMenu;
		GameUI.Enabled = State == GameState.Game;
		OptionsMenu.Enabled = false;
		EscapeMenu.Enabled = false;
	}

	private void HideUI()
	{
		MainMenuUI.Enabled = false;
		GameUI.Enabled = false;
		OptionsMenu.Enabled = false;
		HUDUI?.ClearAnnouncement();
	}
}
