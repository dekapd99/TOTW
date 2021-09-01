using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        //radius gizmos waypoint
        const float waypointGizmosRadius = 0.3f;
        //visualisasi dari patrolpath dari enemy
        private void OnDrawGizmos() 
        {
            //procedural
            //menggunakan for loop untuk mereferensikan waypoint dari patrolpath
            //menggunakan for agar bisa kembali ke waypoint sebelumnya
            //transform.childCount menunjukkan berapa banyak child dalam 1 parent
            //mulai dari 0 = Waypoint, 1 = Waypoint (1) dan 2 = Waypoint (2)
            for (int i = 0; i < transform.childCount; i++)
            {
                //method untuk GetNextIndex
                int j = GetNextIndex(i);
                //method GetWaypoint agar memudahkan memanggil method berkali kali
                //membuat gizmos untuk menyambungkan setiap posisi dari child dengan radius dari sphere nya
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmosRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        //public agar bisa direferensikan di aicontroller.cs
        public int GetNextIndex(int i)
        {
            //kalau kita berada pada transform.childCount
            //contoh kalau kita punya 3 child, dan i kita punya 2, berarti kita sedang ada di jalur terakhir
            //berarti next jalurnya harus 0 karena harus kembali ke path yang 0. sehingga akan menjadi loop
            if (i + 1 == transform.childCount)
            {
                //
                return 0;
            }    
            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            //membuat gizmos untuk menyambungkan setiap posisi dari child
            return transform.GetChild(i).position;
        }
    }
}