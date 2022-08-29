using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void ChangeActionPointsDelegate(int points = Character.MAXActionPoints);

public class TurnController : TurnsSubscriber, IDoActions
{
    private ChangeActionPointsDelegate _changeActionPointsDelegate;
    
    public int ActionPoints { get; private set; }
    private CharacterFacade _facade;

    public void Initialize(CharacterFacade character)
    {
        _changeActionPointsDelegate = ChangePointsPoints;

        _facade = character;
        if (character.Alignment.Id == 0)
            SubscribeToState(_facade.Turns.PlayerTurn);
        else
            SubscribeToState(_facade.Turns.AiTurn);
        
        SetInputStrategy(_facade);
    }
 
    private void SubscribeToState(BaseState state)
    {
        SubscribeToStateChanges(state);
    }

    private void SetInputStrategy(CharacterFacade character)
    {
        if (character.GetStrategy() is PlayerStrategy player)
        {
            _facade.turnBasedInput.playerStrategy = player;
            _facade.turnBasedInput.ActivateAction = Activate;
        }
    }

    public override Result OnEnter()
    {
        ActionPoints = _facade.GetActionPoints();

        _facade.GetStrategy().OnEnter(CreateFightState());

        return Result.Success;
    }


    public override Result Tick()
    {
        _facade.GetStrategy().Tick();
        return Result.Success;
    }


    public override Result OnExit()
    {
        if (_facade.GetStrategy() == null) return Result.StrategyNotSet;

        _facade.GetStrategy().OnExit(CreateFightState());
        return Result.Success;
    }


    private void Activate(Active active, List<CharacterFacade> targets, int chosenTarget)
    {
        var points = 0;
        if (active.IsMultiTarget())
        {
            foreach (var target in targets)
            {
                _facade.LookAt(target.transform);
                Debug.Log("Activating effect: " + active + " on " + target + " by " + _facade);
                points = active.ActivateEffect(target, _facade);
            }
        }
        else
        {
            var target = targets.First(x => x.Position == chosenTarget);
            _facade.LookAt(target.transform);
            Debug.Log("Activating effect: " + active + "on " + target);
            points = active.ActivateEffect(target, _facade);
        }
        ActionPoints -= points;
    }
    
    private Strategy.CurrentFightState CreateFightState()
    {
        return new Strategy.CurrentFightState()
        {
            Character = _facade,
            Points = ActionPoints,
            Library = _facade.Library,
            Inputs = _facade.turnBasedInput,
            ChangeActionPoints = _changeActionPointsDelegate
        };
    }
    
    private void ChangePointsPoints(int points)
    {
        ActionPoints -= points;
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromStates();
    }
}