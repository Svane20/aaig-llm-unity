using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    private AudioSource component;
    private AudioClip hurt;
    private AudioClip attack;
    private AudioClip heal;
    private AudioClip magic;
    private AudioClip explosion;
    private AudioClip teleport;
    private AudioClip animepunch;
    private AudioClip enemydies;

    private AudioClip shield;

    private AudioClip pew;

    private AudioClip dead;

    private void Start()
    {
        component = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        hurt = Resources.Load<AudioClip>("Sounds/Hurt");
        heal = Resources.Load<AudioClip>("Sounds/Heal");
        attack = Resources.Load<AudioClip>("Sounds/Attack");
        magic = Resources.Load<AudioClip>("Sounds/Magic");
        explosion = Resources.Load<AudioClip>("Sounds/Explosion");
        teleport = Resources.Load<AudioClip>("Sounds/Teleport");
        animepunch = Resources.Load<AudioClip>("Sounds/strongpunch");
        shield = Resources.Load<AudioClip>("Sounds/Shield");
        enemydies = Resources.Load<AudioClip>("Sounds/EnemyDies");
        pew = Resources.Load<AudioClip>("Sounds/Pew");
        dead = Resources.Load<AudioClip>("Sounds/Dead");

    }

    public void PlayHurt()
    {
        component.clip = hurt;
        component.Play();
    }

    public void PlayDead()
    {
        component.clip = dead;
        component.Play();
    }

    public void PlayHeal()
    {
        component.clip = heal;
        component.Play();
    }
    
    public void PlayAttack()
    {
        component.clip = attack;
        component.Play();
    }

    public void PlayMagic()
    {component.clip = magic;
        component.Play();
    }

    public void PlayExplosion()
    {
        component.clip = explosion;
        component.Play();
    }

    public void PlayTeleport()
    {
        component.clip = teleport;
        component.Play();
    }

    public void PlayAnimePunch()
    {
        component.clip = animepunch;
        component.Play();
    }

    public void PlayShield()
    {
        component.clip = shield;
        component.Play();
    }

    public void PlayEnemydies()
    {
        component.clip = enemydies;
        component.Play();
    }

    public void PlayPew()
    {
        component.clip = pew;
        component.loop = true;
        component.Play();
    }
    
}
