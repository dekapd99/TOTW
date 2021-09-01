using UnityEngine;

namespace RPG.Resources
{
    public class Experience : MonoBehaviour
    {
        //experience pada unity engine
        [SerializeField] float experiencePoints = 0;

        public void GainExperience(float experience)
        {
            //exp akan terus bertambah seiring player mengalahkan musuhnya
            experiencePoints += experience;
        }
    }
}