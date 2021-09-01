using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            //kalau partikel tidak perlu ada lagi sudah collision dengan target
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                //maka destroy gameobjectnya
                Destroy(gameObject);
            }
        }
    }
}