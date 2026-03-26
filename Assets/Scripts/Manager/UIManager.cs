using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Script per settare la UI

    //Singleton del UIM
    public static UIManager Instance;

    //i vari menu
    public GameObject pauseMenuUI;

    //gli oggetti UI in HUD
    [SerializeField] Image staminaFill;
    [SerializeField] public Image staminaIconTimer;
    [SerializeField] Image bike_ruota, bike_pedale, bike_manubrio, bike_cestello, bike_sellino;
    [SerializeField] TMP_Text powerCandyCounter;


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
        UpdateCandyCounter();  //...aggiorniamo il counter delle caramelle Poteri
    }
    private void OnDisable()
    {
        PowerCandyManager.Instance.OnCandiesChange -= UpdateCandyCounter;
    }

    public void Start()
    {
        //all'inizio il menu è disattivato
        pauseMenuUI.SetActive(false);

        //il counter di caramelle è a 0
        powerCandyCounter.text = "x0";

        //il timer del freeze è a 0
        staminaIconTimer.fillAmount = 0;
    }

    private void Update()
    {
        staminaFill.fillAmount = Movement.Instance.currentEnergy / (float)Movement.Instance.maxEnergy;
    }

    public void PauseUI()
    {
        //se il menu è attivo
        if (pauseMenuUI.activeSelf)
        {
            //lo disttivo
            pauseMenuUI.SetActive(false);
        }

        else
        {
            //e viceversa
            pauseMenuUI.SetActive(true);
        }
    }

    //funzione per aggiornare la UI delle caramelle
    public void UpdateCandyCounter()
    {
        powerCandyCounter.text = "x" + PowerCandyManager.Instance.currentCandies;
    }
}
