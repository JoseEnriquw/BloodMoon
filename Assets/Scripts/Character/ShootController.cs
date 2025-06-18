using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using Assets.Scripts.Character;
public class ShootController : MonoBehaviour
{
   [SerializeField]CinemachineVirtualCamera aimvirtualCamera;
    private StarterAssetsInputs starterAssetsInputs;
    private StarterAssets.ThirdPersonController thirdPersonController;


    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
   // [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Animator animator;
    [SerializeField] PlayerData playerData;
    [SerializeField] private AudioClip shootAudioClip;
    private AudioSource audioSource;
    private bool IsEquiped=false;

    private void Awake()
    {
        starterAssetsInputs= GetComponent<StarterAssetsInputs>();
        thirdPersonController= GetComponent<StarterAssets.ThirdPersonController>();
        animator= GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        Vector3 mouseWoldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
           // debugTransform.position = raycastHit.point;
            mouseWoldPosition=raycastHit.point;
        }
        HandleAnimations();
        if (IsEquiped)
        {
            if (starterAssetsInputs.aim)
            {
                aimvirtualCamera.gameObject.SetActive(true);
                thirdPersonController.SetSensitivity(aimSensitivity);
                thirdPersonController.SetRotateOnMove(false);


                Vector3 worldAimTarget = mouseWoldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            }
            else
            {
                aimvirtualCamera.gameObject.SetActive(false);
                thirdPersonController.SetSensitivity(normalSensitivity);
                thirdPersonController.SetRotateOnMove(true);
            }
            if (starterAssetsInputs.shoot)
            {
                if (playerData.Bullets > 0) {
                    if (shootAudioClip != null)
                        audioSource.PlayOneShot(shootAudioClip);
                    Vector3 aimDir = (mouseWoldPosition - spawnBulletPosition.position).normalized;
                    Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                    starterAssetsInputs.shoot = false;
                    playerData.Bullets -= 1;
                }
                
            }
        }
       
    }
    private void HandleAnimations()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetLayerWeight(1,Mathf.Lerp(animator.GetLayerWeight(1),1f,Time.deltaTime *10f));
            EquipAnimation();
        }

       
    }
    private void EquipAnimation()
    {
        if (IsEquiped)
        {
            animator.SetBool("UsingGun", false);
        }
        else
        {
            animator.SetBool("UsingGun", true);
        }
        IsEquiped = !IsEquiped;
    }
}
