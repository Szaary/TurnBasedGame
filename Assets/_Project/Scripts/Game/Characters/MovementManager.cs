using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;


public class MovementManager : MonoBehaviour
{
    [Header("Logic")] [SerializeField] private GameObject realtime;
    [SerializeField] private GameObject turnBased;


    [Header("Movement")] 
    public CharacterController controller;
    
    public FirstPersonController fps;
    public RelativeController relativeController;
    public AnimatorMovementController animatorController;
    public NavMeshAgentMovement navMeshAgentMovement;
    
    private GameManager _gameManager;
    private CharacterFacade _facade;

    [Header("Camera Logic")] public Transform cameraFpsFollowPoint;
    public Transform cameraFppFollowPoint;

    
    public void Initialize(CharacterFacade characterFacade)
    {
        _facade = characterFacade;
        _gameManager = _facade.gameManager;
        
        fps.Initialize(_facade);
        relativeController.Initialize(_facade);
        animatorController.Initialize(_facade);
        navMeshAgentMovement.Initialize(_facade);
    }


    private void Start()
    {
        OnGameModeChanged(_gameManager.GameMode);
        _gameManager.GameModeChanged += OnGameModeChanged;
    }

    private void OnDestroy()
    {
        _gameManager.GameModeChanged -= OnGameModeChanged;
    }


    private void OnGameModeChanged(GameMode gameMode)
    {
        if (gameMode == GameMode.InMenu)
        {
            TurnOffCharacterControl();
            SetTurnBasedLogic(false);
            SetRealtimeLogic(false);
            
        }
        if (gameMode == GameMode.TurnBasedFight)
        {
            SetTurnBasedLogic(true);
            SetRealtimeLogic(false);

            TurnOffCharacterControl();
            TurnOnNavMeshControl();
        }
        else if (gameMode == GameMode.Fps)
        {
            SetRealtimeLogic(true);
            TurnOffCharacterControl();
            
            if (_facade.GetRealTimeStrategy() is PlayerRealTimeStrategy)
            {
                controller.enabled = true;
                fps.enabled = true;
            }
            else
            {
                TurnOnNavMeshControl();
            }
        }
        else if (gameMode is GameMode.Adventure or GameMode.Tpp )
        {
            SetRealtimeLogic(true);
            TurnOffCharacterControl();
            
            if (_facade.GetRealTimeStrategy() is PlayerRealTimeStrategy)
            {
                controller.enabled = true;
                animatorController.enabled = true;
            }
            else
            {
                TurnOnNavMeshControl();
            }
        }
    }

    private void TurnOnNavMeshControl()
    {
        navMeshAgentMovement.enabled = true;
        
        controller.enabled = false;

        animatorController.enabled = false;
        fps.enabled = false;
        relativeController.enabled = false;
    }

    private void TurnOffCharacterControl()
    {
        controller.enabled = false;
        animatorController.enabled = false;
        fps.enabled = false;
        relativeController.enabled = false;
        navMeshAgentMovement.enabled = false;
    }


    private void SetTurnBasedLogic(bool isEnabled)
    {
        turnBased.SetActive(isEnabled);
    }

    private void SetRealtimeLogic(bool isEnabled)
    {
        realtime.SetActive(isEnabled);
    }
}