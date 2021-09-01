using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables; //untuk playable director bagian cinematics

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {   
        //jika cutscene belum tertrigger
        bool alreadyTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            //kalo player belum trigger cutscene 
            if (!alreadyTriggered && other.gameObject.tag == "Player")
            {
                // maka play cutscenenya
                alreadyTriggered = true; 
                //melakukan animasi play ketika collider player terkena rigibody introsequence
                GetComponent<PlayableDirector>().Play();
            
            }
        }
    }
}


