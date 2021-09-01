using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        // Update is called once per frame
        [SerializeField] Transform target; 
        //LateUpdate untuk mengurangi miss match antara player yang bergerak dengan kamera
        //walaupun pake update biasa perbedaannya gak jauh ini cuma detailing process 
        void LateUpdate()
        {
            transform.position = target.position; 
        }
    }
}
