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

                // ���ӵ�
                // ͨ��������ӵ���������
                // 1.�ӵ����� && �ӵ��������� &&�����б����ӵ�
                if (curr_BulletNum == 0 && curr_BulletNum < curr_MaxBulletNum && standby_BulletNum > 0)
                {
                    player_Controller.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                // 2.�ӵ�û���꣬����R����
                if (standby_BulletNum > 0 && curr_BulletNum < curr_MaxBulletNum && Input.GetKeyDown(KeyCode.R))
                {
                    player_Controller.ChangePlayerState(PlayerState.Reload);
                    return;
                }

                // ������
                if (canShoot && curr_BulletNum > 0 && Input.GetMouseButton(0))
                {
                    player_Controller.ChangePlayerState(PlayerState.Shoot);
                }
                break;
        }
    }
}
