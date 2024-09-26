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
		HideUI();
		Game.ActiveScene.Load( scene );
		Game.ActiveScene.Name = scene.Title;
	}
}
