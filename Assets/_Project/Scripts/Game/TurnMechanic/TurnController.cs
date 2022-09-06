using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : TurnsSubscriber, IDoActions, ICharacterSystem
{
    public int ActionPoints { get; private set; }

    private CharacterFacade _facade;
    public TurnBasedStrategy.SelectedStrategy SelectedStrategy = new();
    private AnimatorManager _animator;


    public void Initialize(CharacterFacade character)
    {
        _facade = character;
        if (character.Alignment.IsPlayerAlly)
        {
            SubscribeToState(_facade.Turns.PlayerTurn);
            Debug.Log("Subscribe to player turn by: "+ character.name);
        }
        else
            SubscribeToState(_facade.Turns.AiTurn);
        
        _facade.CharacterSystems.Add(this);
        
        _animator = character.animatorManager;
        _animator.animationEnded += OnAnimationEnded;
        _animator.animationWorked += OnAnimationWorked;
    }

    private void OnAnimationWorked()
    {
        SelectedStrategy.selectedSkill.ActivateEffect(SelectedStrategy.selectTargets, SelectedStrategy.character);
        
    }

    private void OnAnimationEnded()
    {
        ActionPoints -= SelectedStrategy.selectedSkill.cost;
        
        SelectedStrategy = new TurnBasedStrategy.SelectedStrategy();
    }

    private void SubscribeToState(BaseState state)
    {
        SubscribeToStateChanges(state);
    }

    public override Result Tick()
    {
        return Result.Success;
    }

    public override Result OnEnter()
    {
        ActionPoints = _facade.GetActionPoints();

        var unitStrategy = _facade.GetStrategy();
        var result = unitStrategy.SelectTactic(CreateFightState(), out var selectedStrategy);
        if (result != Result.Success)
        {
            Debug.LogError(typeof(TurnController) + " " + result);
        }

        if (!_facade.Alignment.IsPlayer)
        {
            StartCoroutine(WaitForAttack(selectedStrategy));
        }

        return Result.Success;
    }

    private IEnumerator WaitForAttack(TurnBasedStrategy.SelectedStrategy selectedStrategy)
    {
        var random = Random.Range(0, 0.7f);
        yield return new WaitForSeconds(random);
        SelectStrategy(selectedStrategy);
    }


    public void SelectStrategy(TurnBasedStrategy.SelectedStrategy strategy)
    {
        SelectedStrategy = strategy;
        if (strategy.selectedSkill.IsAttack()) _animator.Attack();
        else if (strategy.selectedSkill.IsDefensive()) _animator.Defensive();
    }


    public override Result OnExit()
    {
        return Result.Success;
    }

    private TurnBasedStrategy.CurrentFightState CreateFightState()
    {
        return new TurnBasedStrategy.CurrentFightState()
        {
            Character = _facade,
            Library = _facade.Library,
            Points = ActionPoints,
            TurnBasedInputManager = _facade.BasedInputManager
        };
    }


    private void OnDestroy()
    {
        UnsubscribeFromStates();
        _animator.animationEnded -= OnAnimationEnded;
        _animator.animationWorked -= OnAnimationWorked;
    }

    public void Disable()
    {
        UnsubscribeFromStates();
        enabled = false;
    }
}