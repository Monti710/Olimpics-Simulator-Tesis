using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro; // Asegúrate de tener este namespace

public class SceneMan : MonoBehaviour
{
    [Header("Time")]
    public float timeLimit = 60f; // Tiempo total en segundos
    private float timeRemaining; // Tiempo restante
    public TextMeshProUGUI timeText; // Referencia al componente TextMeshPro para el tiempo

    [Header("Shots")]
    public int maxShots = 20; // Número máximo de disparos
    public Shooter shooter; // Referencia al script Shooter para controlar el disparo
    public GameObject shotIconPrefab; // Prefab que representa una bala
    public Transform shotsPanel; // Panel donde se mostrarán los iconos de las balas
    public TextMeshProUGUI shotsText; // Texto que muestra el número de balas restantes
    public float iconSpacing = 30f; // Espacio entre los íconos de las balas (ajustable desde el Inspector)

    // Nuevos campos para el manejo de las filas de balas
    public int maxIconsPerRow = 10; // Máximo de íconos por fila antes de un salto de línea
    public float lineSpacing = 40f; // Espacio entre las filas de balas

    [Header("Points")]
    public TextMeshProUGUI scoreText; // Texto donde se mostrará el puntaje
    public PointCounter pointCounter; // Contador de puntos

    [Header("Scene Loader")]
    public string sceneToLoad;

    void Start()
    {
        // Inicializamos las variables
        timeRemaining = timeLimit;

        // Asignamos el valor de maxShots a Shooter
        shooter.SetMaxShots(maxShots);

        // Aseguramos que el Shooter también maneje las balas
        UpdateShotDisplay(maxShots); // Para actualizar la visualización inicial de las balas
    }

    void Update()
    {
        // Reducimos el tiempo restante
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Reduce el tiempo con el tiempo real que pasa
        }
        else
        {
            timeRemaining = 0; // Aseguramos que el tiempo no sea negativo
            LoadScene(); // Cargamos la escena cuando el tiempo se acaba
        }

        // Actualizamos el texto con el tiempo restante
        UpdateTimeDisplay();
        // Actualizamos el puntaje
        UpdateScoreText();
    }

    // Método para actualizar el TextMeshPro con el tiempo restante
    void UpdateTimeDisplay()
    {
        // Mostramos el tiempo restante en el formato minutos:segundos
        timeText.text = Mathf.Floor(timeRemaining / 60).ToString("00") + ":" + Mathf.Floor(timeRemaining % 60).ToString("00");
    }

    // Método para actualizar la visualización de las balas
    public void UpdateShotDisplay(int currentShots)
    {
        // Limpiamos los iconos de las balas existentes
        foreach (Transform child in shotsPanel)
        {
            Destroy(child.gameObject); // Elimina los iconos de las balas previas
        }

        // Mostramos el número de disparos restantes en el texto
        shotsText.text = "Balas: "+ currentShots.ToString();

        // Variables para el cálculo de la posición de las balas
        float yOffset = 0f; // Distancia vertical para las nuevas filas de balas

        // Instanciamos un icono de bala para cada bala restante
        for (int i = 0; i < currentShots; i++)
        {
            // Instanciamos el prefab de la bala en el panel
            GameObject shotIcon = Instantiate(shotIconPrefab, shotsPanel);

            // Calculamos la posición de cada bala con un pequeño espacio entre ellas
            float xOffset = (i % maxIconsPerRow) * iconSpacing; // Separación en el eje X
            if (i % maxIconsPerRow == 0 && i != 0)  // Si alcanzamos el límite de íconos por fila, hacemos salto de línea
            {
                yOffset -= lineSpacing; // Ajustamos la distancia vertical entre filas
            }
            shotIcon.transform.localPosition = new Vector3(xOffset, yOffset, 0); // Posicionamos con desplazamiento en X y Y
        }

        if (currentShots == 0)
        {
            LoadScene();
        }
    }

    // Método para actualizar el texto del puntaje
    public void UpdateScoreText()
    {
        if (scoreText != null && pointCounter != null)
        {
            scoreText.text = "Puntos: " + pointCounter.GetPoints();
        }
    }

    public void LoadScene()
    {
        PlayerPrefs.SetInt("FinalScore", pointCounter.GetPoints());
        PlayerPrefs.SetString("NextScene", sceneToLoad); // Guarda el destino
        SceneManager.LoadScene("LoadingScene"); // Va a la escena de carga
    }
}
