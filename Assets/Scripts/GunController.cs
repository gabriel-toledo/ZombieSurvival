using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GunController : MonoBehaviour
{
    [Header("Assault")]
    public int assaultDamage;
    public float assaultTimeBetweenShooting, assaultSpread, assaultAimSpread, assaultMovingSpread, assaultRange, assaultReloadTime, assaultTimeBetweenShots;
    public int assaultMagazineSize, assaultBulletsPerTap;
    public bool assaultAllowButtonHold;

    [Header("Sniper")]
    public int sniperDamage;
    public float sniperTimeBetweenShooting, sniperSpread, sniperAimSpread, sniperMovingSpread, sniperRange, sniperReloadTime, sniperTimeBetweenShots;
    public int sniperMagazineSize;

    [Header("Pistol")]
    public int pistolDamage;
    public float pistolTimeBetweenShooting, pistolSpread, pistolAimSpread, pistolMovingSpread, pistolRange, pistolReloadTime, pistolTimeBetweenShots;
    public int pistolMagazineSize;

    [Header("Shotgun")]
    public int shotgunDamage;
    public float shotgunTimeBetweenShooting, shotgunSpread, shotgunAimSpread, shotgunMovingSpread, shotgunRange, shotgunReloadTime, shotgunTimeBetweenShots;
    public int shotgunMagazineSize, shotgunBulletsPerTap;

    //bools 
    bool shooting, reloading, aiming, moving;

    [Header("Reference")]
    public Camera fpsCam;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy; 
    public AudioSource audioSource; 
    public AudioSource reload;
    public GameObject assaultPrefab;
    public GameObject sniperPrefab;
    public GameObject pistolPrefab;
    public GameObject shotgunPrefab;

    [Header("Graphics")]
    public GameObject enviromentBulletHoleGraphic, enemyBulletHoleGraphic;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI ammunition;

    private Gun assaultRifle;
    private Gun sniper;
    private Gun pistol;
    private Gun shotgun;

    public Gun currentGun;
    public enum GunTypes
    {
        assault,
        sniper,
        pistol,
        shotgun
    };

    public struct Gun
    {
        public GunTypes type;
        public bool readyToShoot;
        public int damage;
        public float timeBetweenShooting, spread, aimSpread, movingSpread, range, timeBetweenShots, reloadTime;
        public int magazineSize, bulletsPerTap;
        public bool allowButtonHold;
        public int bulletsLeft, bulletsShot;
        public GameObject gun;
        public Transform attackPoint;
        public ParticleSystem muzzleFlash, cartridgeEjec;

        public Gun(GunTypes type, GameObject gunPrefab, int damage, int magazineSize, int bulletsPerTap, float timeBetweenShooting, float spread, float aimSpread, float movingSpread, float range, float reloadTime, float timeBetweenShots, bool allowButtonHold, Transform gunTransform)
        {
            this.type = type;
            this.damage = damage;
            this.magazineSize = magazineSize * bulletsPerTap;
            this.bulletsPerTap = bulletsPerTap;
            this.timeBetweenShooting = timeBetweenShooting;
            this.spread = spread;
            this.aimSpread = aimSpread;
            this.movingSpread = movingSpread;
            this.range = range;
            this.reloadTime = reloadTime;
            this.timeBetweenShots = timeBetweenShots;
            this.allowButtonHold = allowButtonHold;
            this.gun = Instantiate(gunPrefab, gunTransform);
            this.gun.SetActive(false);
            this.bulletsLeft = this.magazineSize;
            this.bulletsShot = 0;
            this.readyToShoot = false;
            this.attackPoint = this.gun.transform.GetChild(0).transform;
            this.muzzleFlash = this.gun.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
            this.cartridgeEjec = this.gun.transform.GetChild(1).GetChild(0).transform.GetComponent<ParticleSystem>();
        }
    }

    private void Awake()
    {
        assaultRifle = new Gun(GunTypes.assault, assaultPrefab, assaultDamage, assaultMagazineSize, assaultBulletsPerTap, assaultTimeBetweenShooting, assaultSpread, assaultAimSpread, assaultMovingSpread, assaultRange, assaultReloadTime, assaultTimeBetweenShots, assaultAllowButtonHold, transform);
        sniper = new Gun(GunTypes.sniper, sniperPrefab, sniperDamage, sniperMagazineSize, 1, sniperTimeBetweenShooting, sniperSpread, sniperAimSpread, sniperMovingSpread, sniperRange, sniperReloadTime, sniperTimeBetweenShots, false, transform);
        pistol = new Gun(GunTypes.pistol, pistolPrefab, pistolDamage, pistolMagazineSize, 1, pistolTimeBetweenShooting, pistolSpread, pistolAimSpread, pistolMovingSpread, pistolRange, pistolReloadTime, pistolTimeBetweenShots, false, transform);
        shotgun = new Gun(GunTypes.shotgun, shotgunPrefab, shotgunDamage, shotgunMagazineSize, shotgunBulletsPerTap, shotgunTimeBetweenShooting, shotgunSpread, shotgunAimSpread, shotgunMovingSpread, shotgunRange, shotgunReloadTime, shotgunTimeBetweenShots, false, transform);

        currentGun = assaultRifle;
        currentGun.readyToShoot = true;
        currentGun.gun.SetActive(true);
    }

    private void Update()
    {
        MyInput();

        //Set Ammunition
        ammunition.SetText(currentGun.bulletsLeft / currentGun.bulletsPerTap + " / " + currentGun.magazineSize / currentGun.bulletsPerTap);
    }

    private void MyInput()
    {
        if (currentGun.allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if ((Input.GetKeyDown(KeyCode.R) && currentGun.bulletsLeft < currentGun.magazineSize && !reloading) || (currentGun.bulletsLeft <=0 && shooting & !reloading)) Reload();

        //Shoot
        if (currentGun.readyToShoot && shooting && !reloading && currentGun.bulletsLeft > 0)
        {
            currentGun.bulletsShot = currentGun.bulletsPerTap;
            Shoot();
        }

        //Switch Gun
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchGun(assaultRifle);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchGun(sniper);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchGun(shotgun);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchGun(pistol);

        //Aim
        if (Input.GetMouseButtonDown(1)) Aim();
        if (Input.GetMouseButtonUp(1)) StopAim();
    }

    private void SwitchGun(Gun gun)
    {
        if (currentGun.type != gun.type)
        {
            reloading = false;
            shooting = false;
            currentGun.readyToShoot = false;
            currentGun.gun.SetActive(false);
            if(aiming) StopAim();
            switch (currentGun.type) 
            {
                case GunTypes.assault: assaultRifle = currentGun; break;
                case GunTypes.sniper: sniper = currentGun; break;
                case GunTypes.pistol: pistol = currentGun; break;
                case GunTypes.shotgun: shotgun = currentGun; break;
                default: return;
            }
            currentGun = gun;
            currentGun.readyToShoot = true;
            currentGun.gun.SetActive(true);
        }
    }

    private void Aim()
    {
        aiming = true;
        transform.GetComponentInChildren<Animator>().Play("Aim");

    }

    private void StopAim()
    {
        aiming = false;
        transform.GetComponentInChildren<Animator>().Play("New State");
    }
    
    private void Shoot()
    {
        currentGun.readyToShoot = false;

        //Spread
        float x = 0; 
        float y = 0;
        float spreadMoving = 0;
        if (moving) spreadMoving = currentGun.movingSpread;
        if (aiming)
        {
            x = Random.Range(-currentGun.aimSpread - spreadMoving, currentGun.aimSpread + spreadMoving);
            y = Random.Range(-currentGun.aimSpread - spreadMoving, currentGun.aimSpread + spreadMoving);
        } else
        {
            x = Random.Range(-currentGun.spread, currentGun.spread);
            y = Random.Range(-currentGun.spread, currentGun.spread);
        }

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, currentGun.range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                Instantiate(enemyBulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
                rayHit.collider.GetComponent<EnemyAi>().TakeDamage(currentGun.damage);
            }
            else
                Instantiate(enviromentBulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
        }

        //ShakeCamera
        camShake.Shake(camShakeDuration, camShakeMagnitude);

        //Graphics
        currentGun.cartridgeEjec.Play();
        currentGun.muzzleFlash.Play();

        currentGun.bulletsLeft--;
        currentGun.bulletsShot--;

        Invoke("ResetShot", currentGun.timeBetweenShooting);

        audioSource.Play();
        if (currentGun.bulletsShot > 0 && currentGun.bulletsLeft > 0)
            Invoke("Shoot", currentGun.timeBetweenShots);
    }
    private void ResetShot()
    {
        currentGun.readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        reload.Play();
        Invoke("ReloadFinished", currentGun.reloadTime);
    }
    private void ReloadFinished()
    {
        currentGun.bulletsLeft = currentGun.magazineSize;
        reloading = false;
    }
    public void SetMoving(bool moving)
    {
        this.moving = moving;
    }
}