using System;
using UnityEngine;
using Zenject;

public class CharacterFacade : MonoBehaviour
{
    public event Action<CharacterStatistics> StatisticSet;
    public event Action<CharacterFacade> DeSpawned;
    public void InvokeDeSpawnedCharacter() => DeSpawned?.Invoke(this);
 
    


    [SerializeField] private Character character;
    [SerializeField] private TurnController turnController;
    
    private PlayerTurn _playerTurn;
    private AiTurn _aiTurn;
    private CharacterStatistics _statistics;
    public int zoneIndex;

    public CharacterStatistics Statistics
    {
        get => _statistics;
        set
        {
            _statistics = value;
            StatisticSet?.Invoke(_statistics);
        }
    }

    public ActiveManager Active => character.active;
    
    public Alignment Alignment => character.Alignment;

    public Character GetCharacter() => character;
    public void SetCharacter(Character character)
    {
        this.character = character.Clone();
    }

    [Inject]
    public void Construct(PlayerTurn playerTurn, 
        AiTurn aiTurn)
    {
        _playerTurn = playerTurn;
        _aiTurn = aiTurn;
    }

    private void Start()
    {
        var arguments = new Character.InitializationArguments()
        {
            caller = this,
            playerTurn = _playerTurn,
            aiTurn = _aiTurn
        };
        
        Statistics = character.InitializeStats(arguments);
        turnController.InitializeStrategy(arguments, character);
    }


    private void OnDestroy()
    {
        Destroy(character);
    }


    public class Factory : PlaceholderFactory<CharacterFacade>
    {
    }
}