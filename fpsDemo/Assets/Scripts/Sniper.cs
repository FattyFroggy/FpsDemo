using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : WeaponBase
{
    [SerializeField] GameObject SightCanvas;
    [SerializeField] GameObject[] Renders;
    private bool isAim = false;
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                if(isAim==true)
                {
                    StopAim();
                }
                OnLeftAttack();
                break;
            case PlayerState.Reload:
                if (isAim == true)
                {
                    StopAim();
                }
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


                //TODO:开镜
                if (canShoot && Input.GetMouseButtonDown(1))
                {
                    isAim = !isAim;
                    if (isAim)
                    {
                        StartAim();
                    }
                    else
                    {
                        StopAim();
                    }
                }
                break;
        }
    }
    private void StartAim()
    {
        //Debug.LogError("aim");
        animator.SetBool("Aim", true);
        //Debug.LogWarning(animator.parameters[3]);
        wantShootEF = false;


    }
    private void StopAim()
    {
        animator.SetBool("Aim", false);

        wantShootEF = true;

        for (int i = 0; i < Renders.Length; i++)
        {
            Renders[i].SetActive(true);
        }

        SightCanvas.SetActive(false);
        player_Controller.SetCameraView(60);
    }
    #region 动画事件
    private void StartLoad()
    {
        PlayAudio(3);
    }
    private void AimOver()
    {
        StartCoroutine(DoAim());
    }
    #endregion

    IEnumerator DoAim()
    {
        for (int i = 0; i < Renders.Length; i++)
        {
            Renders[i].SetActive(false);
        }
        yield return new WaitForSeconds(0.1f);
        SightCanvas.SetActive(true);
        player_Controller.SetCameraView(30);

    }
    private void ShootEnd()
    {
        canShoot = true;
        if (player_Controller.PlayerState == PlayerState.Shoot)
        {
            player_Controller.ChangePlayerState(PlayerState.Move);
        }
    }
}
