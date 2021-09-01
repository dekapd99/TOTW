using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour 
    {
        IAction currentAction;
        // fixing bug dari Fighter/Attack --> BUG: kalau klik enemy akan lock target 
        // fixing bug dari Mover/StartMoveAction --> BUG: kalau klik area kosong/kabur maka akan kembali ke target
        public void StartAction(IAction action)
        {
            //canceling setelah mover
            if (currentAction == action) return;
            //stop canceling setiap saat ada enemy
            if (currentAction != null)
            {
                //canceling animasi
                //kalau untuk menyerang enemy maka akan ke method Cancel yang ada di Fighter
                //kalau untuk bergerak ke area kosong/kabur maka akan ke method Cancel yang ada di Mover
                currentAction.Cancel();
            }
            //action sekarang = action yang baru dijalanin
            currentAction = action;
        }
        //untuk cancelling segala bentuk aksi yang dilakukan enemy dan player saat sudah mati
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
