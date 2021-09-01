using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour 
    {
        [SerializeField] Weapon weapon = null;
        //respawn time untuk senjata yang diambil
        [SerializeField] float respawnTime = 5;

        private void OnTriggerEnter(Collider other) 
        {
            //kalau player yang deketin gameObjectnya maka equip weapon dan destroy gameObjectya 
            if (other.gameObject.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }            
        }
        //method enumerator ketika kita mengambil senjata senjata tersebut tidak menghilang selamanya
        private IEnumerator HideForSeconds(float seconds)
        {
            ShowPickup(false);
            //nunggu sampai senjatanya spawn lagi
            yield return new WaitForSeconds(seconds);
            ShowPickup(true); 
        }

        private void ShowPickup(bool shouldShow)
        {
            //ketika senjata sudah diambil aktifkan collidernya sehingga objectnya muncul kembali
            GetComponent<Collider>().enabled = shouldShow;
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
    
}


