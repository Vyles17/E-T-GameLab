using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool isETing = false;

    //oggetti da attivare quando siamo in mod ET
    public GameObject powersLight;
    public GameObject normalHand, powersHand;

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

        //possiamo entrare in modalità ETing solo se non siamo in pausa
        if (!isPaused)
        {
            //in base se siamo alla modalità poteri, possiamo usarli
            if (isETing)
            {
                SetGameStatus(GameStatus.ETmode);
                powersLight.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                normalHand.SetActive(false);
                powersHand.SetActive(true);

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
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                normalHand.SetActive(true);
                powersHand.SetActive(false);

                //prendiamo tutti gli oggetti in scena che hanno il tag Interactable
                GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

                foreach (GameObject interactable in interactables)
                {
                    //e ne disattiviamo i figli  (che è l'oggetto "powerMode")
                    foreach (Transform child in interactable.transform)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }

    }

    //mini metodo che mi serve per forzare l'uscita dalla modalità ET
    public void ExitETMode()
    {
        if (isETing)
        {
            //setto lo stato
            isETing = false;

            SetGameStatus(GameStatus.Running);

            //disattivo le varie luci / cursori, ecc
            powersLight.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            normalHand.SetActive(true);
            powersHand.SetActive(false);

            //prendiamo tutti gli oggetti in scena che hanno il tag Interactable
            GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

            foreach (GameObject interactable in interactables)
            {
                //e ne disattiviamo i figli (che è l'oggetto "powerMode")
                foreach (Transform child in interactable.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

    }

    //metodo per ricominciare una partita
    public void RestartGame()
    {
        // la scena attiva
        Scene currentScene = SceneManager.GetActiveScene();

        // ricarica la scena
        SceneManager.LoadScene(currentScene.name);
    }

    //metodo per uscire dal gioco
    public void QuitGame()
    {
        Debug.Log("Sei uscito :D");
        Application.Quit();
    }
}
