using System;
using UnityEngine;
using UnityEngine.UI;
using RPG.Resources;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        //references health component
        Fighter fighter;
        private void Awake() 
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        
        private void Update() 
        {
            //cek apakah ada target
            //jika target tidak ada maka text display Tidak ada
            if(fighter.GetTarget() == null)
            {
                //
                GetComponent<Text>().text = "Tidak Ada";
                return;
            }

            Health health = fighter.GetTarget();
            //untuk mengambil hasil persentase dari darahnya dan memasukkannya ke text
            //String.Format untuk membuat format angka tidak memiliki banyak angka desimal
            //maksudnya adalah ambil persentase yang sudah di hitung di health.cs kemudian masukkan ke dalam format string
            //ke format ini "{0:0}%" :0 artinya tidak ada desimal .0 artinya desimalnya 1 aja
            GetComponent<Text>().text = String.Format("{0:0.0}%", health.GetPercentage()); 
        }
    }
}