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
		InputHintsHUD = GameUI.Components.Get<InputHintsHUD>();

		RefreshUI();
	}

	protected override void OnUpdate()
	{
		if ( State == GameState.Game )
		{
			SpawnSkeletonOnTimer();
		}

		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			ToggleEscapeMenu();
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
}
