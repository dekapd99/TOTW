using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        //references health component
        Health health;
        private void Awake() 
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }
        
        private void Update() 
        {
            //untuk mengambil hasil persentase dari darahnya dan memasukkannya ke text
            //String.Format untuk membuat format angka tidak memiliki banyak angka desimal
            //maksudnya adalah ambil persentase yang sudah di hitung di health.cs kemudian masukkan ke dalam format string
            //ke format ini "{0:0}%" :0 artinya tidak ada desimal .0 artinya desimalnya 1 aja
            GetComponent<Text>().text = String.Format("{0:0.0}%", health.GetPercentage()); 
        }
    }
}