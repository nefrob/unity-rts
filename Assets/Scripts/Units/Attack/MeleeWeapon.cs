using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] private MeleeAttack meleeAttack = null;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger enter");
        meleeAttack.OnWeaponHit(other);
    }
}
