using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Assets.Scripts.Weapons
{
    public class LookAtObjectAnimationRigging : MonoBehaviour
    {
        private Rig rig;
        private float targetWeight = 0f;

        private void Awake()
        {
            rig = GetComponent<Rig>();
        }

        private void Update()
        {
            rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * 10f);

            if(Input.GetKeyDown(KeyCode.T))
            {
                targetWeight = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                targetWeight = 0f;
            }
        }
    }
}
