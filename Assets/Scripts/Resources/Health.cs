using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        //serializefield disini digunakan untuk mengaktifkan kon figurasi 
        [SerializeField] float healthPoints = 100f;
        
        //ini bool buat assign variabel apakah enemy udah mati/belum --> disini artinya FALSE = belum mati
        bool isDead = false;

        private void Start() 
        {
            //ngambil health dari basestats method GetHealth
            healthPoints = GetComponent<BaseStats>().GetStat(Stat.Health);    
        }
        //ini simple class untuk mengetahui apakah enemy udah mati atau belum?
        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            //0 sebagai batasnya
            //fungsi matematika untuk menghitung dengan cara mengambil nilai max dari value ini
            //yaitu 100, dan 0 --> jika nanti nilainya minus maka 0 akan lebih besar dari nilai minus
            //maka kita akan mengambil nilai akhir yaitu 0 jadi gak mungkin angkanya kurang dari 0
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            // healthPoints == 0 --> kenapa tidak <= 0 ?? karena di code healthPoints = Mathf.Max(healthPoints - damage, 0);
            //batasnya gak mungkin kurang dari 0 jadinya healthPoints == 0
            if (healthPoints == 0)
            {
                //method die
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetPercentage()
        {
            //healthPoint yang sekarang dibagi dengan health point base stats nanti hasilnya dapat persen
            return 100 * (healthPoints / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Die()
        {
            //cek apakah sudah mati, kalau memang true, ya langsung return karena kalau udah mati yaudah mati
            //jangan sampai enemy melakukan apa apa setelah mati
            if(isDead) return;
            //kalau bener mati maka lanjutkan dengan mentrigger animasi die
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            //untuk memberitahu ke actionscheduler agar cancel StartAction untuk memberhentikan enemy/player yang sudah mati
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AwardExperience(GameObject instigator)
        {
            //storing experience component
            Experience experience = instigator.GetComponent<Experience>();
            //kalau tidak ada exp maka return aja lanjutin ke line code berikutnya
            if (experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }


        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            //casting state menjadi float terus taroh ke healthpoints
            healthPoints = (float)state;
            //FIXING BUG: saat kita membunuh enemy kemudian melakukan save, dan stop play
            //nanti pas kita play lagi terus pencet L nanti enemy yang udah mati hidup lagi
            if (healthPoints == 0)
            {
                //method die
                Die();
            }
        }
    }
}
