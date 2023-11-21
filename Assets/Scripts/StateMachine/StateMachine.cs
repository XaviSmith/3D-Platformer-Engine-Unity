using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    StateNode current;
    Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>(); //Dictionaries map keys to values and cannot have duplicate keys
    HashSet<ITransition> anyTransitions = new HashSet<ITransition>(); //Transitions that can happen from any state

    //Updates our state. Not to be confused with Unity Update. Called by a Monobehaviour's Update function (see PlayerController for an example)
    public void Update()
    {
        ITransition transition = GetTransition();
        if(transition != null)
        {
            ChangeState(transition.ToState);
        }

        current.State?.Update(); //? here basically just means if State is null, then just do nothing, or if(current.State != null){ Update(); }
    }

    //Same as above.
    public void FixedUpdate()
    {
        current.State?.FixedUpdate();
    }


    public void SetState(IState state)
    {
        current = nodes[state.GetType()]; //go to dictionary entry with the key state, GetType() so it's generic
        current.State?.OnEnter();
    }

    void ChangeState(IState _state)
    {
        if (_state == current.State) return; 

        IState prevState = current.State;
        IState nextState = nodes[_state.GetType()].State;

        prevState?.OnExit();
        nextState?.OnEnter();
        current = nodes[_state.GetType()];
    }

    ITransition GetTransition()
    {
        foreach(ITransition t in anyTransitions)
        {
            if (t.Predicate.Evaluate())
                return t;
        }

        foreach(ITransition t2 in current.Transitions)
        {
            if (t2.Predicate.Evaluate())
                return t2;
        }

        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    //Gets a state node if it exists, adds it if it doesn't
    StateNode GetOrAddNode(IState _state)
    {
        StateNode _node = nodes.GetValueOrDefault(_state.GetType()); //Grab the node form the dict if it exists, return null if it doesn't.

        if(_node == null)
        {
            _node = new StateNode(_state);
            nodes.Add(_state.GetType(), _node);
        }

        return _node;
    }


    class StateNode
    {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; } //hashsets are essentially lists that don't allow duplicates, which makes them faster and allow for Union/Intersection etc operations

        public StateNode(IState _state)
        {
            State = _state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate predicate)
        {
            Transitions.Add(new Transition(to, predicate));
        }
    }
}
