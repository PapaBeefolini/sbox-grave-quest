namespace MightyBrick.GraveQuest;

public sealed class TimedDestroy : Component
{
	[Property] 
	public float Time { get; set; } = 1.0f;
	[Property] 
	public bool FadeOut { get; set; } = false;
	[Property, ReadOnly]
	public TimeUntil TimeUntilDestroy { get; private set; } = 0;
	public ModelRenderer[] Renderers { get; private set; }

	protected override void OnStart()
	{
		TimeUntilDestroy = Time;
		if ( FadeOut )
		{
			Renderers = Components.GetAll<ModelRenderer>( FindMode.EnabledInSelfAndDescendants ).ToArray();
		}
	}

	protected override void OnUpdate()
	{
		if ( FadeOut && TimeUntilDestroy.Fraction >= 0.75f )
		{
			foreach ( ModelRenderer renderer in Renderers )
			{
				float progress = (TimeUntilDestroy.Fraction - 0.75f) * 4.0f;
				float alpha = 1.0f - progress;
				renderer.Tint = renderer.Tint.WithAlpha( alpha );
			}
		}

		if ( TimeUntilDestroy )
		{
			GameObject.Destroy();
		}
	}
}

public static partial class Extensions
{
	public static void DestroyDelayed( this GameObject self, float seconds = 1.0f, bool fade = false )
	{
		TimedDestroy component = self.Components.Create<TimedDestroy>();
		component.Time = seconds;
		component.FadeOut = fade;
	}
}
