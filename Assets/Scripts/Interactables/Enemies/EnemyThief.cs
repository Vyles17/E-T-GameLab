using UnityEngine;

public class EnemyThief : EnemyMovement
{
    public int stuffToSteal = 1; //roba che rubiamo al player
    private bool isRobbed;

    private void Start()
    {
        isRobbed = false;
    }

    protected override void Update()
    {
        if (!isRobbed)
        {
            //esegue il comportamento base (movimento)
            base.Update();

            //se raggiunge il player
            if (!enemyAgent.pathPending && enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                //arruba i possedimenti
                RobET(stuffToSteal);

                //appena ha arrubbato E.T., torna al waypoint della casa
                enemyAgent.SetDestination(baseWaypoint.position);
            }
        }

        else
        {
            //se l'enemy passa per il waypoint casa madre, torna a inseguire E.T.
            if (!enemyAgent.pathPending && enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                isRobbed = false;
            }
        }
    }

    void RobET(int stuff)
    {
        //se il player ha dei pezzi dell'antenna
        if (AntennaManager.Instance.antennaPiecesFound > 0)
        {
            //sto ladro piezzemmerd gli ruba uno di quelli
            AntennaManager.Instance.RemoveAntennaPiece(stuff);

            isRobbed = true;
        }

        //sennò gli arruba le caramelle come a un bebè
        else
        {
            //metodo per rubargli una caramella e aggiornare la UI
            PowerCandyManager.Instance.RemoveCandy(stuff);

            isRobbed = true;
        }
    }

    protected override bool CanChasePlayer()
    {
        // insegue il player solo se pezzi dell'antenna o delle caramelle
        return PowerCandyManager.Instance.currentCandies > 0 || AntennaManager.Instance.antennaPiecesFound > 0;
    }
}
