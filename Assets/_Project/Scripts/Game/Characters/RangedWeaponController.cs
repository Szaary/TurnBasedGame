﻿using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class RangedWeaponController : WeaponController, ICharacterSystem
{
    public event Action<int, int> magazineChanged;
    public RangedWeapon weapon;
    [SerializeField] private CinemachineImpulseSource source;
    public int Magazine { get; private set; }

    public float TimeBetweenAttacks => weapon.timeBetweenAttacks;
    public float Range => weapon.range;

    private void Start()
    {
        Magazine = weapon.MagazineSize;
    }

    private void Update()
    {
        if (weapon == null) return;
        var delta = Facade.TimeManager.GetDeltaTime(this);
        if (Input.shoot && AttackTimeout >= TimeBetweenAttacks && Magazine > 0)
        {
            AttackTimeout = 0;

            FireWeapon();
            magazineChanged?.Invoke(Magazine, weapon.MagazineSize);
            source.GenerateImpulse();
            if (Magazine == 0)
            {
                StartCoroutine(ReloadWeapon());
            }
        }

        if (AttackTimeout < TimeBetweenAttacks)
        {
            AttackTimeout += delta;
        }
    }

    private IEnumerator ReloadWeapon()
    {
        yield return new WaitForSeconds(weapon.reloadTime);
        Magazine = weapon.MagazineSize;
        magazineChanged?.Invoke(Magazine, weapon.MagazineSize);
    }


    public void FireWeapon()
    {
        Magazine--;
        var position = MainCamera.transform.position + MainCamera.transform.forward;
        var direction = MainCamera.transform.forward;
        
        var newProjectile = Pool.Add(position, direction, Facade, weapon.Modifiers);

        newProjectile.StartCoroutine(DestroyAfterTime(newProjectile));
        PlaySfx();
    }

    public void FireWeaponForward()
    {
        var newProjectile = Pool.Add(transform.position, transform.forward, Facade, weapon.Modifiers);
        
        newProjectile.StartCoroutine(DestroyAfterTime(newProjectile));
        PlaySfx();
    }

    private bool attacked;

    // USED BY GRAPH 
    public void FireWeaponWithCooldown()
    {
        if (!attacked)
        {
            attacked = true;
            var projectile = Pool.Add(transform.position, transform.forward, Facade, weapon.Modifiers);
            
            projectile.StartCoroutine(DestroyAfterTime(projectile));
            
            Invoke(nameof(Reset), TimeBetweenAttacks);
            PlaySfx();
        }
    }

    public void Reset()
    {
        attacked = false;
    }


    private IEnumerator DestroyAfterTime(Projectile projectile)
    {
        yield return new WaitForSeconds(3);
        Pool.Remove(projectile);
    }

    public override void PlaySfx()
    {
        soundEmitter.Play(weapon.eventReference);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (weapon == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * weapon.range);
    }
#endif

    public override void Disable()
    {
        enabled = false;
    }
}