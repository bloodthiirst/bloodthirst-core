using System.Collections.Generic;

public interface IGameState
{
    IGameState Parent { get; set; }
    IReadOnlyList<IGameState> SubStates { get; }
}
