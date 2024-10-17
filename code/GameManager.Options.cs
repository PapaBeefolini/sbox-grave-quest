namespace MightyBrick.GraveQuest;

public partial class GameManager
{
	public float GameVolume { get; set; } = 100.0f;
	public float MusicVolume { get; set; } = 75.0f;

	private bool showInputHints = true;
	public bool ShowInputHints
	{
		get => showInputHints;
		set
		{
			showInputHints = value;
			InputHintsHUD.Enabled = value;
		}
	}

	public bool IsCustomizing { get; set; } = false;
	public int HatIndex { get; set; } = 0;
	public int HatColor { get; set; } = 0;
}
