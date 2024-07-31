using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public abstract class GameStateBase : IGameState
{
    IGameState IGameState.Parent { get; set; }

    private List<IGameState> subStates;
    List<IGameState> IGameState.SubStates => subStates;

    public GameStateBase()
    {
        subStates = new List<IGameState>();
    }

    public TState AddSubState<TState>(TState state) where TState : IGameState
    {
        state.Parent = this;
        subStates.Add(state);

        return state;
    }

    public IGameState RemoveSubState(IGameState state)
    {
        state.Parent = null;
        subStates.Remove(state);

        return state;
    }

    void IGameState.OnDestroy()
    {
        Debug.Log($"State {GetType().Name} destroyed");
    }

    void IGameState.OnEnter()
    {
        Debug.Log($"State {GetType().Name} entered");
    }

    void IGameState.OnExitUp()
    {
        Debug.Log($"State {GetType().Name} exited up");
    }

    void IGameState.OnExitDown()
    {
        Debug.Log($"State {GetType().Name} exited down");
    }

    void IGameState.OnInitialize()
    {
        Debug.Log($"State {GetType().Name} initialized");
    }
}
public class GameStateRoot : GameStateBase { }
public class GameStateFirst : GameStateBase { }
public class GameStateSecond : GameStateBase { }
public class GameStateThird : GameStateBase { }

public class GameStateMachineTests
{
    [Test( Description = "Simple game state machine Enter/Exit test" )]
    public void GameStateMachineTestsSimpleEnter()
    {
        GameStateRoot root = new GameStateRoot();
        root.AddSubState(new GameStateFirst());
        root.AddSubState(new GameStateSecond());

        GameStateMachine<GameStateBase> gameState = new GameStateMachine<GameStateBase>(root);

        gameState.Enter();
        gameState.Exit();  
    }

    [Test(Description = "Going from root to first child")]
    public void GameStateMachineTestsGoToDirectChild()
    {
        GameStateRoot root = new GameStateRoot();
        root.AddSubState(new GameStateFirst());
        root.AddSubState(new GameStateSecond());

        GameStateMachine<GameStateBase> gameState = new GameStateMachine<GameStateBase>(root);

        gameState.Enter();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateRoot));

        gameState.GoToState<GameStateFirst>();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateFirst));

    }

    [Test(Description ="Going from root to first child , then from first child to second child")]
    public void GameStateMachineTestsGoToIndirectChild()
    {
        GameStateRoot root = new GameStateRoot();
        root.AddSubState(new GameStateFirst());
        root.AddSubState(new GameStateSecond());

        GameStateMachine<GameStateBase> gameState = new GameStateMachine<GameStateBase>(root);

        gameState.Enter();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateRoot));

        gameState.GoToState<GameStateFirst>();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateFirst));

        gameState.GoToState<GameStateSecond>();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateSecond));
    }

    [Test(Description = "Going from root to first child , then from first child to third child that's located in a different branch and with more depth")]
    public void GameStateMachineTestsGoToDeeperIndirectChild()
    {
        GameStateRoot root = new GameStateRoot();
        root.AddSubState(new GameStateFirst());
        root.AddSubState(new GameStateSecond()).AddSubState(new GameStateThird());

        GameStateMachine<GameStateBase> gameState = new GameStateMachine<GameStateBase>(root);
        gameState.Initialize();

        gameState.Enter();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateRoot));

        gameState.GoToState<GameStateFirst>();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateFirst));

        gameState.GoToState<GameStateThird>();

        Assert.IsTrue(gameState.CurrentState.GetType() == typeof(GameStateThird));

        gameState.Destroy();

    }


}
