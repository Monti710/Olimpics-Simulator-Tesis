using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class SceneMan : MonoBehaviour
{
    [Header("Time")]
    public float timeLimit = 60f;
    private float timeRemaining;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeText1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    [Header("Shots")]
    public int maxShots = 20;
    public Shooter shooter;
    public GameObject shotIconPrefab;
    public Transform shotsPanel;
    public Transform shotsPanel1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    public TextMeshProUGUI shotsText;
    public TextMeshProUGUI shotsText1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    public float iconSpacing = 30f;

    public int maxIconsPerRow = 10;
    public float lineSpacing = 40f;

    [Header("Points")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText1; // <-- ASEGÚRATE DE ASIGNAR ESTO

    public PointCounter pointCounter;

    [Header("Scene Loader")]
    public string sceneToLoad;

    [Header("Scene Delay")]
    public float delayBeforeSceneLoad = 2f; // Delay configurable desde el Inspector

    void Start()
    {
        timeRemaining = timeLimit;
        shooter.SetMaxShots(maxShots);
        UpdateShotDisplay(maxShots);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            timeRemaining = 0;
            LoadScene();
        }

        UpdateTimeDisplay();
        UpdateScoreText();
    }

    void UpdateTimeDisplay()
    {
        timeText.text = Mathf.Floor(timeRemaining / 60).ToString("00") + ":" + Mathf.Floor(timeRemaining % 60).ToString("00");

        // Esta línea ya estaba, solo necesita la asignación en el Inspector
        if (timeText1 != null)
        {
            timeText1.text = Mathf.Floor(timeRemaining / 60).ToString("00") + ":" + Mathf.Floor(timeRemaining % 60).ToString("00");
        }
    }

    public void UpdateShotDisplay(int currentShots)
    {
        foreach (Transform child in shotsPanel)
        {
            Destroy(child.gameObject);
        }

        // Esta lógica ya estaba, solo necesita la asignación en el Inspector
        foreach (Transform child in shotsPanel1)
        {
            Destroy(child.gameObject);
        }

        shotsText.text = "Balas: " + currentShots.ToString();

        // ===== CORRECCIÓN AQUÍ =====
        // Añadimos la actualización para shotsText1
        if (shotsText1 != null)
        {
            shotsText1.text = "Balas: " + currentShots.ToString();
        }
        // ===========================

        float yOffset = 0f;

        for (int i = 0; i < currentShots; i++)
        {
            GameObject shotIcon = Instantiate(shotIconPrefab, shotsPanel);

            // Esta lógica ya estaba, solo necesita la asignación en el Inspector
            GameObject shotIcon1 = Instantiate(shotIconPrefab, shotsPanel1);

            float xOffset = (i % maxIconsPerRow) * iconSpacing;
            if (i % maxIconsPerRow == 0 && i != 0)
            {
                yOffset -= lineSpacing;
            }
            shotIcon.transform.localPosition = new Vector3(xOffset, yOffset, 0);

            // Esta lógica ya estaba, solo necesita la asignación en el Inspector
            shotIcon1.transform.localPosition = new Vector3(xOffset, yOffset, 0);
        }

        if (currentShots == 0)
        {
            StartCoroutine(DelayedSceneLoad());
        }
    }

    public void UpdateScoreText()
    {
        if (scoreText != null && pointCounter != null)
        {
            scoreText.text = "Puntos: " + pointCounter.GetPoints();

            if (scoreText1 != null)
            {
                scoreText1.text = "Puntos: " + pointCounter.GetPoints();
            }
        }
    }

    private IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(delayBeforeSceneLoad);
        LoadScene();
    }

    public void LoadScene()
    {
        PlayerPrefs.SetInt("FinalScore", pointCounter.GetPoints());
        PlayerPrefs.SetString("NextScene", sceneToLoad);
        SceneManager.LoadScene("LoadingScene");
    }
}