using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameStatus
{
    Running,
    Paused,
    ETmode, //status per quando stiamo usando i poteri
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
    [HideInInspector] public bool isTutorial = true;
    public GameStatus status;

    //Musics (menu e level)
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] float musicDuration;
    [SerializeField] float gameOverJDuration;
    [SerializeField] float winJDuration;
    [SerializeField] bool isPlaying = false;

    //oggetti da attivare quando siamo in mod ET
    GameObject[] interactables => GameObject.FindGameObjectsWithTag("Interactable");
    public GameObject powersLight;
    public GameObject normalHand, powersHand;
    //distanza a cui vogliamo rendere interagibile gli oggetti
    public float enemyInteractionDistance = 15f;
    public float objectInteractionDistance = 5f;

    //Input map per gestire i comandi per la UI
    private InputMap inputMap;

    //variabili per la win condition
    [SerializeField] GameObject enemies, assembledAntenna, winSpot;
    [SerializeField] AudioClip signalSfx, winSfx, WinJingle;
    private float timerDuration = 2.5f;

    //GameOver
    [SerializeField] AudioClip GameOverSfx, GameoverJingle;
    bool played = false;

    //BTN sfx
    [SerializeField] AudioClip clickSfx;

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
    private void Start()
    {
        StartCoroutine(bgMusic());

        //se siamo nella prima schermata main, non possiamo mettere in pausa
        Scene mainMenu = SceneManager.GetSceneByBuildIndex(0);
        Scene mainLevel = SceneManager.GetSceneByBuildIndex(1);
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene == mainMenu)
            Cursor.visible = true;

        else if (currentScene == mainLevel)
        {
            UIManager.Instance.tutorialPanel.SetActive(true);
            isTutorial = true;
        }
    }

    private void Update()
    {
        if (isTutorial)
            SetGameStatus(GameStatus.Paused);

        if (isETing)
        {
            foreach (GameObject interactable in interactables)
            {
                //calcolo se il player è abbastanza vicino per interagire con l'oggetto
                float dist = Vector3.Distance(interactable.transform.position, Movement.Instance.transform.position);


                //se lo è (in base al tipo di oggetto) attivo il figlio col componente del particle system
                if (interactable.TryGetComponent<EnemyMovement>(out var enemy))
                {
                    if (dist < enemyInteractionDistance)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else if (dist > enemyInteractionDistance)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }

                else if (interactable.TryGetComponent<TelekinesisInteractableSpawner>(out var obj))
                {
                    if (dist < objectInteractionDistance)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else if (dist > objectInteractionDistance)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }

                else if (interactable.TryGetComponent<BloomingInteractableSpawner>(out var obj2))
                {
                    if (dist < objectInteractionDistance)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else if (dist > objectInteractionDistance)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    public void SetGameStatus(GameStatus status)
    {
        //metodo per settare lo stato di gioco, switchandolo da uno all'altro
        switch (status)
        {
            case GameStatus.Running:
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;

            case GameStatus.Paused:
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            case GameStatus.ETmode:
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        //abbiamo perso o abbiamo vinto, non possiamo mettere in pausa
        if (isGameOver || isWinning || isTutorial)
            return;

        //se siamo nella prima schermata main, non possiamo mettere in pausa
        Scene mainMenu = SceneManager.GetSceneByBuildIndex(0);
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene == mainMenu)
            return;

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
        // siamo in pausa, abbiamo perso o abbiamo vinto, non possiamo mettere in mod poteri
        if (isGameOver || isWinning || isPaused || isTutorial)
            return;

        //se siamo nella prima schermata main, non possiamo mettere in mod poteri
        Scene mainMenu = SceneManager.GetSceneByBuildIndex(0);
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene == mainMenu)
            return;

        isETing = !isETing;


        //in base se siamo alla modalità poteri, possiamo usarli
        if (isETing)
        {
            SetGameStatus(GameStatus.ETmode);
            powersLight.SetActive(true);
            normalHand.SetActive(false);
            powersHand.SetActive(true);

            foreach (GameObject interactable in interactables)
            {
                //e ne attiviamo il figlio powermode
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
            normalHand.SetActive(true);
            powersHand.SetActive(false);

            foreach (GameObject interactable in interactables)
            {
                //e ne disattiviamo i figli  (che è l'oggetto "powerMode" e l'oggetto "nearDistance Power Mode")
                foreach (Transform child in interactable.transform)
                {
                    interactable.transform.GetChild(0).gameObject.SetActive(false);
                    if (interactable.transform.childCount > 1)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void GameRunning()
    {
        isTutorial = false;
        SetGameStatus(GameStatus.Running);
    }

    //mini metodo che mi serve per forzare l'uscita dalla modalità ET
    public void ExitETMode()
    {
        if (!isETing)
            return;

        if (isETing)
        {
            //setto lo stato
            isETing = false;

            SetGameStatus(GameStatus.Running);

            //disattivo le varie luci / cursori, ecc
            powersLight.SetActive(false);
            normalHand.SetActive(true);
            powersHand.SetActive(false);

            foreach (GameObject interactable in interactables)
            {
                //e ne disattiviamo i figli (che è l'oggetto "powerMode" e l'oggetto "nearDistance Power Mode")
                foreach (Transform child in interactable.transform)
                {
                    interactable.transform.GetChild(0).gameObject.SetActive(false);
                    if (interactable.transform.childCount > 1)
                    {
                        interactable.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void Win()
    {
        //ferma la BgMusic
        Destroy(MusicManager.instance.audioSource);

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
        // la scena con il livello di gioco
        Scene mainScene = SceneManager.GetActiveScene();

        // ricarica la scena
        SceneManager.LoadScene(mainScene.name);
    }
    public void StartLevel()
    {
        SceneManager.LoadScene(1);
    }
    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //metodo per il Game Over
    public void GameOver()
    {
        //ferma la BgMusic
        Destroy(MusicManager.instance.audioSource);

        if (isETing)
        {
            ExitETMode();
        }

        isGameOver = true;

        StartCoroutine(FaintingETRoutine());
    }

    IEnumerator FaintingETRoutine()
    {
        if (!played)
            AudioManager.instance.PlaySfx(GameOverSfx);

        played = true;

        //la rotazione iniziale di E.T.
        Quaternion startRot = Movement.Instance.rb.rotation;

        //rotazione a cui arriva E.T. quando sviene
        Quaternion endRot = startRot * Quaternion.Euler(-40, 0, 0);

        //blocco il player
        Movement.Instance.rb.constraints = RigidbodyConstraints.FreezePosition;


        float time = 0f;

        while (time < 2)
        {
            //lo faccio svenire
            time += Time.deltaTime;
            float t = time / 2;
            Movement.Instance.rb.rotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        // assicura che arrivi esattamente alla rotazione finale
        Movement.Instance.rb.rotation = endRot;


        //parte l'animazione della schermata che diventa nera
        UIManager.Instance.blackPanel.SetActive(true);

        time = 0f;
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

        //disattivo i nemici e la in game UI
        enemies.SetActive(false);
        UIManager.Instance.inGameUI.SetActive(false);

        //faccio partire la corutine per ciclare la musica
        StartCoroutine(GameOverJingles());

        //Attivo il panel in UI del Game Over
        UIManager.Instance.gameOverPanel.SetActive(true);

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
        //rendo visibile e delockko il cursore
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIManager.Instance.blackPanel.SetActive(false);
    }

    IEnumerator WinningSequence()
    {
        //dopo che spawna l'ultimo pezzo dell'antenna, aspettiamo 3 secondi 

        //(Loris non so se vuoi metterci un mini jingle qua ?)
        yield return new WaitForSeconds(3f);
        AudioManager.instance.PlaySfx(winSfx);

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

        //faccio partire l'SFX
        AudioManager.instance.PlaySfx(signalSfx);

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

        //(Loris metti qui il rumore dell'antenna segnale)

        //LORIS se ti serve più tempo per la musichetta cambia il valore qui sotto
        yield return new WaitForSeconds(5f);

        //trascorsi questi ultimi secondi, la schermata torna nera
        time = 0f;

        while (time < timerDuration)
        {
            time += Time.deltaTime;
            float opacity = Mathf.Lerp(0f, 1f, time / timerDuration);

            color.a = opacity;
            blackPanelIMG.color = color;

            yield return null;
        }

        //attivo il panel di vittoria e disattivo l'in game UI
        UIManager.Instance.winPanel.SetActive(true);
        UIManager.Instance.inGameUI.SetActive(false);

        //once again il black panel torna trasparente, poi lo disattivo
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
        //faccio partire la coroutine per ciclare la musica
        StartCoroutine(WinJingles());

        //aspetto che trascorra l'animazione
        yield return new WaitForSeconds(5f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //attivo il testo della schermata finale
        UIManager.Instance.winText.SetActive(true);
    }
    IEnumerator bgMusic()
    {
        while (true)
        {
            if (!isPaused)
            {
                MusicManager.instance.PlayBg(backgroundMusic);
                if (isWinning || isGameOver)
                {
                    Destroy(MusicManager.instance.audioSource);
                }
            }
            yield return new WaitForSeconds(musicDuration);
        }
    }
    IEnumerator GameOverJingles()
    {
        while (!isPlaying)
        {
            isPlaying = true;
            MusicManager.instance.PlayFinal(GameoverJingle);
            yield return new WaitForSeconds(gameOverJDuration);
            isPlaying = false;
        }
    }
    IEnumerator WinJingles()
    {
        while (!isPlaying)
        {
            isPlaying = true;
            MusicManager.instance.PlayFinal(WinJingle);
            yield return new WaitForSeconds(winJDuration);
            isPlaying = false;
        }
    }

    public void ClickSfx()
    {
        //faccio partire l'SFX
        AudioManager.instance.PlaySfx(clickSfx);
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //metodo per uscire dal gioco
    public void QuitGame()
    {
        Application.Quit();
    }
}
