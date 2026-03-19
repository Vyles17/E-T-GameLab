using UnityEngine;

public class EnemyDoctor : EnemyMovement
{
    //script figlio per la variabile enemy dello scienziato che ingabbia E.T.

    public Transform cage; // la nostra gabbia
    private bool isCaged = false; //il bool per sapere se è in gabbia

    private void Start()
    {
        isCaged = false; //di base,non è in gabbia quando il gioco comincia
    }
    protected override void Update()
    {
        // prima esegue il comportamento base (movimento)
        base.Update();

        //poi se raggiunge il player
        if (!enemyAgent.pathPending && enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
        {
            //porta il player nella gabbia
            CageET();
        }
    }

    void CageET()
    {
        //gettiamo la posizione della gabbia, ma manteniamo la y del player
        Vector3 cagedETposition = new Vector3(cage.position.x, targetPlayer.position.y, cage.position.z);

        //ci gettiamo il suo rigidbody per "freezarlo" nella gabbia
        Rigidbody playerRB = targetPlayer.GetComponent<Rigidbody>();

        //e lo spostiamo e ingabbiamo
        playerRB.position = cagedETposition;

        //playerRB.constraints = RigidbodyConstraints.FreezeAll; //NON SO SE CI SERVE OPPURE NOOOO chiedere a giulio

        isCaged = true; //DEVO RICORDARMI DI SETTARLO FALSE QUANDO DECIDIAMO COME SI LIBERA
    }

    protected override bool CanChasePlayer()
    {
        // insegue il player solo se non è già in gabbia
        return !isCaged;
    }
}
