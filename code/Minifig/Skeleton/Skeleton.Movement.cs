﻿namespace MightyBrick.GraveQuest;

public partial class Skeleton
{
	[RequireComponent]
	public NavMeshAgent Agent { get; private set; }

	private TimeUntil timeUntilMove = 2.6f;

	private void Move()
	{
		if ( IsDead || !Agent.IsValid || Scene.NavMesh.IsGenerating )
			return;

		if ( timeUntilMove )
		{
			if ( Game.Random.Int( 5 ) > 0 )
				MoveToRandomPoint();
			else
				Stop(); // Small chance of just idling for a bit
		}
	}

	private void MoveToRandomPoint()
	{
		Agent.MoveTo( Scene.NavMesh.GetRandomPoint() ?? Vector3.One );
		timeUntilMove = Game.Random.Float( 2.0f, 4.0f );
	}

	private void Stop()
	{
		Agent.Stop();
		timeUntilMove = Game.Random.Float( 0.25f, 1.25f );
	}
}