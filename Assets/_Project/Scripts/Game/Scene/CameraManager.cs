using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Zenject;

public class CameraManager : MonoBehaviour
{
    private const int MaxCameraPriority = 30; 
    private const int UsualCameraPriority = 15; 

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera uiCamera;

    [SerializeField] private List<Settings> cameras;

    [SerializeField] private CinemachineVirtualCamera fpsCamera;
    [SerializeField] private CinemachineVirtualCamera turnBasedCamera;

    public Camera MainCamera => mainCamera;
    public CinemachineVirtualCamera FpsCamera => fpsCamera;

    private GameManager _gameManager;
    private CharactersLibrary _library;
    
    
    [Inject]
    public void Construct(GameManager gameManager, CharactersLibrary library)
    {
        _gameManager = gameManager;
        _library = library;
    }

    private void Start()
    {
        OnGameModeChanged(_gameManager.GameMode);
        _gameManager.GameModeChanged += OnGameModeChanged;
    }

    private void OnGameModeChanged(GameMode newMode)
    {
        if (newMode == GameMode.TurnBasedFight)
        {
            SetTurnBasedCamera();
        }
        else if (newMode == GameMode.Fps)
        {
            SetFpsCamera();
        }
    }

    private void OnDestroy()
    {
        _gameManager.GameModeChanged -= OnGameModeChanged;
    }

    public void SetFpsCamera()
    {
        var result = _library.GetControlledCharacter(out CharacterFacade facade);
        if (result != Result.Success)
        {
            Debug.LogError(typeof(CameraManager) + " " + result);
            return;
        }

        var cameraFpsFollowPoint = facade.cameraFpsFollowPoint;
        
        foreach (var cam in cameras)
        {
            if (cam.camera == fpsCamera)
            {
                cam.camera.Priority = MaxCameraPriority;
                SetupCameraAfterTarget(cam.camera, cameraFpsFollowPoint);
            }
            else
            {
                cam.camera.Priority = UsualCameraPriority;
            }
        }
    }
    
    public void SetTurnBasedCamera()
    {
        foreach (var cam in cameras)
        {
            if (cam.camera == turnBasedCamera)
            {
                cam.camera.Priority = MaxCameraPriority;
            }
            else
            {
                cam.camera.Priority = UsualCameraPriority;
            }
        }
    }
    
    
    private void SetupCameraAfterTarget(CinemachineVirtualCamera cam, Transform cameraTarget)
    {
        cam.Follow = cameraTarget;
    }

    [Serializable]
    public struct Settings
    {
        public CinemachineVirtualCamera camera;
    }
}