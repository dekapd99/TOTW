using UnityEngine;
using System.Collections;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        private void Awake() 
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        //method ini digunakan ketika melakukan load dimana pun secara otomatis ketika memencet play
        //maka akan terjadi efek fadeout
        public void fadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        //membuat waktu fadeout dengan coroutine
        //kita harus membuat berapa banyak alpha pada canvas group
        //catatan:
        //N = time/deltaTime
        //deltaAlpha 1 = 1 / N
        //deltaAlpha = 1 * deltaTime / time  
        public IEnumerator FadeOut(float time)
        {
            //kita akan melakukan update alpha untuk setiap framnya
            while(canvasGroup.alpha < 1) // alpha tidak sama dengan 1
            {
                //melakukan update alpha secara increment per frame
                canvasGroup.alpha += Time.deltaTime / time;
                //naikin alpha sampai 1
                //coroutine ini harus jalan lagi pas masuk ke next frame
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            //kita akan melakukan update alpha untuk setiap framnya
            while (canvasGroup.alpha > 0) // alpha tidak sama dengan 1
            {   
                //melakukan update alpha secara decrement per frame
                canvasGroup.alpha -= Time.deltaTime / time;
                //naikin alpha sampai 1
                //coroutine ini harus jalan lagi pas masuk ke next frame
                yield return null;
            }
        }
    }
}
