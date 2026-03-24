using UnityEngine;
using System;


public class AntennaManager : MonoBehaviour
{
    //script manager per tenere conto dei pezzi di antenna trovati

    //contatore dei pezzi di antenna trovati (Win condition)
    public int antennaPieces = 5; //pezzi da trovare
    public int antennaPiecesFound = 0; //i pezzi trovati

    //public event Action OnAntennaChange; //evento per quando viene modificato il counter di caramelle

    public static AntennaManager Instance;

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
        //all'inizio il counter di pezzi di antenna è a 0
        antennaPiecesFound = 0;
    }

    public void AddAntennaPart(int antennaPart)
    {
        //se le parti di antenna che abbiamo sono meno del massimo (quello vorrebbe dire che abbiamo finito il gioco, sennò)
        if (antennaPiecesFound < antennaPieces)
        {
            //aggiunge una caramella al counter
            antennaPiecesFound += antennaPart;

            //OnAntennaChange?.Invoke(); //iscritto all'evento (DA AGGIORNARE APPENA HO LA UI)
        }
    }

    //metodo per quando ci viene rubata/usiamo una caramella Poteri
    public void RemoveAntennaPart(int antennaPart)
    {
        //se abbiamo almeno una caramella
        if (antennaPiecesFound > 0)
        {
            //togliamo una caramella dal counter
            antennaPiecesFound -= antennaPart;

            //OnAntennaChange?.Invoke(); //iscritto all'evento (DA AGGIORNARE APPENA HO LA UI)
        }
    }
}
