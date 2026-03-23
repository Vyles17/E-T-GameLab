using UnityEngine;

public class EnemyThief : EnemyMovement
{
    public int candyToSteal = 1; //caramelle che rubiamo al player
    private float stealCoolDown = 2f;
    public float stealTimer = 0;

    private void Start()
    {
        stealTimer = 0; //settiamo il timer
    }

    protected override void Update()
    {
        // prima esegue il comportamento base (movimento)
        base.Update();

        //se abbiamo derubato E.T., facciamo partire il cooldown
        if (stealTimer > 0)
            stealTimer-= Time.deltaTime;

        //poi se raggiunge il player E il timer è a 0
        if (stealTimer <= 0 && !enemyAgent.pathPending && enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
        {
            //lo derubiamo delle caramelle (evil)
            RobET(candyToSteal);

            //avviamo il cooldown
            stealTimer = stealCoolDown;
        }
    }

    void RobET(int candies)
    {
        //metodo per rubargli una caramella e aggiornare la UI
        PowerCandyManager.Instance.RemoveCandy(candyToSteal);
    }

    protected override bool CanChasePlayer()
    {
        // insegue il player solo se ha caramelle
        return PowerCandyManager.Instance.currentCandies > 0;
    }
}
