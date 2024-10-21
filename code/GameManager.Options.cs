using Sandbox.Audio;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MightyBrick.GraveQuest;

public class GameSettings
{
	private float masterVolume;
	public float MasterVolume
	{
		get
		{
			return masterVolume;
		}
		set
		{
			masterVolume = value;
			GameManager.UpdateSettings();
		}
	}
	private float musicVolume;
	public float MusicVolume
	{
		get
		{
			return musicVolume;
		}
		set
		{
			musicVolume = value;
			GameManager.UpdateSettings();
		}
	}
	public bool ShowInputHints { get; set; } = true;
	public int HatIndex { get; set; } = 0;
	public int HatColor { get; set; } = 0;
}

public partial class GameManager
{
	public static string SaveFilePath => "gamesettings.json";

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
		UpdateSettings();
		JsonSerializerOptions options = new JsonSerializerOptions
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.Never,
			WriteIndented = true
		};
		string json = JsonSerializer.Serialize( GameSettings, options );
		FileSystem.Data.WriteAllText( SaveFilePath, json );
	}

	public static void Load()
	{
		GameSettings = FileSystem.Data.ReadJson<GameSettings>( SaveFilePath, new() );
	}

	public static void UpdateSettings()
	{
		Mixer.Master.Volume = GameSettings.MasterVolume / 100;
		Mixer[] channels = Mixer.Master.GetChildren();
		channels[0].Volume = GameSettings.MusicVolume / 100;
	}
}
