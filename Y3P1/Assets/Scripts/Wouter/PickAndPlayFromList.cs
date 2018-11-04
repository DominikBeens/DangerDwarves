using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAndPlayFromList : MonoBehaviour {

    //public string listName;
    public float cooldownEffect;
    public bool luckOfDraw;
    public bool enableScript;
    public List<AudioSource> SFXSwingSounds = new List<AudioSource>();

    public List<AudioSource> SFXFindingEquipment = new List<AudioSource>();
    public List<AudioSource> SFXGettingDowned = new List<AudioSource>();
    public List<AudioSource> SFXGettingHit = new List<AudioSource>();
    public List<AudioSource> SFXGettingMoney = new List<AudioSource>();
    public List<AudioSource> SFXGettingRevived = new List<AudioSource>();
    public List<AudioSource> SFXOpeningArea = new List<AudioSource>();
    public List<AudioSource> SFXRandomInCaves = new List<AudioSource>();
    public List<AudioSource> SFXSlayingEnemy = new List<AudioSource>();
    public List<AudioSource> SFXUsingAbility = new List<AudioSource>();

    private void Start()
    {
        PlaySFXOpeningArea(5);

    }

    private void Update()
    {

        if(cooldownEffect > 0)
        {
            cooldownEffect -= Time.deltaTime;
        }
    }

    public void DrawLuck(float chance)
    {
        if(Random.Range(0, 100) < chance)
        {
            luckOfDraw = true;
        }
        else
        {
            luckOfDraw = false;
        }
    }

    public void PlaySFXSwingSoundFromList()
    {
        SFXSwingSounds[Random.Range(0, SFXSwingSounds.Count)].Play();
    }

    public void PlaySFXFindingEquipment(float cooldown)
    {
        DrawLuck(50);
        if(cooldownEffect <= 0 && luckOfDraw)
        {
            SFXFindingEquipment[Random.Range(0, SFXFindingEquipment.Count)].Play();
            cooldownEffect = cooldown;
        }
        
    }

    public void PlaySFXGettingDowned(float cooldown)
    {
        DrawLuck(50);
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXGettingDowned[Random.Range(0, SFXGettingDowned.Count)].Play();
            cooldownEffect = cooldown;
        }
    }

    public void PlaySFXGettingHit(float cooldown)
    {
        DrawLuck(100);
        print("ooof");
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXGettingHit[Random.Range(0, SFXGettingHit.Count)].Play();
            cooldownEffect = cooldown;
        }
    }

    public void PlaySFXGettingMoney(float cooldown)
    {
        DrawLuck(50);
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXGettingMoney[Random.Range(0, SFXGettingMoney.Count)].Play();
            cooldownEffect = cooldown;
        }
    }

    public void PlaySFXGettingRevived(float cooldown)
    {
        DrawLuck(50);
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXGettingRevived[Random.Range(0, SFXGettingRevived.Count)].Play();
            cooldownEffect = cooldown;
        }
    }

    public void PlaySFXOpeningArea(float cooldown)
    {
        DrawLuck(50);
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXOpeningArea[Random.Range(0, SFXOpeningArea.Count)].Play();
            cooldownEffect = cooldown;
        }
    }

    public void PlaySFXRandomInCaves(float cooldown)
    {
        DrawLuck(50);
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXRandomInCaves[Random.Range(0, SFXRandomInCaves.Count)].Play();
            cooldownEffect = cooldown;
        }
    }

    public void PlaySFXSlayingEnemy(float cooldown)
    {
        DrawLuck(50);
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXSlayingEnemy[Random.Range(0, SFXSlayingEnemy.Count)].Play();
            cooldownEffect = cooldown;
        }
    }

    public void PlaySFXUsingAbility(float cooldown)
    {
        DrawLuck(50);
        if (cooldownEffect <= 0 && luckOfDraw)
        {
            SFXUsingAbility[Random.Range(0, SFXUsingAbility.Count)].Play();
            cooldownEffect = cooldown; ;
        }
    }
}
