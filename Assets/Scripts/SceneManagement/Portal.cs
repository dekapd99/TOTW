using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        //untuk membuat sambungan antar portal 
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }
        //inisiasi sceneToLoad yang bisa di konfigurasi di unity engine
        [SerializeField] int sceneToLoad = -1;
        //cache memory spawnPoint 
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;
        //loading next scene
        private void OnTriggerEnter(Collider other) 
        {
            //cek apakah player yang ke portal?
            if (other.tag == "Player")
            {
                //coroutine yang berjalan diantara scenes
                StartCoroutine(Transition());
            }
        }
        //transisi ini memungkinkan kita mentransfer informasi dari scene setelah/sebelumnya
        private IEnumerator Transition()
        {
            //cek apakah portal sceneToLoad sudah di set apa belum
            if(sceneToLoad < 0)
            {
                Debug.LogError("Scene to load belum di set.");
                yield break;
            }

            //sebelum scene load
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>();
            //loading fadeout
            yield return fader.FadeOut(fadeOutTime);

            //save current level agar bisa melakukan save diantara scene
            //reloading level
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            wrapper.Save();

            //load scene yang di tuju dan anak memanggil coroutine yang sama jika scenenya sudah selesai loading
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            //load current level agar bisa melakukan load diantara scene
            wrapper.Load();

            //sambungin sama portal yang lain dan melakukan update terhadap informasi player seperti lokasi/posisi
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            //untuk menunggu camera stabil
            yield return new WaitForSeconds(fadeWaitTime);
            //load fadeIn
            yield return fader.FadeIn(fadeInTime);
            //setelah scene load 
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            //disable navmeshagent
            player.GetComponent<NavMeshAgent>().enabled = false; 
            //fixing bug ketika pergi ke portal terjadi conflic postion antara NavMeshAgent
            //dengan spawnPoint
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            //foreach ketika player menemukan/ada portal lainnya
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                //akan return ke portal kalau memiliki tujuan yang tepat
                if(portal == this) continue;
                if(portal.destination != destination) continue;

                return portal;
            }
            //ketika tidak ada portal
            return null;
        }
    }
}
