using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        //membuat sebuah slider pada unity 1-99
        [Range(1, 99)]
        //base level dari character
        [SerializeField] int startingLevel = 1;
        //class dari character si player
        [SerializeField] CharacterClass characterClass;
        //progression scriptableobject
        [SerializeField] Progression progression = null;

        
        public float GetStat(Stat stat)
        {
            //base stat awal berdasarkan stat, class dan level
            return progression.GetStat(stat, characterClass, startingLevel);
        }
    } 
}