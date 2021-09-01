using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        //jarak untuk chase
        [SerializeField] float chaseDistance = 5f;
        //waktu untuk suspicion selama 3 detik
        //suspicious seperti diam ditempat melihat sekitar
        [SerializeField] float suspicionTime = 3f;
        //referensi patrolpath
        [SerializeField] PatrolPath patrolPath;
        //parameter wayPointTolerance
        [SerializeField] float waypointTolerance = 1f;
        //dwelltime digunakan agar setiap enemy nyampe di waypointnya dia akan diam dulu melihat sekitar
        [SerializeField] float waypointDwellTime = 3f;
        //memproteksi patrol fraction agar valuenya 0-1
        [Range(0,1)]
        //speed enemy patrol: fraction --> bagian terkecil/persenan dari maxSpeed yang akan digunakan sesuai kebutuhan
        [SerializeField] float patrolSpeedFraction = 0.2f;
        //cache reference dari figther
        Fighter fighter;
        //cache reference untuk Health component
        Health health;
        //cache reference untuk mover
        Mover mover;
        //cache reference untuk player 
        GameObject player;
        //memory untuk menyimpan lokasi/posisi
        Vector3 guardPosition;
        //adjusting memory Enemy AI dengan library Mathf
        //ini adalah inisiasi sebelum masuk ke state suspicion --> tidak perlu finite machine
        //karena mekanik game ini tidaklah kompleks
        float timeSinceLastSawPlayer = Mathf.Infinity;
        //kita mau memberikan batasan saat enemy move ke waypoint
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        //declare index currentWaypointIndex mulai dari index 0
        int currentWaypointIndex = 0;

        private void Start() 
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            //FindWithTag unity documentation
            //fungsi untuk menemukan gameobject di sandbox dengan string reference dengan tag Player
            player = GameObject.FindWithTag("Player");
            //enemy menggunakan posisi saat ini
            guardPosition = transform.position;
        }

        private void Update()
        {
            //kalau health sudah mati maka return yang berarti kita tidak akan melakukan apa-apa diupdate
            if (health.IsDead()) return;
            //method untuk enemy mengejar player
            //dan menentukan apakah player bisa diserang/tidak berdasarkan chase jarak/range, hal ini untuk menghindari bug
            //ketika enemy telah membunuh player, enemy akan terus menyerang player yang sudah mati
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                //method untuk attack behaviour
                AttackBehaviour();
            }
            //kalau waktu enemy terakhir melihat player lebih kecil dari suspicionTime maka enemy akan suspicious
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                //method SuspicionBehaviour
                //suspicion state terjadi ketika saat tidak bisa menyerang
                SuspicionBehaviour();
            }
            else
            {
                //method PatrolBehaviour
                //terjadi ketika sudah tidak suspicion/diluar range chase
                PatrolBehaviour();
            }
            //method updatetimer untuk increment per frame
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            //increment sesuai dengan per-frame
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            //guardPosition di set default
            Vector3 nextPosition = guardPosition;

            //fungsi if ini berguna untuk melakukan patroli
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                //untuk canceling enemy chase apabila diluar range
                //startmoveaction akan cancel secara otomatis ketika tidak ada targetnya dia akan patroli ke nextPosition
                //sekarang kita passing in patrolSpeedFraction untuk kecepatan enemy jalan
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
            
        }

        //method ini merupakan bool untuk mengetahui apakah enemy ada di waypoint
        private bool AtWaypoint()
        {
            //kalkulasikan distanceToWaypoint berdasarkan jarak dari posisi dengan mengetahio currentwaypoint
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        //patrolling path sesuai dengan waypoint yang telah dibuat secara berurutan dan akan pergi ke waypoint selanjutnya
        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        // untuk mendapatkan info lokasi waypoint saat ini
        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            //cancel current action
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            //reset 
            timeSinceLastSawPlayer = 0;
            //saat bisa attack maka enemy akan menyerang
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            //method Distance ada di fighter.cs
            //fungsi ini untuk menentukan/kalkulasi berapa jarak untuk enemy mengejar player
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            //kalau jarak player lebih kecil dari chase distance maka harus kejar playernya
            return distanceToPlayer < chaseDistance;
        }

        //called by unity
        private void OnDrawGizmosSelected() 
        {
            //gizmos API unity Documentation
            Gizmos.color = Color.red;
            //membuat gizmos radius berbentuk wiresphere yang berdasarkan chase distance
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

