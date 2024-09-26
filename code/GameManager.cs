global using Sandbox;
global using Sandbox.Utility;
global using System;
global using System.Linq;
global using System.Threading.Tasks;

namespace MightyBrick.GraveQuest;

public partial class GameManager : Component
{
	public static GameManager Instance { get; private set; }

	[Property, Category( "UI" )]
	public ScreenPanel MainMenuUI { get; private set; }
	[Property, Category( "UI" )]
	public ScreenPanel GameUI { get; private set; }
	[Property, Category( "UI" )]
	public SoundPointComponent MusicSoundPoint { get; private set; }

	public bool IsCustomizing { get; set; } = false;
	public int Score;

	public GameState State { get; private set; } = GameState.MainMenu;
	public enum GameState
	{
		MainMenu,
		Game
	}

	protected override void OnAwake()
	{
		if ( Instance.IsValid() && Instance != this )
		{
			GameObject.Destroy();
			return;
		}

		Instance = this;
		GameObject.Flags |= GameObjectFlags.DontDestroyOnLoad;
	}

	protected override void OnStart()
	{
		RefreshUI();
	}

	protected override void OnUpdate()
	{
		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			LoadMainMenuScene();
		}

		if ( State == GameState.Game )
		{
			SpawnSkeletonOnTimer();
		}
	}

	private void StartGame()
	{
		skeletonSpawningTask = SpawnInitialSkeletons();
	}

	private void SetGameState( GameState state )
	{
		State = state;
		RefreshUI();
	}

	private void RefreshUI()
	{
		MainMenuUI.GameObject.Enabled = State == GameState.MainMenu;
		GameUI.GameObject.Enabled = State == GameState.Game;
	}
}
