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
			if ( State == GameState.Game )
				return;
			SetGameState( GameState.Game );
			StartGame();
		}
		else if ( scene.Source.ResourceId == MainMenuScene.ResourceId )
		{
			if ( State == GameState.MainMenu )
				return;
			SetGameState( GameState.MainMenu );
		}

		FadeUI.FadeOut( 0.25f );
	}

	public void FadeInMainMenu()
	{
		FadeUI.FadeIn( 0.25f, LoadMainMenuScene );
		Scene.TimeScale = 1.0f;
	}

	public void FadeInGameScene()
	{
		FadeUI.FadeIn( 0.25f, LoadGameScene );
	}

	public void LoadGameScene()
	{
		LoadScene( GameScene );
	}

	public void LoadMainMenuScene()
	{
		LoadScene( MainMenuScene );
	}

	private void LoadScene( SceneFile scene )
	{
		if ( !scene.IsValid() )
			return;
		EndGame();
		HideUI();
		Game.ActiveScene.Load( scene );
		Game.ActiveScene.Name = scene.Title;
	}
}
