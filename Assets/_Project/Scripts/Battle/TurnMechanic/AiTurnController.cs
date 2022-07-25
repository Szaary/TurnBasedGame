using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AiTurnController : MonoBehaviour, ISubscribeToBattleStateChanged, IDoActions
{
    private Settings _settings;
    public int CurrentActions { get; private set; }

    public BaseState BaseState { get; private set; }

    [Inject]
    public void Construct(Settings settings, AiTurn aiTurn)
    {
        BaseState = aiTurn;
        _settings = settings;
    }

    private void OnEnable()
    {
        ((ISubscribeToBattleStateChanged)this).SubscribeToStateChanges();
    }

    private void OnDisable()
    {
        ((ISubscribeToBattleStateChanged)this).UnsubscribeFromStateChanges();
    }

    public Task Tick()
    {
        return Task.CompletedTask;
    }

    public async Task OnEnter()
    {
        CurrentActions = _settings.maxNumberOfActions;

        await DoAction();
    }

    private async Task DoAction()
    {
        Debug.Log("Before doing action");
        await Task.Delay(1000);

        CurrentActions = 0;
        Debug.Log("After doing action");
    }

    public Task OnExit()
    {
        return Task.CompletedTask;
    }


    [Serializable]
    public class Settings
    {
        public int maxNumberOfActions;
    }
}