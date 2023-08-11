using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : WeaponBase
{


    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                OnLeftAttack();
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

                // 换子弹
                // 通用情况：子弹不是满的
                // 1.子弹打完 && 子弹不是满的 &&但是有备用子弹
                if (curr_BulletNum == 0 && curr_BulletNum < curr_MaxBulletNum && standby_BulletNum > 0)
                {
                    player_Controller.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                // 2.子弹没打完，但是R键换
                if (standby_BulletNum > 0 && curr_BulletNum < curr_MaxBulletNum && Input.GetKeyDown(KeyCode.R))
                {
                    player_Controller.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                // 检测射击
                if (canShoot && curr_BulletNum > 0 && Input.GetMouseButton(0))
                {
                    player_Controller.ChangePlayerState(PlayerState.Shoot);
                }
                break;
        }
    }
}
