using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Algorithm : ScriptableObject 
{
    public abstract void Modify(Statistic stats, Modifier modifier, CharacterFacade user);
    public abstract void UnModify(Statistic stats, Modifier modifier, CharacterFacade user);
}
