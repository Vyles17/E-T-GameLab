using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Script per settare la UI

    //Singleton del UIM
    public static UIManager Instance;

    //i vari menu
    public GameObject tutorialPanel, pauseMenuUI1, pauseMenuUI2, pauseMenuUI3, winPanel, gameOverPanel, blackPanel;

    //gli oggetti UI in HUD
    [SerializeField] Image staminaFill;
    [SerializeField] Image freezedStaminaFill;
    public GameObject staminaOutline;
    private Sprite defaultStaminaSprite;
    public Image staminaIconTimer;
    [SerializeField] GameObject antennaPiece1, antennaPiece2, antennaPiece3, antennaPiece4, antennaPiece5;
    [SerializeField] TMP_Text powerCandyCounter;
    public RectTransform handET;
    public RectTransform powerHandET;
    public GameObject inGameUI, winText;

    //per il lerp della mano
    private Vector2 handStartPos;
    private Vector2 handEndPos;
    private Vector2 powerHandStartPos;
    private Vector2 powerHandEndPos;
    private float animDuration = 1f;
    private float powerAnimDuration = 0.3f;
    private float handTimer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        PowerCandyManager.Instance.OnCandiesChange += UpdateCandyCounter; //quando viene chiamato l'evento...
        AntennaManager.Instance.OnAntennaChange += UpdateAntennaPieces;
    }
    private void OnDisable()
    {
        PowerCandyManager.Instance.OnCandiesChange -= UpdateCandyCounter;
        AntennaManager.Instance.OnAntennaChange -= UpdateAntennaPieces;
    }

    public void Start()
    {
        tutorialPanel.SetActive(true);
        pauseMenuUI1.SetActive(false);
        pauseMenuUI2.SetActive(false);
        pauseMenuUI3.SetActive(false);
        blackPanel.SetActive(false);

        //il timer del freeze è a 0
        staminaIconTimer.fillAmount = 0;
        defaultStaminaSprite = staminaFill.sprite;

        //i pezzi di antenna sono vuoti
        antennaPiece1.SetActive(false);
        antennaPiece2.SetActive(false);
        antennaPiece3.SetActive(false);
        antennaPiece4.SetActive(false);
        antennaPiece5.SetActive(false);

        //settiamo le posizioni per l'animazione della mano
        handStartPos = handET.anchoredPosition;
        powerHandStartPos = powerHandET.anchoredPosition;
        handEndPos = new Vector2(handStartPos.x - 10, handStartPos.y - 20);
        powerHandEndPos = new Vector2(powerHandStartPos.x - 15, powerHandStartPos.y);
    }

    private void Update()
    {
        UpdateCandyCounter();
        UpdateAntennaPieces();

        //se ho la stamina freezata, sostituisco la sprite
        if (Movement.Instance.freezed)
        {
            staminaFill.sprite = freezedStaminaFill.sprite;
            freezedStaminaFill.fillAmount = Movement.Instance.currentEnergy / (float)Movement.Instance.maxEnergy;
        }

        else if (!Movement.Instance.freezed)
        {
            staminaFill.sprite = defaultStaminaSprite;
            staminaFill.fillAmount = Movement.Instance.currentEnergy / (float)Movement.Instance.maxEnergy;
        }

        if (!GameManager.Instance.isETing && !GameManager.Instance.isPaused)
        {
            //lerp per l'animazione della manina di ET (ondeggia su e giu in loop)
            handTimer += Time.deltaTime;
            float anim = Mathf.PingPong(handTimer / animDuration, 1f);

            handET.anchoredPosition = Vector2.Lerp(handStartPos, handEndPos, anim);
        }

        else if (GameManager.Instance.isETing == true)
        {
            //lerp per l'animazione della manina di ET in PowerMode (vibra sull'asse X)
            handTimer += Time.deltaTime;
            float powerAnim = Mathf.PingPong(handTimer / powerAnimDuration, 1f);

            powerHandET.anchoredPosition = Vector2.Lerp(powerHandStartPos, powerHandEndPos, powerAnim);
        }
    }

    public void PauseUI()
    {
        //se il menu è attivo
        if (pauseMenuUI1.activeSelf || pauseMenuUI2.activeSelf || pauseMenuUI3.activeSelf)
        {
            //lo disttivo
            pauseMenuUI1.SetActive(false);
            pauseMenuUI2.SetActive(false);
            pauseMenuUI3.SetActive(false);
        }

        else if (!pauseMenuUI1.activeSelf && !pauseMenuUI2.activeSelf && !pauseMenuUI3.activeSelf)
        {
            //e viceversa
            pauseMenuUI1.SetActive(true);
        }
    }

    //funzione per aggiornare la UI delle caramelle
    public void UpdateCandyCounter()
    {
        powerCandyCounter.text = "x" + PowerCandyManager.Instance.currentCandies;
    }

    //metodo per aggiornare i pezzi di antenna in UI
    public void UpdateAntennaPieces()
    {
        if (AntennaManager.Instance.antennaPiecesFound == 1)
        {
            antennaPiece1.SetActive(true);
            antennaPiece2.SetActive(false);
            antennaPiece3.SetActive(false);
            antennaPiece4.SetActive(false);
            antennaPiece5.SetActive(false);
        }

        else if (AntennaManager.Instance.antennaPiecesFound == 2)
        {
            antennaPiece1.SetActive(true);
            antennaPiece2.SetActive(true);
            antennaPiece3.SetActive(false);
            antennaPiece4.SetActive(false);
            antennaPiece5.SetActive(false);
        }

        else if (AntennaManager.Instance.antennaPiecesFound == 3)
        {
            antennaPiece1.SetActive(true);
            antennaPiece2.SetActive(true);
            antennaPiece3.SetActive(true);
            antennaPiece4.SetActive(false);
            antennaPiece5.SetActive(false);
        }

        else if (AntennaManager.Instance.antennaPiecesFound == 4)
        {
            antennaPiece1.SetActive(true);
            antennaPiece2.SetActive(true);
            antennaPiece3.SetActive(true);
            antennaPiece4.SetActive(true);
            antennaPiece5.SetActive(false);
        }

        else if (AntennaManager.Instance.antennaPiecesFound == 5)
        {
            antennaPiece1.SetActive(true);
            antennaPiece2.SetActive(true);
            antennaPiece3.SetActive(true);
            antennaPiece4.SetActive(true);
            antennaPiece5.SetActive(true);
        }

        else
        {
            antennaPiece1.SetActive(false);
            antennaPiece2.SetActive(false);
            antennaPiece3.SetActive(false);
            antennaPiece4.SetActive(false);
            antennaPiece5.SetActive(false);
        }
    }
}
