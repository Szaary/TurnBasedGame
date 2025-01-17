using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;


public class LogicController : MonoBehaviour
{
    [Header("Logic")] [SerializeField] private GameObject realtime;
    [SerializeField] private GameObject turnBased;
    [SerializeField] private GameObject weapons;
    [SerializeField] private AiGraphFacade graphFacade;


    [Header("Movement")] public CharacterController controller;

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
        _gameManager = _facade.GameManager;

        fps.Initialize(_facade);
        relativeController.Initialize(_facade);
        animatorController.Initialize(_facade);
        navMeshAgentMovement.Initialize(_facade);
        graphFacade.Initialize(_facade);
    }
 
    public void SetPosition(BaseSpawnZone.SpawnLocation playerPosition)
    {
        if (navMeshAgentMovement.enabled)
        {
            navMeshAgentMovement.RunToLocation(playerPosition.transform.position);
        }
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
        if (_facade.IsDead) return;
        
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
            weapons.SetActive(true);
            if (_facade.Alignment.IsPlayer)
            {
                controller.enabled = true;
                fps.enabled = true;
            }
            else
            {
                TurnOnNavMeshControl();
                graphFacade.gameObject.SetActive(true);
            }
        }
        else if (gameMode is GameMode.Adventure or GameMode.Tpp)
        {
            SetRealtimeLogic(true);
            TurnOffCharacterControl();

            if (_facade.Alignment.IsPlayer)
            {
                controller.enabled = true;
                animatorController.enabled = true;
            }
            else
            {
                TurnOnNavMeshControl();
                graphFacade.gameObject.SetActive(true);
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
        weapons.SetActive(false);
    }

    private void TurnOffCharacterControl()
    {
        graphFacade.gameObject.SetActive(false);
        controller.enabled = false;
        animatorController.enabled = false;
        fps.enabled = false;
        relativeController.enabled = false;
        navMeshAgentMovement.enabled = false;
        weapons.SetActive(false);
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