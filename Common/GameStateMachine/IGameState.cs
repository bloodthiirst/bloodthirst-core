using System.Collections.Generic;

public interface IGameState
{
    IGameState Parent { get; set; }
    List<IGameState> SubStates { get; }
    void OnInitialize();
    void OnEnter();
    void OnExitUp();
    void OnExitDown();
    void OnDestroy();
}
