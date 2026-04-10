using System;
using UnityEngine;

public class PowerCandyManager : MonoBehaviour
{
    //script manager per tenere conto delle caramelle potere del player

    public int candiesMaxCapacity; //quante caramelle potere può trasportare il player
    public float currentCandies; //quante caramelle ha attualmente il player

    public event Action OnCandiesChange; //evento per quando viene modificato il counter di caramelle


    public static PowerCandyManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void Start()
    {
        //all'inizio il counter di caramelle è a 0
        currentCandies = 0;
    }

    //metodo per quando raccogliamo una caramella Poteri
    public void AddCandy(int candy)
    {
        //se le caramelle che abbiamo sono meno di quelle Max che possiamo trasportare
        if (currentCandies < candiesMaxCapacity)
        {
            //aggiunge una caramella al counter
            currentCandies += candy;

            OnCandiesChange?.Invoke(); //iscritto all'evento
        }
    }

    //metodo per quando ci viene rubata/usiamo una caramella Poteri
    public void RemoveCandy(int candy)
    {
        //se abbiamo almeno una caramella
        if (currentCandies > 0)
        {
            //togliamo una caramella dal counter
            currentCandies-= candy;
            OnCandiesChange?.Invoke(); //iscritto all'evento
        }
    }
}
