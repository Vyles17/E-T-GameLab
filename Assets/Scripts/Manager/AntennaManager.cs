using UnityEngine;
using System;


public class AntennaManager : MonoBehaviour
{
    //script manager per tenere conto dei pezzi di antenna trovati

    //contatore dei pezzi di antenna trovati (Win condition)
    public int antennaPieces = 5; //pezzi da trovare
    public int antennaPiecesFound = 0; //i pezzi trovati

    //SFX
    [SerializeField] AudioClip stealSfx;

    public event Action OnAntennaChange; //evento per quando viene modificato il counter di caramelle

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

    //metodo per quando viene aggiunto un pezzo di antenna
    public void AddAntennaPiece(int antennaPart)
    {
        //se le parti di antenna che abbiamo sono meno del massimo (quello vorrebbe dire che abbiamo finito il gioco, sennò)
        if (antennaPiecesFound < antennaPieces)
        {
            //aggiunge un pezzo di antenna al counter
            antennaPiecesFound += antennaPart;

            OnAntennaChange?.Invoke(); //iscritto all'evento per la UI
        }

    }

    //metodo per quando ci viene rubata/usiamo un pezzo di antenna
    public void RemoveAntennaPiece(int antennaPart)
    {
        //se abbiamo almeno un pezzo di antenna
        if (antennaPiecesFound > 0)
        {
            //togliamo un pezzo di antenna dal counter
            antennaPiecesFound -= antennaPart;

            //faccio partire l'Sfx
            AudioManager.instance.PlaySfx(stealSfx);

            OnAntennaChange?.Invoke(); //iscritto all'evento per la UI
        }
    }
}
