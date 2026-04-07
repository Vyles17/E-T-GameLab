using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isWinning = false;

    //oggetti da attivare quando siamo in mod ET
    public GameObject powersLight;
    public GameObject normalHand, powersHand;

    //Input map per gestire i comandi per la UI
    private InputMap inputMap;

    //variabili per la win condition
    [SerializeField] GameObject enemies, assembledAntenna, winSpot;
    private float timerDuration = 3f;

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
        //se abbiamo perso o abbiamo vinto, non possiamo mettere in pausa
        if (isGameOver || isWinning)
            return;

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

        //possiamo entrare in modalità ETing solo se non siamo in pausa, non siamo morti o non abbiamo vinto
        if (!isPaused && !isWinning && !isGameOver)
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
                        interactable.transform.GetChild(0).gameObject.SetActive(true);
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
                        interactable.transform.GetChild(0).gameObject.SetActive(false);
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
                    interactable.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    public void Win()
    {
        //settiamo la variabile
        isWinning = true;

        //freeziamo il movimento del player
        Movement.Instance.FreezeMovement();

        //inizia la prima coroutine
        StartCoroutine(WinningSequence());
    }

    //metodo per ricominciare una partita
    public void RestartGame()
    {
        //resettiamo gli stati
        if (isWinning)
            isWinning = false;
        if (isGameOver)
            isGameOver = false;

        //disattiviamo i pezzi di antenna, se sono rimasti attivi dopo una vincita
        if (assembledAntenna.activeSelf)
            assembledAntenna.SetActive(false);

        //riattiviamo i nemici
        if (!enemies.activeSelf)
            enemies.SetActive(true);

        //sblocchiamo il player, se era bloccato dal game Over/ Win
        Movement.Instance.rb.constraints = RigidbodyConstraints.None;
        Movement.Instance.rb.constraints = RigidbodyConstraints.FreezePositionY;

        //sblocchiamo la timescale
        SetGameStatus(GameStatus.Running);

        // la scena attiva
        Scene currentScene = SceneManager.GetActiveScene();

        // ricarica la scena
        SceneManager.LoadScene(currentScene.name);
    }

    //metodo per il Game Over
    public void GameOver()
    {
        if (isETing)
        {
            ExitETMode();
        }

        isGameOver = true;

        StartCoroutine(FaintingETRoutine());
    }

    IEnumerator FaintingETRoutine()
    {
        //la rotazione iniziale di E.T.
        Vector3 currentEuler = Movement.Instance.rb.rotation.eulerAngles;
        Quaternion startRot = Quaternion.Euler(currentEuler);

        //rotazione a cui arriva E.T. quando sviene
        Quaternion endRot = Quaternion.Euler(currentEuler.x - 90f, currentEuler.y, currentEuler.z);

        Movement.Instance.rb.constraints = RigidbodyConstraints.FreezePosition;

        float time = 0f;

        while (time < timerDuration)
        {
            float t = time / timerDuration;
            Movement.Instance.rb.rotation = Quaternion.Lerp(startRot, endRot, t);

            time += Time.deltaTime;
            yield return null;
        }

        // assicura che arrivi esattamente alla rotazione finale
        Movement.Instance.rb.rotation = endRot;

        //Attiva il panel in UI del Game Over
        UIManager.Instance.gameOverPanel.SetActive(true);

        //settiamo la TimeScale in 0
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetGameStatus(GameStatus.Paused);
    }

    IEnumerator WinningSequence()
    {
        //dopo che spawna l'ultimo pezzo dell'antenna, aspettiamo 3 secondi 

        //(Loris non so se vuoi metterci un mini jingle qua ?)
        yield return new WaitForSeconds(3f);

        //passati i 3 secondi, parte l'animazione della schermata che diventa nera
        UIManager.Instance.blackPanel.SetActive(true);

        float time = 0f;
        Image blackPanelIMG = UIManager.Instance.blackPanel.GetComponent<Image>();
        Color color = blackPanelIMG.color;

        while (time < timerDuration)
        {
            time += Time.deltaTime;
            float opacity = Mathf.Lerp(0f, 1f, time / timerDuration);

            color.a = opacity;
            blackPanelIMG.color = color;

            yield return null;
        }

        // lo schermo rimane nero finchè sposto il mio player e gli resetto la camera
        yield return new WaitForSeconds(2f);

        Movement.Instance.rb.transform.position = winSpot.transform.position;
        Movement.Instance.rb.rotation = winSpot.transform.rotation;

        Movement.Instance.cameraPitch = 0f;
        Movement.Instance.playerCamera.localRotation = Quaternion.Euler(0f, 0f, 0f); 

        //attivo l'antenna assemblata e disattivo i nemici
        assembledAntenna.SetActive(true);
        enemies.SetActive(false);

        //quindi la schermata torna trasparente
        time = 0f;

        while (time < timerDuration)
        {
            time += Time.deltaTime;
            float opacity = Mathf.Lerp(1f, 0f, time / timerDuration);

            color.a = opacity;
            blackPanelIMG.color = color;

            yield return null;
        }

        UIManager.Instance.blackPanel.SetActive(false);

        //(Loris metti qui il rumore dell'antenna segnale)

        yield return new WaitForSeconds(5f);

        //trascorsi questi ultimi secondi, compare il panel di vincita
        UIManager.Instance.winPanel.SetActive(true);

        //settiamo la TimeScale in 0
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SetGameStatus(GameStatus.Paused);
    }

    //metodo per uscire dal gioco
    public void QuitGame()
    {
        Debug.Log("Sei uscito :D");
        Application.Quit();
    }
}
