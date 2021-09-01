using UnityEngine;
using RPG.Resources;

namespace RPG.Combat
{
    //pada saat kita add component combat target, maka script Health akan ke add secara otomatis
    //karena sekarang Health.cs bergantung kepada Combat Target
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour 
    {
        
    }
}