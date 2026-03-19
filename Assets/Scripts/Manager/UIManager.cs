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
    [SerializeField] Image staminaFill, powersFill;
    [SerializeField] Image staminaIcon;
    [SerializeField] Image bike_ruota, bike_pedale, bike_manubrio, bike_cestello, bike_sellino;
    [SerializeField] Button phoneButton;

    PowerCandyManager PCM;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        PCM = FindAnyObjectByType<PowerCandyManager>();
    }

    private void OnEnable()
    {
        PCM.OnCandiesChange += UpdateCandyBar; //quando viene chiamato l'evento...
        UpdateCandyBar();  //...aggiorniamo la barra delle caramelle Poteri
    }
    private void OnDisable()
    {
        PCM.OnCandiesChange -= UpdateCandyBar;
    }

    public void Start()
    {
        //all'inizio il menu è disattivato
        pauseMenuUI.SetActive(false);
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
    public void UpdateCandyBar()
    {
        powersFill.fillAmount = (float)PCM.currentCandies / PCM.candiesMaxCapacity;
    }
}
