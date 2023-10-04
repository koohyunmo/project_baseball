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

            Debug.Log("Catch a Ball");
            other.gameObject.SetActive(false);
            Managers.Game.StrikeEvent();
            gameObject.SetActive(true);
            Managers.Game.GameEnd();
            psController.CatchBall();

        }
    }
}
