using System.Collections.Generic;
using UnityEngine;

public interface IModifyStats
{
    public List<float> Amounts { get; set; }
    public BaseStatistic StatisticToModify { get; set; }
    public Modifier Modifier { get; set; }

    public bool OnApplyStatus(Character character, MonoBehaviour caller)
    {
        foreach (var statistic in character.battleStats.statistics)
        {
            if (StatisticToModify == statistic.baseStatistic)
            {
                Modifier.Modify(statistic, Amounts, caller);
            }
        }

        return true;
    }

    public bool OnRemoveStatus(Character character, MonoBehaviour caller)
    {
        foreach (var statistic in character.battleStats.statistics)
        {
            if (StatisticToModify == statistic.baseStatistic)
            {
                Modifier.UnModify(statistic, Amounts, caller);
            }
        }

        return true;
    }
}