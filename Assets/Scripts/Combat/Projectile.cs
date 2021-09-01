using UnityEngine;
using RPG.Resources;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        //speed dari projectile
        [SerializeField] float speed = 1;
        //memory cache apakah projectile mengincar target yang ditembaki
        [SerializeField] bool isHoming = true;
        //memory cache untuk efek yang digunakan
        [SerializeField] GameObject hitEffect = null;
        //memory cache untuk nanti destroy projectile ketika tidak mengenai musuh
        [SerializeField] float maxLifeTime = 10;
        //memory cache projectile mana yang langsung destroy
        [SerializeField] GameObject[] destroyOnHit = null;
        //memory cache efek setelah menabrak 
        [SerializeField] float lifeAfterImpact = 2;
        //storing health dari si target
        Health target = null;
        //ini untuk system exp untuk mengecek siapa yang akan mendapatkan exp tersebut
        GameObject instigator = null;    
        float damage = 0;
        
        private void Start() 
        {
            // agar projectile mengarah ke targetnya tapi tidak mengincar
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            //memastikan apakah projectile jenis ini adalah tipe yang mengincar target
            //BUG FIXING: pada projectile homing ketika target sudah mati projectilenya nyangkut
            if(isHoming && !target.IsDead())
            {
                // agar projectile mengarah ke targetnya
                transform.LookAt(GetAimLocation());
            }
            
            //agar projectile bergerak lurus terhadap sumbu z --> 0, 0, 1 (forward) ke arah target dengan kecepatan per frame
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        //passing ke instance variable Health diatas
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            //menunjuk pada gameobject ini yang akan diattack
            this.target = target;
            //menunjuk pada gameobject ini yang akan diterima damagenya dari projectile
            this.damage = damage;
            //instigator untuk semua method settarget   
            this.instigator = instigator;
            //projectile yang gak kena musuh akan dihapus ketika melewati maxtime
            Destroy(gameObject, maxLifeTime);
        }
        private Vector3 GetAimLocation()
        {
            //untuk mengarah ke capsule collider karena tinggi player setara dengan capsule collider
            //bukan setara dengan lokasinya
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            //kalo targetgak punya capsulecollider return target position
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            //mengarah ke posisi target ke arah sumbu y (up) 0, 1, 0 (up) dikalikan dengan setengah dari tinggi capsule collider
            return target.transform.position + Vector3.up * (targetCapsule.height / 2);
        }

        //mekanisme ini untu projectile saat collide/crash dengan enemy
        private void OnTriggerEnter(Collider other) 
        {
            //kalau projectilenya kena target maka enemy akan menerima damage
            if(other.GetComponent<Health>() != target) return;
            //kalau enemy sudah mati jangan tembak projectilenya lagi
            if(target.IsDead()) return;
            target.TakeDamage(instigator, damage);
            //agar panah sudah tidak ada kecepatannya setelah menambrak target
            speed = 0;
            //ketiak ada hiteffect maka munculkan hit efeknya pada target sasaran
            if(hitEffect != null)
            {
                //passing kedalam instantiate
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
            //
            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            //destroy gameObject projectilenya
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
