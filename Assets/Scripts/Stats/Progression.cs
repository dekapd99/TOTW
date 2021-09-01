using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        //agar player memiliki progression jenis classnya sendiri
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;
        
        //untuk mendapatkan basestats character tergantung pada levelnya
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            //foreach progress untuk player 
            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                
                if (progressionClass.characterClass != characterClass)
                
                foreach(ProgressionStat progressionStat in progressionClass.stats)
                {
                    if(progressionStat.stat != stat) continue;
                    
                    if(progressionStat.levels.Length < level) continue;

                    //pengambilan stat berdasarkan levels
                    return progressionStat.levels[level - 1];

                }
            }
            return 0;
        }

        //agar system bisa diatur di unity pada progression scriptableobject
        [System.Serializable]
        class ProgressionCharacterClass
        {
            //class untuk player
            public CharacterClass characterClass;
            //progression stats untuk player
            public ProgressionStat[] stats;
            //jadi setiap naik level akan ada progression terhadap health
            
        }

        //agar system bisa diatur di unity pada progression scriptableobject
        [System.Serializable]
        class ProgressionStat
        {
            //health dan exp points
            public Stat stat;
            public float[] levels;
        }
    }
}