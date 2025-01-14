using System;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private CharacterFacade _facade;
    
    private void Awake()
    {
        _facade ??= GetComponent<CharacterFacade>();
    }

    public void TakeDamage(CharacterFacade shooter, List<Modifier> modifiers)
    {
        if (shooter.Alignment.IsAlly(_facade.Alignment)) return;
        _facade.Modify(shooter, modifiers);
    }

    private void OnValidate()
    {
        _facade ??= GetComponent<CharacterFacade>();
    }
}