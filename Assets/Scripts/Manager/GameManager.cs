using System;
using UnityEngine;
using UnityEngine.EventSystems;
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

    //oggetti da attivare quando siamo in mod ET
    public GameObject powersLight;
    public GameObject powersGlowStamina;

    //contatore dei pezzi di antenna trovati (Win condition)
    public int antennaPieces = 5; //pezzi da trovare
    public int antennaPiecesFound = 0; //i pezzi trovati

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

        //ci gettiamo l'input map
        inputMap = new InputMap();

        //ci assicuriamo che la luce della mod Poteri sia disattivata all'inizio
        powersLight.SetActive(false);
        powersGlowStamina.SetActive(false);
    }


    void OnEnable()
    {
        inputMap.Enable();
        inputMap.GameStatus.Pause.performed += Pause;
        inputMap.GameStatus.PowerMode.performed += ETMode;
        inputMap.GameStatus.PowerMode.canceled += ETMode;

    }

    void OnDisable()
    {
        inputMap.Disable();
        inputMap.GameStatus.Pause.performed -= Pause;
        inputMap.GameStatus.PowerMode.performed -= ETMode;
        inputMap.GameStatus.PowerMode.canceled -= ETMode;
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
                Time.timeScale = 1;
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else
        {
            SetGameStatus(GameStatus.Running);
            UIManager.Instance.PauseUI();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ETMode(InputAction.CallbackContext context)
    {
        isETing = !isETing;

        //in base se siamo alla modalità poteri, possiamo usarli
        if (isETing)
        {
            SetGameStatus(GameStatus.ETmode);
            powersLight.SetActive(true);
            powersGlowStamina.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //prendiamo tutti gli oggetti in scena che hanno il tag Interactable
            GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

            foreach (GameObject interactable in interactables)
            {
                //e ne attiviamo i figli
                foreach (Transform child in interactable.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

        else
        {
            SetGameStatus(GameStatus.Running);
            powersLight.SetActive(false);
            powersGlowStamina.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //prendiamo tutti gli oggetti in scena che hanno il tag Interactable
            GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

            foreach (GameObject interactable in interactables)
            {
                //e ne disattiviamo i figli
                foreach (Transform child in interactable.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
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

    //metodo per uscire dal gioco
    public void QuitGame()
    {
        Debug.Log("Sei uscito :D");
        Application.Quit();
    }
}
