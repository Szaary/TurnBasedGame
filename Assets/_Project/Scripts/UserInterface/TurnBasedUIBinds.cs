using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class TurnBasedUIBinds : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    [SerializeField] private float messageTime = 3;

    [SerializeField] private List<BindRepresentation> binds;

    private TurnBasedInput _input;
    private GameManager _gameManager;

    [Inject]
    public void Construct(TurnBasedInput input, GameManager gameManager)
    {
        _input = input;
        _gameManager = gameManager;
    }

    private void Start()
    {
        _input.WrongWSADPressed += HandleWrongInputs;
        _input.InputsPopulated += OnInputsPopulated;
        _input.ActionSelected += OnActionSelected;
        _input.ActionActivated += OnActionActivated;
        
        foreach (var bind in binds)
        {
            bind.Initialize();
        }
        
        OnGameModeChanged(_gameManager.GameMode);
        _gameManager.GameModeChanged += OnGameModeChanged;
    }

    private void OnDestroy()
    {
        _gameManager.GameModeChanged -= OnGameModeChanged;
        
        _input.WrongWSADPressed -= HandleWrongInputs;
        _input.InputsPopulated -= OnInputsPopulated;
        _input.ActionSelected -= OnActionSelected;
        _input.ActionActivated -= OnActionActivated;
    }

    private void OnGameModeChanged(GameMode obj)
    {
        foreach (var bind in binds)
        {
            bind.ChangePositions(obj);
        }
    }
    
    
    private void OnActionActivated()
    {
        HideBinds();
    }

    private void HideBinds()
    {
        foreach (var bind in binds)
        {
            bind.HideButton();
        }
    }

    private void OnActionSelected(Active skill, List<CharacterFacade> possibleTargets)
    {
        HideBinds();
        for (var index = 0; index < binds.Count; index++)
        {
            var bind = binds[index];
            if (possibleTargets.Count <= index)
            {
                bind.HideButton();
                continue;
            }
            if (possibleTargets.Count(x => x.Position == index) > 0)
            {
                bind.ShowButton(possibleTargets[index]);
            }
            else
            {
                bind.HideButton();
            }
        }
    }

    private void OnInputsPopulated(List<Active> actives, List<CharacterFacade> targets, CharacterFacade user,
        int comboPoints)
    {
        HideBinds();
        for (var index = 0; index < binds.Count; index++)
        {
            var bind = binds[index];
            if (actives.Count <= index)
            {
                bind.HideButton();
                continue;
            }
            if (actives.Count(x => x.IndexOnBar == index) > 0)
            {
                bind.ShowButton(actives[index]);
            }
            else
            {
                bind.HideButton();
            }
        }
    }

    private void HandleWrongInputs(Result result)
    {
        if (result == Result.AiTurn)
        {
            ShowUiMessage("Actions can not be taken in ai Turn.");
        }

        if (result == Result.NoSkillAvailable)
        {
            ShowUiMessage("Skill is currently not available.");
        }

        if (result == Result.NoTarget)
        {
            ShowUiMessage("Can not find target for skill.");
        }
        if (result == Result.NoSuitableSkillsToUse)
        {
            ShowUiMessage("Action is not possible to take.");
        }
    }


    private void ShowUiMessage(string resultAsText)
    {
        StartCoroutine(PresentMessage(resultAsText));
    }

    private IEnumerator PresentMessage(string resultAsText)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = resultAsText;
        yield return new WaitForSeconds(messageTime);
        resultText.gameObject.SetActive(false);
    }
}