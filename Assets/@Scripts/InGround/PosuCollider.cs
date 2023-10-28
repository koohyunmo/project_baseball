using UnityEngine;

public class PosuCollider : MonoBehaviour
{

    public PosuController psController;

    private void Start()
    {
        psController = GetComponentInParent<PosuController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Ball"))
        {

            if (Managers.Game.GameState == Define.GameState.InGround || Managers.Game.GameState == Define.GameState.End)
            {
                if (Managers.Game.GameScore == 0)
                {
                    Managers.Game.HitEvent();
                    return;
                }
                   

                other.gameObject.SetActive(false);
                Managers.Game.StrikeEvent();
                gameObject.SetActive(true);
                Managers.Game.GameEnd();
                psController.CatchBall();
            }

        }
    }
}