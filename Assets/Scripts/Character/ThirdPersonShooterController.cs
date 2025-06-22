using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera aimvirtualCamera;
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private GameObject crossHair;

    [Header("Weapon Positions Settings")]
    [SerializeField] private Transform weaponHolderPos;
    [SerializeField] private Transform equipPos;
    [SerializeField] private Transform aimingPos;

    [Header("Aim Targets")]
    [SerializeField] private MultiAimConstraint aimRing;
    [SerializeField] private MultiAimConstraint bodyAimRing;
    [SerializeField] private Transform aimTarget;

    [Header("Left Hand Target")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform leftHandHint;

    [Header("IK Positions")]
    [SerializeField] private Transform IKLeftHandPos;
    [SerializeField] private Transform IKLeftHandHintPos;

    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new();
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] PlayerData playerData;
    [SerializeField] private AudioClip shootAudioClip;
    //[SerializeField] private Transform debugTransform;

    private StarterAssetsInputs starterAssetsInputs;
    private StarterAssets.ThirdPersonController thirdPersonController;
    private Animator animator;

    private bool IsInHand;
    private bool IsEquiped = false;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<StarterAssets.ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        aimvirtualCamera.gameObject.SetActive(false);
        IsInHand = false;
        IsEquiped = false;
        aimRing.weight = 0.0f;
        bodyAimRing.weight = 0.0f;
        leftHandIK.weight = 0.0f;
        crossHair.SetActive(false);
        PutBackWeapon();
    }

    private void Update()
    {
        Vector3 mouseWoldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            //debugTransform.position = raycastHit.point;
            mouseWoldPosition = raycastHit.point;
        }
        HandleAnimations();
        if (IsEquiped)
        {
            if (starterAssetsInputs.aim)
            {
                crossHair.SetActive(true);
                aimvirtualCamera.gameObject.SetActive(true);
                thirdPersonController.SetSensitivity(aimSensitivity);
                thirdPersonController.SetRotateOnMove(false);

                aimTarget.position = mouseWoldPosition;
                Vector3 worldAimTarget = mouseWoldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
                if (starterAssetsInputs.shoot)
                {
                    if (playerData.Bullets > 0)
                    {
                        //if (shootAudioClip != null)
                        //    audioSource.PlayOneShot(shootAudioClip);
                        Vector3 aimDir = (mouseWoldPosition - spawnBulletPosition.position).normalized;
                        //Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                        ShootAnimation(true);
                        currentWeapon.Shoot(aimDir,spawnBulletPosition.position);
                        starterAssetsInputs.shoot = false;
                        playerData.Bullets -= 1;
                        ShootAnimation(false);
                    }

                }
            }
            else
            {
                crossHair.SetActive(false);
                aimvirtualCamera.gameObject.SetActive(false);
                thirdPersonController.SetSensitivity(normalSensitivity);
                thirdPersonController.SetRotateOnMove(true);
            }
        }


    }

    public void SetIsInHand()
    {
        IsInHand = !IsInHand;
    }


    #region Animations

    private void HandleAnimations()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            EquipAnimation();
        }

        HandleRigging();
    }

    private void EquipAnimation()
    {
        if (IsEquiped)
        {
            animator.SetBool("UsingGun", false);
            //crossHair.SetActive(false);
        }
        else
        {
            animator.SetBool("UsingGun", true);
            //crossHair.SetActive(true);
        }
        IsEquiped = !IsEquiped;
    }

    private void ShootAnimation(bool isShooting)
    {
        animator.SetBool("Shoot", isShooting);
    }

    void HandleRigging()
    {
        if (!IsInHand)
        {
            leftHandIK.weight = 0f;
            aimRing.weight = 0f;
            bodyAimRing.weight = 0f;
        }
        else
        {
            leftHandIK.weight = 1f;
            leftHandTarget.SetPositionAndRotation(IKLeftHandPos.position, IKLeftHandPos.rotation);
            leftHandHint.SetPositionAndRotation(IKLeftHandHintPos.position, IKLeftHandHintPos.rotation);

            bodyAimRing.weight = 0.7f;
            aimRing.weight = 1f;
        }
    }
    #endregion

    public void PutBackWeapon()
    {
        if (currentWeapon == null) return;
        currentWeapon.transform.parent = weaponHolderPos.parent;
        currentWeapon.transform.SetPositionAndRotation(weaponHolderPos.position, weaponHolderPos.rotation);
    }

    public void TakeWeapon()
    {
        if (currentWeapon == null) return;
        // Set weapon parent and position.
        currentWeapon.transform.parent = equipPos;
        currentWeapon.transform.SetPositionAndRotation(equipPos.position, equipPos.rotation);
    }

}
