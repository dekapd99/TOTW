//code ini untuk membuat component baru untuk menghilangkan control player ketika cutscene di play
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        //storing gameobject player
        GameObject player;

        private void Start() 
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
            //gameobject dengan tag Player
            player = GameObject.FindWithTag("Player");
        }

        void DisableControl(PlayableDirector pd)
        {
            //canceling action ketika cutscene berjalan
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            //control akan kembali ketika cutscene selesai 
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
