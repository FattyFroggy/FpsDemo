using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;

    protected Player_Controller player_Controller;
    protected int curr_BulletNum;
    public int curr_MaxBulletNum;
    protected int standby_BulletNum;
    public int standby_MaxBulletNum;


    public bool wantBullet;
    public int attackValue;
    public bool wantCrossair;
    public bool wantShootEF;
    public bool wantRecoil;
    public float recoilStrength;
    public bool canThroughWall;





    protected bool isLeftAttack = true;
    protected bool canShoot=false;
    private bool wantReloadOnEnter = false;
    #region 效果
    public AudioClip[] audioClips;
    [SerializeField] protected GameObject shootEF;
    [SerializeField] protected GameObject[] prefab_BulletEF;
    #endregion
    public virtual void Init(Player_Controller player)
    {
        this.player_Controller = player;
        curr_BulletNum = curr_MaxBulletNum;
        standby_BulletNum = standby_MaxBulletNum;

    }
    public abstract void OnEnterPlayerState(PlayerState playerState);
    public abstract void OnUpdatePlayerState(PlayerState playerState);
    public virtual void Enter()
    {
        canShoot = false;
        player_Controller.InitForEnterWeapon(wantCrossair, wantBullet);
        if (wantBullet)
        {
            player_Controller.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
            if (curr_BulletNum > 0)
            {
                PlayAudio(0);
            }
            else
            {
                //:换子弹
                wantReloadOnEnter = true;
            }
        }

        if (shootEF != null)
        {
            shootEF.SetActive(false);
        }
        gameObject.SetActive(true);
    }
    private Action onExitOver;
    public virtual void Exit(Action onExitOver)
    {
        animator.SetTrigger("Exit");
        this.onExitOver = onExitOver;
        player_Controller.PlayerState = PlayerState.Move;
    }
    #region 动画事件
    private void EnterOver()
    {
        canShoot = true;
        gameObject.SetActive(true);
        if (wantReloadOnEnter)
        {
            player_Controller.ChangePlayerState(PlayerState.Move);
        }
    }
    private void ExitOver()
    {
        gameObject.SetActive(false);
        onExitOver?.Invoke();
        
    }
    #endregion
    protected virtual void OnLeftAttack()
    {
        if (wantBullet)
        {
            curr_BulletNum--;
            player_Controller.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        }
        canShoot = false;
        animator.SetTrigger("Shoot");
        if (wantShootEF)
        {
            shootEF.gameObject.SetActive(true);

        }
        if (wantRecoil)
        {
            player_Controller.StartShootRecoil(recoilStrength);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (canThroughWall)
        {
            // 穿墙
            RaycastHit[] hitInfos = Physics.RaycastAll(ray, 1500f);
            for (int i = 0; i < hitInfos.Length; i++)
            {
                HitGameObject(hitInfos[i]);
            }
        }
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1500f))
            {
                HitGameObject(hitInfo);
            }
        }

        PlayAudio(1);



    }

    private void HitGameObject(RaycastHit hitInfo)
    {
        //
        if (hitInfo.collider.gameObject.CompareTag("Zombie"))
        {
            GameObject go = Instantiate(prefab_BulletEF[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);

            ZombieController zombie = hitInfo.collider.gameObject.GetComponent<ZombieController>();
            if (zombie == null)
            {
                zombie = hitInfo.collider.gameObject.GetComponentInParent<ZombieController>();
            }
            zombie.Hurt(attackValue);
        }
        else if (hitInfo.collider.gameObject != player_Controller.gameObject)
        {
            GameObject go = Instantiate(prefab_BulletEF[0], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }


    protected void PlayAudio(int index)
    {
        audioSource.PlayOneShot(audioClips[index]); ;
    }
    protected virtual void ShootOver()
    {
        Debug.LogWarning("shootOver");
        canShoot = true;
        if (wantShootEF) shootEF.SetActive(false);
        if (player_Controller.PlayerState == PlayerState.Shoot)
        {
            player_Controller.ChangePlayerState(PlayerState.Move);
        }
    }

    private void ReloadOver()
    {
        // 填充子弹
        // 子弹不够、子弹足够
        int want = curr_MaxBulletNum - curr_BulletNum;
        if ((standby_BulletNum - want) < 0)
        {
            want = standby_BulletNum;
        }
        standby_BulletNum -= want;
        curr_BulletNum += want;

        // 更新UI
        player_Controller.UpdateBulletUI(curr_BulletNum, curr_MaxBulletNum, standby_BulletNum);
        animator.SetBool("Reload", false);
        player_Controller.ChangePlayerState(PlayerState.Move);
    }
}
