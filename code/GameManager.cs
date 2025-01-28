global using Sandbox;
global using Sandbox.Utility;
global using Sandbox.Events;
global using Sandbox.Services;
global using System;
global using System.Linq;
using MightyBrick.GraveQuest.UI;
using System.Threading;
using System.Threading.Tasks;

namespace MightyBrick.GraveQuest;

public partial class GameManager : Component, IGameEventHandler<SkeletonDiedEvent>
{
	public static GameManager Instance { get; private set; }
	public static CancellationTokenSource GameCTS { get; private set; }
	public static bool Paused => Game.ActiveScene.TimeScale != 1.0f;

	[Property, Category( "Sounds" )]
	public SoundEvent GameStartSound { get; set; }
	[Property, Category( "Sounds" )]
	public SoundEvent CountdownSound { get; set; }

	public EnemySpawner EnemySpawner { get; private set; }
	public bool IsGameRunning { get; private set; } = false;
	public float TimeRemaining { get; private set; } = 0.0f;
	public int Score = 0;

	public GameState State { get; private set; } = GameState.MainMenu;
	public enum GameState
	{
		MainMenu,
		Game,
		Loading
	}

	public double GlobalKills => Stats.Global.Get( "kills" ).Value;

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

		Mouse.Visible = false;

		Load();

		EnemySpawner = GetComponent<EnemySpawner>( true );
		EnemySpawner.Enabled = false;

		EscapeMenu = GameUI.Components.Get<EscapeMenu>();
		RefreshUI();
	}

	protected override void OnUpdate()
	{
		HandleInputs();

		if ( TimeRemaining <= 0.0f && IsGameRunning )
			_ = RestartGame();
		if ( IsGameRunning )
			TimeRemaining -= Time.Delta;
	}

	private void HandleInputs()
	{
		if ( Input.EscapePressed )
		{
			Input.EscapePressed = false;
			ToggleEscapeMenu();
		}
	}

	public void OnGameEvent( SkeletonDiedEvent args )
	{
		if ( !IsGameRunning )
			return;
		TimeRemaining += 2.0f;
		Score++;
		Stats.Increment( "kills", 1 );
	}

	private void StartGame()
	{
		GameCTS = new CancellationTokenSource();
		TimeRemaining = 8.0f;
		Score = 0;
		EnemySpawner.Enabled = true;
		EnemySpawner?.SpawnEnemies( 5 );
		_ = StartCountdown();
	}

	private void EndGame()
	{
		GameCTS?.Cancel();
		EnemySpawner.Enabled = false;
		IsGameRunning = false;
	}

	private async Task RestartGame()
	{
		EndGame();
		GameCTS = new CancellationTokenSource();
		HUDUI?.DisplayAnnouncement( "Game Over", 8.0f, $"Score: {Score}", "ui/skull-small.png" );
		MusicManager.Instance.PlayGameOverMusic();
		await GameTask.DelaySeconds( 6.0f, GameCTS.Token );
		LoadGameScene();
	}

	private async Task StartCountdown()
	{
		await GameTask.DelaySeconds( 1.0f, GameCTS.Token );
		for ( int i = 3; i >= 1; i-- )
		{
			Sound.Play( CountdownSound );
			HUDUI?.DisplayAnnouncement( i.ToString(), 1.0f );
			await GameTask.DelaySeconds( 1.0f, GameCTS.Token );
		}
		IsGameRunning = true;
		HUDUI?.DisplayAnnouncement( "GO!", 1.0f );
		Sound.Play( GameStartSound );
		MusicManager.Instance.PlayGameMusic();
	}

	private void SetGameState( GameState state )
	{
		State = state;
		RefreshUI();
	}

	public async Task<Leaderboards.Board2> GetLeaderboard()
	{
		Leaderboards.Board2 board = Leaderboards.GetFromStat( "brick.gravequest", "kills" );
		board.MaxEntries = 100;
		await board.Refresh();
		return board;
	}
}
