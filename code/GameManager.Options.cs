using Sandbox.Audio;

namespace MightyBrick.GraveQuest;

public class GameSettings
{
	private float masterVolume = 100.0f;
	public float MasterVolume
	{
		get => masterVolume;
		set
		{
			masterVolume = value;
			Mixer.Master.Volume = masterVolume / 100.0f;
		}
	}
	private float musicVolume = 70.0f;
	public float MusicVolume
	{
		get => musicVolume;
		set
		{
			musicVolume = value;
			Mixer.Master.GetChildren()[0].Volume = musicVolume / 100.0f;
		}
	}
	public bool ShowInputHints { get; set; } = true;
	public int HatIndex { get; set; } = 0;
	public int HatColor { get; set; } = 0;
}

public partial class GameManager
{
	private static GameSettings gameSettings { get; set; }
	public static GameSettings GameSettings
	{
		get
		{
			if ( gameSettings is null )
				Load();
			return gameSettings;
		}
		set
		{
			gameSettings = value;
		}
	}

	public bool IsCustomizing { get; set; } = false;

	public static void Save()
	{
		try
		{
			FileSystem.Data.WriteJson( "gamesettings.json", GameSettings );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Failed to save game settings: {ex.Message}" );
		}
	}

	public static void Load()
	{
		try
		{
			GameSettings = FileSystem.Data.ReadJson<GameSettings>( "gamesettings.json", new() );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Failed to load game settings: {ex.Message}" );
			GameSettings = new();
		}
	}
}
