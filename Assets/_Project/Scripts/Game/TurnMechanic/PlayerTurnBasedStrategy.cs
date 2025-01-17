using UnityEngine;

//[CreateAssetMenu(fileName = "AIS_", menuName = "Strategy/PlayerStrategy")]
public class PlayerTurnBasedStrategy : TurnBasedStrategy
{
    public override Result SelectTactic(CurrentFightState currentFightState, out SelectedStrategy selectedStrategy)
    {
        selectedStrategy = new SelectedStrategy();
        
        currentFightState.Character.GainControl();
        currentFightState.TurnBasedInputManager.ResetInputs();
        Debug.Log("Entered Player turn, selecting move.");
        var result = TacticsLibrary.GetPossibleActions(currentFightState, out var active, out var targets);
        if (result is TacticsLibrary.Possible.Both or TacticsLibrary.Possible.OnlyDefensive
            or TacticsLibrary.Possible.OnlyOffensive)
        {
            currentFightState.TurnBasedInputManager.PopulateCurrentState(active, targets, currentFightState.Character,
                currentFightState.Points);
            return Result.Success;
        }
        Debug.Log(typeof(PlayerTurnBasedStrategy) + " " + result);
        return Result.NoSuitableSkillsToUse;
    }
}