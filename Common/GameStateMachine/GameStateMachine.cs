using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

/// <summary>
/// <para>A class dedicated to transition and move from one state of the game to the other in a clean way </para>
/// <para>Implementation wise , all you have to do is define a <typeparamref name="TGameState"/> base class for your game state and initialize the <see cref="GameStateMachine{TGameState}"/> with the root state</para>
/// <para>Once the state machine is created , call <see cref="Initialize"/> and use <see cref="GoToState{TState}"/> to go from the current state to an other</para>
/// <para>On shutdown , call <see cref="Destroy "/> to have a clean all the states</para>
/// </summary>
/// <typeparam name="TGameState"></typeparam>
public class GameStateMachine<TGameState> where TGameState : IGameState
{
    public TGameState Root { get; private set; }

    private Dictionary<Type, TGameState> gameStateTypes;
    public IReadOnlyDictionary<Type, TGameState> AllGameStates => gameStateTypes;

    public TGameState CurrentState { get; set; }

    public GameStateMachine(TGameState root)
    {
        Root = root;
        CurrentState = root;
        QueryGameStateTypes();
    }

    private void QueryGameStateTypes()
    {
        gameStateTypes = new Dictionary<Type, TGameState>();

        List<(Type, TGameState)> allType = new List<(Type, TGameState)>();

        allType.AddRange(QueryGameStateTypesRecursive(Root));

        // checkf or no duplicated types
        for (int i = 0; i < allType.Count; i++)
        {
            (Type, TGameState) t = allType[i];

            if (!gameStateTypes.TryAdd(t.Item1, t.Item2))
            {
                throw new Exception($"The type {t.Item1.Name} has been added more than once , please check the way you created your {nameof(TGameState)} root");
            }
        }
    }

    private IEnumerable<(Type, TGameState)> QueryGameStateTypesRecursive(TGameState gameState)
    {
        yield return (gameState.GetType(), gameState);

        for (int i = 0; i < gameState.SubStates.Count; i++)
        {
            IGameState g = gameState.SubStates[i];

            foreach ((Type, TGameState) t in QueryGameStateTypesRecursive((TGameState)g))
            {
                yield return t;
            }
        }
    }

    /// <summary>
    /// <para>Finds the common parent state between A and B</para>
    /// <para>returns the paths leading from A->parent and B->parent </para>
    /// </summary>
    /// <param name="stateA"></param>
    /// <param name="stateB"></param>
    /// <param name="parentToDest">the first element is the common parent , and the last element is B's parent</param>
    /// <param name="srcToParent">the first element is A's parent , and the last element is the common parent</param>
    /// <returns></returns>
    public void GoToState(Type destType, List<TGameState> srcToParent, List<TGameState> parentToDest  , out TGameState dest)
    {
        if (!gameStateTypes.TryGetValue(destType, out TGameState state))
        {
            throw new Exception($"The state machine doesn't contains a state of type {destType.Name}");
        }

        dest = state;

        TGameState commonParent = FindCommonParent(CurrentState , dest, srcToParent , parentToDest);

        parentToDest.Reverse();
    }

    /// <summary>
    /// Returns the depth of the node , a depth of 0 means it's the root node
    /// </summary>
    /// <param name="stateA"></param>
    /// <returns></returns>
    private int GetDepth(IGameState state)
    {
        int depth = 0;
        while (state.Parent != null)
        {
            depth++;
            state = state.Parent;
        }

        return depth;
    }

    /// <summary>
    /// <para>Finds the common parent state between A and B</para>
    /// <para>returns the paths leading from A->parent and B->parent </para>
    /// <para>for path of A->parent : the first element is A's parent , and the last element is the common parent</para>
    /// <para>for path of B->parent : the first element is B's parent , and the last element is the common parent</para>
    /// </summary>
    /// <param name="stateA"></param>
    /// <param name="stateB"></param>
    /// <param name="pathAToParent"></param>
    /// <param name="pathBToParent"></param>
    /// <returns></returns>
    private TGameState FindCommonParent(TGameState stateA, TGameState stateB, List<TGameState> pathAToParent, List<TGameState> pathBToParent)
    {

        if (stateA.Equals(stateB))
            return stateA;

        int depthA = GetDepth(stateA);
        int depthB = GetDepth(stateB);

        // start with the assumption that A is deeper than B in terms of depth
        IGameState deeper = stateA;
        IGameState higher = stateB;
        List<TGameState> deeperPath = pathAToParent;
        List<TGameState> higherPath = pathBToParent;

        int diff = depthA - depthB;

        // otherwise switch the values accordingly
        if (depthB > depthA)
        {
            diff = depthB - depthA;
            deeper = stateB;
            higher = stateA;
            deeperPath = pathBToParent;
            higherPath = pathAToParent;
        }

        // equalize the depth to bring the deeper to the same level as the higher
        for (int i = 0; i < diff; ++i)
        {
            deeper = deeper.Parent;
            deeperPath.Add((TGameState)deeper);
        }

        // at this point both nodes have the same depth of "higher"

        // keep climbing up the tree until we find a common parent
        while (deeper != higher)
        {
            deeper = deeper.Parent;
            deeperPath.Add((TGameState)deeper);

            higher = higher.Parent;
            higherPath.Add((TGameState)higher);
        }

        Assert.AreEqual(deeper, higher);

        return (TGameState)deeper;
    }


}
