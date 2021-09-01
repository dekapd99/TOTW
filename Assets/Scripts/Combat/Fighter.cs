using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Resources;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        //timeBetweenAttacks memberikan delay kepada SetTrigger sebelum melakukan attack lagi (1f = ibarat 1 detik)
        [SerializeField] float timeBetweenAttacks = 1f; 
        //untuk senjata tarohnya di tangan mana kanan dan didefine tidak ada
        [SerializeField] Transform rightHandTransform = null;
        //untuk senjata tarohnya di tangan mana kiri dan didefine tidak ada
        [SerializeField] Transform leftHandTransform = null;
        //untuk jenis senjata apa yang harus mengikuti scriptable objectnya masing-masing
        [SerializeField] Weapon defaultWeapon = null;

        
        //semua enemy mempunyai darah termasuk player untuk memudahkan pemanggilan yang spesifik
        //semakin spesifik variablenya maka semakin spesifik juga melakukan ekstraksi nilai komponennya
        //kondisi sekarang adalah kita mengakses Health melalui variable ini
        Health target;
        //menentukan berapa lama jeda waktu dari lastattack sebelumnya
        //menggunakan Mathf Library, kita bisa langsung attack enemy
        //kenapa infinity karena pada if(timeSinceLastAttack > timeBetweenAttacks) akan menghasilkan return true
        //sehingga kedua object bisa menyerang disaat yang bersamaan
        float timeSinceLastAttack = Mathf.Infinity;
        //tidak punya senjata
        Weapon currentWeapon = null;

        private void Start() 
        {
            //ketika senjata yang sekarang tidak ada maka pasang defaultweapon
            if(currentWeapon == null)
            {
                //saat start disini kita passing defaultWeapon (tidak ada/unarmed)
                EquipWeapon(defaultWeapon);
            }
        }

        private void Update() 
        {
            //+= artinya increment
            //Time.deltaTime, time setelah lasttime dipanggil
            timeSinceLastAttack += Time.deltaTime;


            //kalau target tidak ada maka lanjut ke if selanjutnya
            if (target == null) return;
            if (target.IsDead()) return;

            //kalau target tidak didalam range
            if(!GetIsInRange())   
            {
                //maka player ke posisi dari target
                ////passing 1f yang berarti lakukan max movement
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                //stop vergerak
                GetComponent<Mover>().Cancel();
                //taroh attack animasi disini karena attack terjadi ketika player stop bergerak/movement
                //method animasi attackbehaviour
                AttackBehaviour();
            }
        }
        //membantu pickup weapon
        public void EquipWeapon(Weapon weapon)
        {
            //senjata saat ini
            currentWeapon = weapon;
            //update animasi ketika memiliki senjata
            Animator animator = GetComponent<Animator>();
            //dimana senjata akan dipegang dan animasi apa yang cocok untuk senjata itu
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        private void AttackBehaviour()
        {
            //dokumentasi unity transform.LookAt
            //fixing bug ketika menyerang lawan player tidak melihat ke lawannya
            transform.LookAt(target.transform);
            //saat kita akan attack maka akan menjalankan animasi attack
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
            //method trigget attack
            TriggerAttack();
            timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            //Fixing bug ketika kita attack musuh terus kabur terus attack balik lagi Player akan diam dulu sebentar baru attack.
            //code dibawah ini artinya animasi stopAttack akan direset agar pada saat kembali lagi ke musuh maka player bisa langsung menyerangnya
            GetComponent<Animator>().ResetTrigger("stopAttack");
            //setelah semua ini akan mentrigger void Hit() event dibawah
            //panggil animasinya
            //SetTrigger cara kerjanya seperti boolean value cuma lebih pinter, tapi trigger bekerja kalau diaktifkan
            //yang artinya transisi akan segera terjadi/dilakukan. contoh saat klik target sekali aja, maka dia aktif sekali aja 
            // dan set value langsung jadi FALSE sehingga membantu canceling attack animation
            GetComponent<Animator>().SetTrigger("attack");
            //reset time sejak attack sebelumnya
        }
        //Hit disini merupakan animasi pada Attack --> Animation By Explosive --> Animation Panel
        //Hit merupakan penyebutan dari asset packnys
        //animation event untuk taking damage
        //taking damage ditaroh disini karena kalau ditaroh di AttackBehaviour(), damage langsung diterima sebelum enemy diserang
        void Hit()
        {
            //fixing bug (error handling pada console) --> null reference exception (saat tidak dapat menemukan target)
            if (target == null) 
            {
                return;
            }
            //ini ngasih tau kalo senjata yang digunakan sekarang punya projectile atau tidak
            if(currentWeapon.HasProjectile())
            {
                //TRUE --> kalo punya lempar projectilenya ke target    
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject);
            }
            else
            {
                //saat enemy taking damage
                target.TakeDamage(gameObject, currentWeapon.GetDamage());
            }
        }

        //Shoot disini merupakan animasi pada Bow --> Animation By Explosive --> Animation Panel
        //Shoot merupakan penyebutan dari asset packnys
        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            //isInRange jika target ada didalam range serangan weaponRange
            //transform.position adalah posisi player
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange();
        }

        //fixing bug dimana capsule collider enemy yang sudah mati menghalangi raycast
        //method ini berfungsi untuk mengetahui apakah musuh bisa diserang
        public bool CanAttack(GameObject combatTarget)
        {
            //jika combatTarget tidak ada maka return false yang berati tidak perlu lanjut kebawah dari if ini 
            //tapi kembali lempar ke PlayerController bagian InteractWithCombat loop if foreach pertama
            if (combatTarget == null) 
            { 
                return false; 
            }
            //storing health variable untuk targetToTest
            Health targetToTest = combatTarget.GetComponent<Health>();
            //jika targetnya ada dan targetnya belum mati maka target tersebut bisa diserang
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            //action scheduler
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        //tadinya kita klik target terus klik area selain target maka dia akan mengaktifkan method cancel untuk cancel
        public void Cancel()
        {
            StopAttack();
            target = null;
            //cancel semua yang terjadi 
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            //fixing bug untuk mengatasi apabila kebalikan dari stop attack terjadi
            //melakukan reset trigger sehingga bisa langsung attack
            GetComponent<Animator>().ResetTrigger("Attack");
            //fixing bug
            //cancelling attack animation ketika pergi dari musuh
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public object CaptureState()
        {
            //saving string ke capture state untuk save
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            //saat load game maka senjata yang sudah disave akan ke load juga
            //pasang weapon unarmed
            //UnityEngine.Resources diberikan seperti ini untuk menghindari ambigu dengan mengarah ke namespace apabila dberikan nama Resources saja
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            //equip senjata yang sudah diambil 
            EquipWeapon(weapon);
        }
    }
}