namespace MightyBrick.GraveQuest;

public partial class GameManager
{
	public float GameVolume { get; set; } = 100.0f;
	public float MusicVolume { get; set; } = 100.0f;
	public bool ShowInputHints
	{
		get => showInputHints;
		set
		{
			showInputHints = value;
			InputHintsHUD.Enabled = value;
		}
	}
	private bool showInputHints;
}
