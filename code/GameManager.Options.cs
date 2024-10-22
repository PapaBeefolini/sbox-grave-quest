using Sandbox.Audio;
using System.Text.Json.Serialization;
using System.Text.Json;

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
}
