global using Sandbox;
global using Sandbox.Utility;
global using System;
global using System.Linq;
global using System.Threading.Tasks;
using MightyBrick.GraveQuest.UI;

namespace MightyBrick.GraveQuest;

public partial class GameManager : Component
{
	public static GameManager Instance { get; private set; }

	[Property, Category( "UI" )]
	public ScreenPanel MainMenuUI { get; private set; }
	[Property, Category( "UI" )]
	public ScreenPanel GameUI { get; private set; }
	public EscapeMenu EscapeMenu { get; private set; }
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

	protected override void OnEnabled()
	{
		if ( Instance.IsValid() && Instance != this )
		{
			GameObject.Destroy();
			return;
		}
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

		EscapeMenu = GameUI.Components.Get<EscapeMenu>();
		RefreshUI();
	}

	protected override void OnUpdate()
	{
		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			ToggleEscapeMenu();
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

	public void ToggleEscapeMenu()
	{
		EscapeMenu.Enabled = !EscapeMenu.Enabled;
		Scene.TimeScale = EscapeMenu.Enabled ? 0.0f : 1.0f;
	}

	private void RefreshUI()
	{
		MainMenuUI.Enabled = State == GameState.MainMenu;
		GameUI.Enabled = State == GameState.Game;
		EscapeMenu.Enabled = false;
	}

	private void HideUI()
	{ 
		MainMenuUI.Enabled = false;
		GameUI.Enabled = false;
	}
}
