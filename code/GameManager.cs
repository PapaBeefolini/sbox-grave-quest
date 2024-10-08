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

	public bool IsCountdownActive { get; private set; } = false;
	public TimeUntil TimeUntilGameStart { get; private set; }
	private int lastSecond;

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
		HandleStartCountdown();
	}

	private void HandleInputs()
	{
		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			ToggleEscapeMenu();
		}
	}

	private void HandleStartCountdown()
	{
		int currentSecond = MathX.CeilToInt( TimeUntilGameStart.Relative );
		if ( currentSecond < 0 )
			return;
		if ( State == GameState.Game && TimeUntilGameStart.Relative <= 3.0f )
			IsCountdownActive = true;

		if ( !IsCountdownActive )
			return;

		if ( currentSecond != lastSecond )
		{
			lastSecond = currentSecond;
			if ( currentSecond == 0 )
				Sound.Play( GameStartSound );
			else
				Sound.Play( CountdownSound );
			Log.Info( currentSecond );
		}
		if ( !TimeUntilGameStart )
			return;
		IsCountdownActive = false;
		IsGameRunning = true;
	}

	private void StartGame()
	{
		GameCTS = new CancellationTokenSource();
		EnemySpawner.Enabled = true;
		EnemySpawner?.SpawnEnemy( 5 );
		TimeUntilGameStart = 4.0f;
		EnemySpawner?.Fart();
		Log.Info( "Start" );
	}

	private void EndGame()
	{
		GameCTS?.Cancel();
		EnemySpawner.Enabled = false;
		IsCountdownActive = false;
		IsGameRunning = false;
		Log.Info( "End" );
	}

	private void SetGameState( GameState state )
	{
		State = state;
		RefreshUI();
	}
}
