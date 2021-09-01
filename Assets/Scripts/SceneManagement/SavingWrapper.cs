using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        //memory cache untuk load time fade in selama 2detik
        [SerializeField] float fadeInTime = 0.2f;
        IEnumerator Start() 
        {
            //store fader
            Fader fader = FindObjectOfType<Fader>();
            //lakukan fadeout sebagai loading
            fader.fadeOutImmediate();
            //melakukan load secara otomatis ketika kita memencet tombol play
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            //setelah loading selesai maka lakukan efek fadein
            yield return fader.FadeIn(fadeInTime);
        }   
        void Update()
        {
            //untuk melakukan load ketika menekan tombol L
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            //untuk melakukan save ketika menekan tombol S
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Save()
        {
            //call to saving system save
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
        public void Load()
        {
            //call to saving system load
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
    }

}