using UnityEngine;
using UnityEngine.UI;

public class MapSelectionController : MonoBehaviour
{
    [Header("Referencias UI")]
    public Button desertButton;  // Botón para el mapa Desierto
    public Button stadiumButton;  // Botón para el mapa Estadio
    public Button sabanaButton;   // Botón para el mapa Sabana

    public Button level1Button;  // Botón para el nivel 1
    public Button level2Button;  // Botón para el nivel 2
    public Button level3Button;  // Botón para el nivel 3

    [Header("Color de Resaltado")]
    public Color selectedColor = Color.blue;  // Color para el botón seleccionado
    private Color originalColor;  // Color original de los botones

    private ScoreboardLoader scoreboardLoader;  // Referencia al script ScoreboardLoader

    private string selectedMap = "DesertMap";  // Valor predeterminado del mapa (se puede cambiar según lo que se seleccione)
    private string selectedLevel = "Level1";  // Valor predeterminado del nivel (se puede cambiar según lo que se seleccione)

    void Start()
    {
        // Obtener la referencia al ScoreboardLoader
        scoreboardLoader = FindObjectOfType<ScoreboardLoader>();

        // Configurar los botones con las funciones correspondientes
        desertButton.onClick.AddListener(() => SelectMap("DesertMap"));
        stadiumButton.onClick.AddListener(() => SelectMap("StadiumMap"));
        sabanaButton.onClick.AddListener(() => SelectMap("SabanaMap"));

        level1Button.onClick.AddListener(() => SelectLevel("Level1"));
        level2Button.onClick.AddListener(() => SelectLevel("Level2"));
        level3Button.onClick.AddListener(() => SelectLevel("Level3"));

        // Inicializar el color original de los botones
        originalColor = desertButton.GetComponent<Image>().color;

        // Establecer los botones de acuerdo con PlayerPrefs
        InitializeButtons();
    }

    // Función que divide el valor de PlayerPrefs y selecciona el botón correcto
    void InitializeButtons()
    {
        string levelScene = PlayerPrefs.GetString("LevelScene", "DesertMap_Level1");  // Obtener el valor de PlayerPrefs

        // Dividir la cadena "DesertMap_Level1" en dos partes (mapa y nivel)
        string[] splitScene = levelScene.Split('_');

        if (splitScene.Length == 2)
        {
            selectedMap = splitScene[0];
            selectedLevel = splitScene[1];

            // Llamar a la función para resaltar el mapa y nivel correspondiente
            SetSelectedButtons();
        }
    }

    // Función para seleccionar el mapa y resaltar el botón correspondiente
    void SelectMap(string mapName)
    {
        // Actualizar el mapa seleccionado y guardar en PlayerPrefs
        selectedMap = mapName;
        PlayerPrefs.SetString("LevelScene", selectedMap + "_" + selectedLevel);

        // Resaltar el botón correspondiente al mapa
        SetSelectedButtons();

        // Actualizar la tabla de puntajes en tiempo real
        if (scoreboardLoader != null)
        {
            scoreboardLoader.MostrarListaDePuntajes();
        }
    }

    // Función para seleccionar el nivel y resaltar el botón correspondiente
    void SelectLevel(string level)
    {
        // Actualizar el nivel seleccionado y guardar en PlayerPrefs
        selectedLevel = level;
        PlayerPrefs.SetString("LevelScene", selectedMap + "_" + selectedLevel);

        // Resaltar el botón correspondiente al nivel
        SetSelectedButtons();

        // Actualizar la tabla de puntajes en tiempo real
        if (scoreboardLoader != null)
        {
            scoreboardLoader.MostrarListaDePuntajes();
        }
    }

    // Función para resaltar el botón del mapa y el nivel seleccionados
    void SetSelectedButtons()
    {
        // Desmarcar todos los botones
        DeselectAllButtons();

        // Resaltar el botón correspondiente al mapa
        Button selectedMapButton = GetMapButton(selectedMap);
        if (selectedMapButton != null)
        {
            selectedMapButton.GetComponent<Image>().color = selectedColor;
        }

        // Resaltar el botón correspondiente al nivel
        Button selectedLevelButton = GetLevelButton(selectedLevel);
        if (selectedLevelButton != null)
        {
            selectedLevelButton.GetComponent<Image>().color = selectedColor;
        }
    }

    // Función para desmarcar todos los botones (restaurar color original)
    void DeselectAllButtons()
    {
        desertButton.GetComponent<Image>().color = originalColor;
        stadiumButton.GetComponent<Image>().color = originalColor;
        sabanaButton.GetComponent<Image>().color = originalColor;
        level1Button.GetComponent<Image>().color = originalColor;
        level2Button.GetComponent<Image>().color = originalColor;
        level3Button.GetComponent<Image>().color = originalColor;
    }

    // Obtener el botón correspondiente al mapa seleccionado
    Button GetMapButton(string mapName)
    {
        switch (mapName)
        {
            case "DesertMap":
                return desertButton;
            case "StadiumMap":
                return stadiumButton;
            case "SabanaMap":
                return sabanaButton;
            default:
                return null;
        }
    }

    // Obtener el botón correspondiente al nivel seleccionado
    Button GetLevelButton(string level)
    {
        switch (level)
        {
            case "Level1":
                return level1Button;
            case "Level2":
                return level2Button;
            case "Level3":
                return level3Button;
            default:
                return null;
        }
    }
}
