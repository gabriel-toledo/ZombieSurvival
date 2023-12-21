using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GunController : MonoBehaviour
{
    //AssaultRifle stats
    public int assaultDamage;
    public float assaultTimeBetweenShooting, assaultSpread, assaultRange, assaultReloadTime, assaultTimeBetweenShots;
    public int assaultMagazineSize, assaultBulletsPerTap;
    public bool assaultAllowButtonHold;

    //Sniper stats
    public int sniperDamage;
    public float sniperTimeBetweenShooting, sniperSpread, sniperRange, sniperReloadTime, sniperTimeBetweenShots;
    public int sniperMagazineSize;

    //Pistol stats
    public int pistolDamage;
    public float pistolTimeBetweenShooting, pistolSpread, pistolRange, pistolReloadTime, pistolTimeBetweenShots;
    public int pistolMagazineSize;

    //Shotgun stats
    public int shotgunDamage;
    public float shotgunTimeBetweenShooting, shotgunSpread, shotgunRange, shotgunReloadTime, shotgunTimeBetweenShots;
    public int shotgunMagazineSize, shotgunBulletsPerTap;

    //bools 
    bool shooting, reloading;

    //Reference
    public Camera fpsCam;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy; 
    public AudioSource audioSource; 
    public AudioSource reload;
    public GameObject assaultPrefab;
    public GameObject sniperPrefab;
    public GameObject pistolPrefab;
    public GameObject shotgunPrefab;

    //Graphics
    public GameObject enviromentBulletHoleGraphic, enemyBulletHoleGraphic;
    public CameraShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

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
        public float timeBetweenShooting, spread, range, timeBetweenShots, reloadTime;
        public int magazineSize, bulletsPerTap;
        public bool allowButtonHold;
        public int bulletsLeft, bulletsShot;
        public GameObject gun;
        public Transform attackPoint;
        public ParticleSystem muzzleFlash, cartridgeEjec;

        public Gun(GunTypes type, GameObject gunPrefab, int damage, int magazineSize, int bulletsPerTap, float timeBetweenShooting, float spread, float range, float reloadTime, float timeBetweenShots, bool allowButtonHold, Transform gunTransform)
        {
            this.type = type;
            this.damage = damage;
            this.magazineSize = magazineSize * bulletsPerTap;
            this.bulletsPerTap = bulletsPerTap;
            this.timeBetweenShooting = timeBetweenShooting;
            this.spread = spread;
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
            Debug.Log(muzzleFlash, cartridgeEjec);
        }
    }

    private void Awake()
    {
        assaultRifle = new Gun(GunTypes.assault, assaultPrefab, assaultDamage, assaultMagazineSize, assaultBulletsPerTap, assaultTimeBetweenShooting, assaultSpread, assaultRange, assaultReloadTime, assaultTimeBetweenShots, assaultAllowButtonHold, transform);
        sniper = new Gun(GunTypes.sniper, sniperPrefab, sniperDamage, sniperMagazineSize, 1, sniperTimeBetweenShooting, sniperSpread, sniperRange, sniperReloadTime, sniperTimeBetweenShots, false, transform);
        pistol = new Gun(GunTypes.pistol, pistolPrefab, pistolDamage, pistolMagazineSize, 1, pistolTimeBetweenShooting, pistolSpread, pistolRange, pistolReloadTime, pistolTimeBetweenShots, false, transform);
        shotgun = new Gun(GunTypes.shotgun, shotgunPrefab, shotgunDamage, shotgunMagazineSize, shotgunBulletsPerTap, shotgunTimeBetweenShooting, shotgunSpread, shotgunRange, shotgunReloadTime, shotgunTimeBetweenShots, false, transform);

        currentGun = assaultRifle;
        currentGun.readyToShoot = true;
        currentGun.gun.SetActive(true);
    }

    private void Update()
    {
        MyInput();

        //SetText
        text.SetText(currentGun.bulletsLeft / currentGun.bulletsPerTap + " / " + currentGun.magazineSize / currentGun.bulletsPerTap);
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
    }

    private void SwitchGun(Gun gun)
    {
        if (currentGun.type != gun.type)
        {
            reloading = false;
            shooting = false;
            currentGun.readyToShoot = false;
            currentGun.gun.SetActive(false);
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

    private void Shoot()
    {
        currentGun.readyToShoot = false;

        //Spread
        float x = Random.Range(-currentGun.spread, currentGun.spread);
        float y = Random.Range(-currentGun.spread, currentGun.spread);

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
}