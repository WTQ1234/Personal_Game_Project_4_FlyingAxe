using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HRL;
using Sirenix.OdinInspector;

public class PlayerHealth : MobAttrBase
{
    public int maxHealth;
    public int blinks_num;
    public float blinks_time;
    public float dieTime;

    public bool avoidHit;
    public float hitCdTime;

    public UI_Comp_HealthBar comp_HealthBar;

    public SpriteRenderer myRender;
    private Animator anim;
    private Rigidbody2D rb2d;

    public Color red_color;

    public bool isDied
    {
        get
        {
            return current_value <= 0;
        }
    }

    void Start()
    {
        //BuffManager.Instance.AddBuffByCfgId(gameObject, 0);
        if (current_value <= 1)
        {
            current_value = 1;
        }
        if (maxHealth <= current_value)
        {
            maxHealth = current_value;
        }
        if (comp_HealthBar != null)
        {
            comp_HealthBar.SetMaxHealth(maxHealth);
            comp_HealthBar.SetCurrentHealth(current_value);
        }

        avoidHit = false;
        if (myRender == null)
        {
            myRender = GetComponent<SpriteRenderer>();
        }
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void SetMaxHp(int _maxHp)
    {
        maxHealth = _maxHp;
        _OnMaxHpChange();
    }

    public void AddMaxHp(int _addMaxHp)
    {
        maxHealth += _addMaxHp;
        _OnMaxHpChange();
    }

    private void _OnMaxHpChange()
    {
        if (comp_HealthBar != null)
        {
            comp_HealthBar.SetMaxHealth(maxHealth);
        }
    }

    [Button("Damage")]
    public void Damage(int damage, bool knock_back = false, Entity trans_damage_from = null)
    {
        if (avoidHit)
        {
            return;
        }
        if (current_value <= 0)
        {
            return;
        }
        //sf.FlashScreen();
        current_value -= damage;
        if(current_value < 0)
        {
            current_value = 0;
        }
        if (comp_HealthBar != null)
        {
            comp_HealthBar.SetCurrentHealth(current_value);
        }
        if (current_value <= 0)
        {
            KillPlayer();
        }
        else
        {
            BlinkPlayer(blinks_num, blinks_time);
            avoidHit = true;
            StartCoroutine(ShowPlayerHitBox());
        }
    }

    IEnumerator ShowPlayerHitBox()
    {
        yield return new WaitForSeconds(hitCdTime);
        avoidHit = false;
    }

    void KillPlayer()
    {
        avoidHit = true;
        //rb2d.velocity = new Vector2(0, 0);
        //rb2d.gravityScale = 0.0f;
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        Owner.Killed();
        //Invoke("AfterDieAnim", dieTime);
        //Messenger.Instance.BroadcastMsg("CombatOver", Owner.teamController.teamId);
    }

    public void RebornPlayer()
    {
        anim.ResetTrigger("Die");
        anim.Play("Idle");
        current_value = maxHealth;
        avoidHit = false;
        comp_HealthBar?.SetCurrentHealth(current_value);
    }

    public void AfterDieAnim()
    {
        Destroy(gameObject);
    }

    void BlinkPlayer(int numBlinks, float seconds)
    {
        StartCoroutine(DoBlinks(numBlinks, seconds));
    }

    IEnumerator DoBlinks(int numBlinks, float seconds)
    {
        for(int i = 0; i < numBlinks; i++)
        {
            myRender.color = red_color;
            //myRender.enabled = !myRender.enabled;
            yield return new WaitForSeconds(seconds);
            myRender.color = Color.white;
            yield return new WaitForSeconds(seconds);
        }
        myRender.color = Color.white;
        //myRender.enabled = true;
    }
}
