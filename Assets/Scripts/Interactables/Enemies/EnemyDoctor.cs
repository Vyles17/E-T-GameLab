using UnityEngine;

public class EnemyDoctor : EnemyMovement
{
    //script figlio per la variabile enemy dello scienziato che ingabbia E.T.

    public Transform cage; // la nostra gabbia
    private bool isCaged = false; //il bool per sapere se è in gabbia
    [SerializeField] AudioClip cageSfx;

    
    protected override void Update()
    {
        if (!isCaged && !GameManager.Instance.isTutorial)
        {
            //esegue il comportamento base (movimento)
            base.Update();

            //se raggiunge il player
            if (!enemyAgent.pathPending && enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                //porta il player nella gabbia
                CageET();

                //appena ha messo al gabbio E.T., torna al waypoint della casa
                enemyAgent.SetDestination(baseWaypoint.position);
            }
        }

        else
        {
            //se l'enemy passa per il waypoint casa madre, torna a inseguire E.T.
            if (!enemyAgent.pathPending && enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                isCaged = false;
            }
        }
    }

    private void CageET()
    {
        AudioManager.instance.PlaySfx(cageSfx);

        //gettiamo la posizione della gabbia, ma manteniamo la y del player
        Vector3 cagedETposition = new(cage.position.x, targetPlayer.position.y, cage.position.z);

        //ci gettiamo il suo rigidbody per "freezarlo" nella gabbia
        Rigidbody playerRB = targetPlayer.GetComponent<Rigidbody>();

        //e lo spostiamo e ingabbiamo
        playerRB.position = cagedETposition;
        isCaged = true;
    }

    protected override bool CanChasePlayer()
    {
        // insegue il player solo se non l'ha appena ingabbiato
        return !isCaged;
    }
}
