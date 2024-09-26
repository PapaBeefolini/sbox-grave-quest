global using Sandbox.Events;

namespace MightyBrick.GraveQuest;

public record SkeletonDiedEvent() : IGameEvent;

public record PizzaThrownEvent() : IGameEvent;
