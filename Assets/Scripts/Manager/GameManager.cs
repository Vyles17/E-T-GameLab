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

    //oggetti da attivare quando siamo in mod ET
    public GameObject powersLight;
    public GameObject powersGlowStamina;

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

        //ci gettiamo l'input map
        inputMap = new InputMap();

        //ci assicuriamo che la luce della mod Poteri sia disattivata all'inizio
        powersLight.SetActive(false);
        powersGlowStamina.SetActive(false);
    }
    void Start()
    {
      
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
            Cursor.visible = true;
            SetGameStatus(GameStatus.Paused);
            UIManager.Instance.PauseUI();
        }

        else
        {
            Cursor.visible = false;
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
            powersLight.SetActive(true);
            powersGlowStamina.SetActive(true);

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

    //metodo per uscire dal gioco
    private void QuitGame()
    {
        Application.Quit();
    }
}
