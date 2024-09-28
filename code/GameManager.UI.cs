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
	public InputHintsHUD InputHintsHUD { get; private set; }
	public EscapeMenu EscapeMenu { get; private set; }

	public void ToggleEscapeMenu()
	{
		if ( OptionsMenu.Enabled )
		{
			ToggleOptionsMenu();
			return;
		}

		EscapeMenu.Enabled = !EscapeMenu.Enabled;
		Scene.TimeScale = EscapeMenu.Enabled ? 0.0f : 1.0f;
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
	}
}
