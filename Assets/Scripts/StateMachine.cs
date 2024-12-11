using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class <c>StateMachine</c> models a general state machine. Can add states and each of their exiting transitions.
/// Can also add transitions to states that can occur from any state
/// </summary>
/// <remarks>Code taken from: <see href="https://youtu.be/V75hgcsCGOM"/></remarks>
public class StateMachine
{
    /// <summary>
    /// Class <c>Transition</c> models a transition between states
    /// </summary>
    private class Transition
    {
        // Moving to this state...
        public IState To { get; }
        // If this condition is true
        public Func<bool> Condition { get; }
        
        /// <summary>
        /// Constructor for <c>Transition</c> class
        /// </summary>
        /// <param name="to">the state this transition will move to</param>
        /// <param name="condition">this condition must be true for this transition to be executed</param>
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }

        public static bool operator !(Transition obj) => obj == null;
    }
    
    // FIELDS
    private IState _currentState;
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();

    private static List<Transition> _emptyTransitions = new List<Transition>(0);
    
    // METHODS
    /// <summary>
    /// Update method for current state
    /// </summary>
    /// <remarks>Uses Update()</remarks>
    public void Tick()
    {
        Transition transition = GetTransition();
        if (transition != null) SetState(transition.To);
        
        _currentState?.Tick();
    }
    
    /// <summary>
    /// Update method for current state
    /// </summary>
    /// <remarks>
    /// Uses FixedUpdate() and can't transition states
    /// </remarks>
    public void FixedTick()
    {
        _currentState?.FixedTick();
    }

    /// <summary>
    /// Changes the current state
    /// </summary>
    /// <param name="state">the new current state</param>
    public void SetState(IState state)
    {
        if (_currentState == state) return;
        
        _currentState?.OnExit();
        _currentState = state;
        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
        _currentTransitions ??= _emptyTransitions;
        
        _currentState.OnEnter();
    }

    
    /// <summary>
    /// Adds a transition to the dictionary of all transitions
    /// </summary>
    /// <param name="from">the initial state</param>
    /// <param name="to">the state moved to after the transition</param>
    /// <param name="condition">the predicate required for this transition to occur</param>
    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        // if from state not already in dictionary
        if (!_transitions.TryGetValue(from.GetType(), out var transitions))
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }
        
        _transitions[from.GetType()].Add(new Transition(to, condition));
    }
    
    /// <summary>
    /// Adds a transition to the list of any transitions. These transitions can occur from any state.
    /// </summary>
    /// <param name="to">the state moved to after the transition</param>
    /// <param name="condition">the predicate required for this transition to occur</param>
    public void AddAnyTransition(IState to, Func<bool> condition)
    {
        _anyTransitions.Add(new Transition(to, condition));
    }

    /// <summary>
    /// Checks the list of any transitions first, then the list of transitions from the current state, looking for the
    /// first transition where the condition is satisfied
    /// </summary>
    /// <returns>the satisfied transition, otherwise null</returns>
    private Transition GetTransition()
    {
        // Check list of any (interuppting) transitions first 
        foreach (var transition in _anyTransitions.Where(transition => transition.Condition()))
            return transition;
        
        // Check list of transitions from current state
        foreach (var transition in _currentTransitions.Where(transition => transition.Condition()))
        {
            return transition;
        }
            

        // no valid transitions
        return null;
    }

}