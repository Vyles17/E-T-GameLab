using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GameStatus
{
    Running,
    Paused,
    ETmode //status per quando stiamo usando i poteri
}

public class GameManager : MonoBehaviour
{
    //Script per settare gli stati del gioco

    //Singleton del GM
    public static GameManager Instance;

    //bools per gli stati di gioco
    private bool isPaused = false;
    private bool isETing = false;

    //contatore dei pezzi di bici trovati (Win condition)
    public int bikePieces = 5; //pezzi da trovare
    public int bikePiecesFound = 0; //i pezzi trovati

    //Input map per gestire i comandi per la UI
    private InputMap inputMap;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        inputMap = new InputMap();
    }

    void OnEnable()
    {
        inputMap.Enable();
        inputMap.GameStatus.Pause.performed += Pause;
        inputMap.GameStatus.PowerMode.performed += ETMode;
    }

    void OnDisable()
    {
        inputMap.Disable();
        inputMap.GameStatus.Pause.performed -= Pause;
        inputMap.GameStatus.PowerMode.performed -= ETMode;
    }

    public void SetGameStatus(GameStatus status)
    {
        //metodo per settare lo stato di gioco, switchandolo da uno all'altro
        switch (status)
        {
            case GameStatus.Running:
                Time.timeScale = 1;
                break;

            case GameStatus.Paused:
                Time.timeScale = 0;
                break;

            case GameStatus.ETmode:
                Time.timeScale = 0;
                break;
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        //avvio/tolgo il menu di pausa in base allo stato del gioco
        if (isPaused)
        {
            SetGameStatus(GameStatus.Paused);
            UIManager.Instance.PauseUI();
        }

        else
        {
            SetGameStatus(GameStatus.Running);
            UIManager.Instance.PauseUI();
        }
    }

    //non badare a questo doppio metodo Loris, non posso usare l'altro per il bottone del Telefono rip
    public void PausePhone()
    {
        isPaused = !isPaused;

        //avvio/tolgo il menu di pausa in base allo stato del gioco
        if (isPaused)
        {
            SetGameStatus(GameStatus.Paused);
            UIManager.Instance.PauseUI();
        }

        else
        {
            SetGameStatus(GameStatus.Running);
            UIManager.Instance.PauseUI();
        }
    }

    public void ETMode(InputAction.CallbackContext context)
    {
        isETing = !isETing;

        //in base se siamo alla modalità poteri, possiamo usarli
        if (isETing)
        {
            SetGameStatus(GameStatus.ETmode);
            //aggiungere qui i poteri
        }
        else
        {
            SetGameStatus(GameStatus.Running);
            //qui disabilitare i poteri
        }
    }

    //metodo per uscire dal gioco
    private void QuitGame()
    {
        Application.Quit();
    }
}
