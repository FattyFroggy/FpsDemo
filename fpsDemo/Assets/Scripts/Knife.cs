using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : WeaponBase
{
    [SerializeField] Knife_collider knife_Collider;

    public override void Init(Player_Controller player)
    {
        base.Init(player);
        knife_Collider.Init(this);
    }
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                if (isLeftAttack)
                {
                    OnLeftAttack();
                }
                else
                {
                    OnRightAttack();
                }
              
                break;
            case PlayerState.Reload:
                PlayAudio(2);
                animator.SetBool("Reload", true);
                break;
        }
    }


    public override void OnUpdatePlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Move:

                if (canShoot && Input.GetMouseButton(0))
                {
                    isLeftAttack = true;
                    player_Controller.ChangePlayerState(PlayerState.Shoot);
                    return;
                }
                if (canShoot && Input.GetMouseButton(1))
                {
                    isLeftAttack = false;
                    player_Controller.ChangePlayerState(PlayerState.Shoot);
                    return;
                }
                break;
        }
    }

    public void HitTarget(GameObject hitObj, Vector3 efPos)
    {
        PlayAudio(2);
        if (hitObj.CompareTag("Zombie"))
        {
            PlayAudio(2);
            // ¹¥»÷Ð§¹û
            GameObject go = Instantiate(prefab_BulletEF[1], efPos, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
            // ½©Ê¬Âß¼­
            ZombieController zombie = hitObj.GetComponent<ZombieController>();
            if (zombie == null) zombie = hitObj.gameObject.GetComponentInParent<ZombieController>();
            if (isLeftAttack)
            {
                zombie.Hurt(attackValue);
            }
            else
            {
                zombie.Hurt(attackValue * 4);
            }
        }
        else if (hitObj.gameObject != player_Controller.gameObject)
        {
            // ¹¥»÷Ð§¹û
            GameObject go = Instantiate(prefab_BulletEF[0], efPos, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }

    protected override void OnLeftAttack()
    {
        PlayAudio(1);
        animator.SetTrigger("Shoot");
        animator.SetBool("IsLeft", true);
        knife_Collider.StarHit();
    }
    private void OnRightAttack()
    {
        PlayAudio(1);
        animator.SetTrigger("Shoot");
        animator.SetBool("IsLeft", false);
        knife_Collider.StarHit();
    }
    protected override void ShootOver()
    {
        base.ShootOver();
        knife_Collider.StopHit();
    }
}
