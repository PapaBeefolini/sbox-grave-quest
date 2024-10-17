global using Sandbox;
global using Sandbox.Utility;
global using Sandbox.Events;
global using System;
global using System.Linq;
using MightyBrick.GraveQuest.UI;
using System.Threading;
using System.Threading.Tasks;

namespace MightyBrick.GraveQuest;

public partial class GameManager : Component
{
	public static GameManager Instance { get; private set; }
	public static CancellationTokenSource GameCTS { get; private set; }

	[Property, Category( "Sounds" )]
	public SoundEvent GameStartSound { get; set; }
	[Property, Category( "Sounds" )]
	public SoundEvent CountdownSound { get; set; }

	public EnemySpawner EnemySpawner { get; private set; }
	public bool IsGameRunning { get; private set; } = false;
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

		EnemySpawner = GetComponent<EnemySpawner>( true );
		EnemySpawner.Enabled = false;

		EscapeMenu = GameUI.Components.Get<EscapeMenu>();
		InputHintsHUD = GameUI.Components.Get<InputHintsHUD>();
		InputHintsHUD.Enabled = ShowInputHints;

		RefreshUI();
	}

	protected override void OnUpdate()
	{
		HandleInputs();
	}

	private void HandleInputs()
	{
		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			ToggleEscapeMenu();
		}
	}

	private void StartGame()
	{
		GameCTS = new CancellationTokenSource();
		EnemySpawner.Enabled = true;
		EnemySpawner?.SpawnEnemies( 5 );
		_ = StartCountdown();
		Log.Info( "Start" );
	}

	private void EndGame()
	{
		GameCTS?.Cancel();
		EnemySpawner.Enabled = false;
		IsGameRunning = false;
		Log.Info( "End" );
	}

	private async Task StartCountdown()
	{
		await GameTask.DelaySeconds( 1.0f );
		if ( GameCTS.IsCancellationRequested )
			return;
		for ( int i = 3; i >= 1; i-- )
		{
			if ( GameCTS.IsCancellationRequested )
				return;
			Sound.Play( CountdownSound );
			HUDUI?.Announce( i.ToString(), 1.0f );
			await GameTask.DelaySeconds( 1.0f );
		}
		if ( GameCTS.IsCancellationRequested )
			return;
		HUDUI?.Announce( "GO!", 1.0f );
		Sound.Play( GameStartSound );
		IsGameRunning = true;
	}

	private void SetGameState( GameState state )
	{
		State = state;
		RefreshUI();
	}
}
