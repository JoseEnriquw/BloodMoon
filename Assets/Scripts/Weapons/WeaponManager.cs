using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Animator))]
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Weapon currentWeapon;

    [Header("Weapon Positions Settings")]
    [SerializeField] private Transform weaponHolderPos;
    [SerializeField] private Transform equipPos;
    [SerializeField] private Transform aimingPos;

    [Header("Right Hand Target")]
    [SerializeField] private TwoBoneIKConstraint rightHandIK;
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform rightHandHint;

    [Header("Left Hand Target")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private Transform leftHandHint;

    [Header("IK Positions")]
    [SerializeField] private Transform IKRightHandPos;
    [SerializeField] private Transform IKLeftHandPos;
    [SerializeField] private Transform IKLeftHandHintPos;
    [SerializeField] private Transform IKRightHandHintPos;

    private Animator playerAnimator;
    private bool IsAiming;
    private bool IsEquiped;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        IsEquiped = false;
        IsAiming = false;
        rightHandIK.weight = 0.0f;
        leftHandIK.weight = 0.0f;
        PutBackWeapon();
    }

    private void Update()
    {
        HandleAnimations();
        HandleRigging();
    }

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

    public void SetAiming()
    {
        IsAiming = !IsAiming;
    }

    public void Shoot() 
    {
        if (currentWeapon == null) return;
        // Here you can add the logic to handle shooting, like instantiating bullets or playing sound effects.
        Debug.Log("Shooting with weapon: " + currentWeapon.name);

        currentWeapon.Shoot();
        playerAnimator.SetBool("Shoot", false);
    }


    #region Animation

    private void HandleAnimations()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EquipAnimation();
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsEquiped)
        {
            ShootAnimation();
        }
    }

    void HandleRigging()
    {
        if (!IsAiming)
        {
            leftHandIK.weight = 0f;
            rightHandIK.weight = 0.0f;
        }
        else
        {
            leftHandIK.weight = 1f;
            leftHandTarget.SetPositionAndRotation(IKLeftHandPos.position, IKLeftHandPos.rotation);
            leftHandHint.SetPositionAndRotation(IKLeftHandHintPos.position, IKLeftHandHintPos.rotation);  
        }
    }

    private void ShootAnimation()
    {
        playerAnimator.SetBool("Shoot", true);
    }

    private void EquipAnimation()
    {
        if (IsEquiped)
        {
            playerAnimator.SetBool("UsingGun", false);
        }
        else
        {
            playerAnimator.SetBool("UsingGun", true);
        }
        IsEquiped = !IsEquiped;
    }

    #endregion

}
