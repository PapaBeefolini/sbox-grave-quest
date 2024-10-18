namespace MightyBrick.GraveQuest;

public partial class GameManager : ISceneLoadingEvents
{
	[Property, Category( "Scenes" )]
	public SceneFile MainMenuScene { get; set; }
	[Property, Category( "Scenes" )]
	public SceneFile GameScene { get; set; }

	public void AfterLoad( Scene scene )
	{
		if ( scene.Source.ResourceId == GameScene.ResourceId )
		{
			SetGameState( GameState.Game );
			StartGame();
		}
		else if ( scene.Source.ResourceId == MainMenuScene.ResourceId )
		{
			SetGameState( GameState.MainMenu );
		}

		FadeUI.FadeOut( 0.25f );
	}

	public void LoadGameScene()
	{
		FadeUI.FadeIn( 0.25f, () => { LoadScene( GameScene ); } );
	}

	public void LoadMainMenuScene()
	{
		FadeUI.FadeIn( 0.25f, () => { LoadScene( MainMenuScene ); } );
		Scene.TimeScale = 1.0f;
		GameCTS?.Cancel();
	}

	private void LoadScene( SceneFile scene )
	{
		if ( scene == null )
			return;
		EndGame();
		HideUI();
		State = GameState.Loading;
		Scene.Load( scene );
		Scene.Name = scene.Title;
	}
}
