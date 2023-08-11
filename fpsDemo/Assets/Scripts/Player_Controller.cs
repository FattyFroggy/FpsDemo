using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;


public enum PlayerState
{
    Move,
    Shoot,
    Reload,
}
public class Player_Controller : MonoBehaviour
{
    public static Player_Controller Instance;
    private FirstPersonController firstPersonController;
    [SerializeField] Image crossImage;
    [SerializeField] WeaponBase[] weaponBases;
    [SerializeField] Camera[] cameras;
    private int currentWeaponIndex = -1;
    private int previousWeaponIndex = -1;
    private bool canChangeWeapon = true;
    private int hp = 100f;
    public PlayerState PlayerState;
    public void ChangePlayerState(PlayerState playerState)
    {
        this.PlayerState = playerState;
        weaponBases[currentWeaponIndex].OnEnterPlayerState(playerState);
    }
    private void Awake()
    {
        Instance = this;
        firstPersonController= this.gameObject.GetComponent<FirstPersonController>();
        for (int i = 0; i < weaponBases.Length; i++)
        {
            if (weaponBases[i] != null)
            {
                weaponBases[i].Init(this);
            }
           
        }
        PlayerState = PlayerState.Move;
        ChangeWeapon(0);

        //cameras = new Camera[2];
        //cameras[0] = this.gameObject.GetComponentInChildren<Camera>();
    }
    private void Update()
    {
        weaponBases[currentWeaponIndex].OnUpdatePlayerState(PlayerState);

        if (!canChangeWeapon) { return; }
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (previousWeaponIndex >= 0)
            {
                ChangeWeapon(previousWeaponIndex);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            StartShootRecoil();
           
        }
    }

    internal void UpdateBulletUI(int curr_BulletNum, int curr_MaxBulletNum, int standby_BulletNum)
    {
        UI_MainPanel.Instance.UpdateCurrBullet_Text(curr_BulletNum,curr_MaxBulletNum);
        UI_MainPanel.Instance.UpdateStandByBullet_Text(standby_BulletNum);
    }

    /// <summary>
    /// ºó×øÁ¦
    /// </summary>
    /// <param name="recoil"></param>
    public void StartShootRecoil(float recoil=1)
    {
        StartCoroutine(ShootRecoil_Croos(recoil));
        if(shootRecoil_Camera != null)
        {
            StopCoroutine(shootRecoil_Camera);
        }
        shootRecoil_Camera=StartCoroutine(ShootRecoil_Camera(recoil));
    }

    internal void Hurt(int damage)
    {
        if (hp < 0)
        {
            hp = 0;
        }
        UI_MainPanel.Instance.UpdateHP_Text(hp);
    }

    IEnumerator ShootRecoil_Croos(float recoil)
    {
        Vector2 scale = crossImage.transform.localScale;
        if (scale.x < 1.3f)
        {
            yield return null;
            scale.x += Time.deltaTime * 3 *recoil;
            scale.y += Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        while (scale.x > 1f)
        {
            yield return null;
            scale.x -= Time.deltaTime * 3 * recoil;
            scale.y -= Time.deltaTime * 3 * recoil;
            crossImage.transform.localScale = scale;
        }
        crossImage.transform.localScale = Vector2.one;
    }

    private Coroutine shootRecoil_Camera;
    IEnumerator ShootRecoil_Camera(float recoil)
    { 
     
        float xOffset = Random.Range(0.3f, 0.6f) * recoil;
        float yOffset = Random.Range(-0.15f, 0.15f) * recoil;
        firstPersonController.xRotOffset = xOffset;
        firstPersonController.yRotOffset = yOffset;

        for(int i = 0; i < 6; ++i)
        {
            yield return null;
        }
        firstPersonController.xRotOffset = 0;
        firstPersonController.yRotOffset = 0;

    }

    private void ChangeWeapon(int index)
    {
        if (currentWeaponIndex==index)
        {
            return;
        }
        previousWeaponIndex = currentWeaponIndex;
        currentWeaponIndex = index;
        if (previousWeaponIndex < 0)
        {
            weaponBases[currentWeaponIndex].Enter();
        }
        else
        {
            weaponBases[previousWeaponIndex].Exit(OnWeaponExitOver);
            canChangeWeapon = false;
        }
    }
    private void OnWeaponExitOver()
    {
        canChangeWeapon = true;
        weaponBases[currentWeaponIndex].Enter();
    }
    public void InitForEnterWeapon(bool wantCrossair,bool wantBullet)
    {
        crossImage.gameObject.SetActive(wantCrossair);
        UI_MainPanel.Instance.InitForEnterWeapon(wantBullet);
    }

    public void SetCameraView(int value)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].fieldOfView = value;
        }
    }
}
