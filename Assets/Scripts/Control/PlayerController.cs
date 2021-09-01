using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;

//namespace RPG.xxx karena kalau nanti kita akan mengimport sesuatu dengan nama control, maka 
//file tidak akan konflik dengan Control
namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        //cache reference untuk health
        Health health;

        public void Start()
        {
            //untuk mengambil health dari player
            health = GetComponent<Health>();
        }

        private void Update() 
        {
            //kalau health sudah mati maka return yang berarti kita tidak akan melakukan apa-apa diupdate
            if (health.IsDead()) return;
            //method untuk hubungan interaksi movement dengan combat
            //kalau kita berinteraksi dengan combat, jika tidak 
            //maka akan skip ke step selanjutnya -->interact with movement
            if (InteractWithCombat()) return;
            //method untuk hubungan interaksi kursor dengan movement
            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            //agar dapat melakukan raycast pada combat
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            //untuk setiap raycast yang hit akan di store ke dalam list hits
            foreach (RaycastHit hit in hits)
            {
                //hubungkan dengan info transform combattarget
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                //kalau kita tidak mempunyai CombatTarget berarti tidak bisa di klik
                //gunanya untuk menghindari klik diri sendiri, klik NPC
                if (target == null) continue;
                //introduce targetGameObject untuk mendapatkan target sebagai gameObject
                GameObject targetGameObject = target.gameObject;
                //jika target tidak ada maka lanjut ke if selanjutnya
                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    //maka continue yang berarti keluar dari fungsi if ini tapi masih didalam fungsi foreach
                    continue;
                }
                //Input.GetMouseButtonDown() = model klik berkali kali bukan yang tekan
                //0 artinya klik kiri, 1 artinya klik tengah/scroll dan 2 artinya klik kanan
                if(Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                //terjadi ketika menemukan combattarget pada terrain
                return true;
            }
            //terjadi ketika tidak menemukan combattarget pada terrain
            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            //method baru (getmouseray) untuk ray yang berdasarkan pada klik mouse
            //nembak Ray, dapet posisi lalu di store di Hit lalu di passing keluar ke kurusor yang baru
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if(Input.GetMouseButton(0))
                {
                    //memanggil method StartMoveAction nanti akan dilanjutkan ke MoveTo dari script mover
                    //passing 1f yang berarti lakukan max movement
                    GetComponent<Mover>().StartMoveAction(hit.point, 1f);
                }
                //jika berinteraksi pada area yang bisa movement
                return true;
            }
            //jika berinteraksi pada daerah yang tidak bisa movement
            //sehingga saat kita klik keluar terrain maka player akan diam
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
