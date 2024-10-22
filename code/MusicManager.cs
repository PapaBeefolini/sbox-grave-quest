namespace MightyBrick.GraveQuest;

public sealed class MusicManager : Component
{
	public static MusicManager Instance { get; private set; }

	[RequireComponent]
	public SoundPointComponent SoundPoint { get; private set; }

	[Property]
	public SoundEvent MenuMusic { get; set; }
	[Property]
	public SoundEvent GameMusic { get; set; }
	[Property]
	public SoundEvent GameOverSound { get; set; }

	private float defaultVolume = 1.0f;
	private float targetVolume = 1.0f;

	protected override void OnAwake()
	{
		if ( Instance.IsValid() && Instance != this )
		{
			GameObject.Destroy();
			return;
		}
		Instance = this;
		defaultVolume = SoundPoint.Volume;
	}

	protected override void OnStart()
	{
		PlayMenuMusic();
	}

	protected override void OnUpdate()
	{
		SoundPoint.Volume = SoundPoint.Volume.LerpTo( targetVolume, Time.Delta * 8.0f );
	}

	public void PlayMenuMusic()
	{
		PlaySound( MenuMusic );
	}

	public void PlayGameMusic()
	{
		PlaySound( GameMusic );
	}

	public void PlayGameOverMusic()
	{
		PlaySound( GameOverSound );
	}

	public void PlaySound( SoundEvent soundEvent )
	{
		targetVolume = defaultVolume;
		SoundPoint.Volume = targetVolume;
		SoundPoint.SoundEvent = soundEvent;
		SoundPoint.StartSound();
	}

	public void FadeOut()
	{
		targetVolume = 0.0f;
	}

	public void Stop()
	{
		SoundPoint.StopSound();
	}
}
