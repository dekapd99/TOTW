//melakukan script terhadap object weapon
using UnityEngine;
using RPG.Resources;

namespace RPG.Combat
{
    //untuk membuat semua menu baru pada saat klik kanan di folder weapons
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        //untuk animasi pemegangan senjata berdasarkan jenis senjatanya dan didefine tidak ada
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        //senjata dalam bentuk prefabs yang didefine tidak ada
        [SerializeField] GameObject equippedPrefab = null;
        //weaponDamage memberikan damage kepada enemy sebanyak 5 setiap attacknya
        [SerializeField] float weaponDamage = 5f;
        //costumable weapon range (2f = ibarat 2meter/2frame)
        [SerializeField] float weaponRange = 2f;
        //inisiasi righthand
        [SerializeField] bool isRightHanded = true;
        //memory cache projectile si panah, menunjukkan kalo ada senjata yang pake projectile ada juga yang tidak punya projectile
        [SerializeField] Projectile projectile = null;
        //string reference
        const string weaponName = "Weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            //menghindari kondisi null agar kalo tidak ada senjata jangan membuat senjata otomatis
            if(equippedPrefab != null)
            {
                //method untuk cek apakah senjata ini tangan kiri/kanan
                Transform handTransform = GetTransform(rightHand, leftHand);
                //agar senjata baru muncul ketika di play
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            //menghindari kondisi null agar kalo tidak ada senjata yang digunakan jangan override animasinya
            if (animatorOverride != null)
            {
                //agar animasi senjata muncul ketika digunakan
                animator.runtimeAnimatorController = animatorOverride;
            }
            // kalau sudah di override 
            else if (overrideController != null)
            {   
                //cari parentnya di runtimeanimator slot
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }    
        }

        //method untuk cek senjata yang sedang digunakan sekarang
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            //cek apakah ada senjata ditangan kanan
            Transform oldWeapon = rightHand.Find(weaponName);
            //kalau oldweapon gak ada ditangan kanan
            if(oldWeapon == null)
            {
                //maka cek tangan kirinya
                oldWeapon = leftHand.Find(weaponName);
            }
            //kalo tangan kiri dan kanan gak ada juga maka gk usah ngelakuin apa apa return aja
            if(oldWeapon == null) return;
            //debug
            oldWeapon.name = "Destroying";
            //kalo ada senjata ditangan kiri/kanan maka hilangkan senjata lamanya
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            //cek apakah senjata ini tangan kiri/kanan
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        //method ini digunakan untuk cek apakah senjata ini ada projectilenya atau tidak
        public bool HasProjectile()
        {
            return projectile != null;
        }
        //method ini untuk melemparkan projectile bagi senjata yang punya projectile
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand,leftHand).position, Quaternion.identity);
            //ini ngasih tau ke projectile ini targetnya dan kejar targetnya    
            projectileInstance.SetTarget(target, instigator, weaponDamage);
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetRange()
        {
            return weaponRange;
        }
    }
}