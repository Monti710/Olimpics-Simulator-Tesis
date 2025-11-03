using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class ScoreboardDisplay : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Texto dentro del Content del ScrollView donde se mostrará la lista")]
    public Text contentText; // TMP_Text dentro del Content del ScrollView

    [Tooltip("Componente ScrollRect del ScrollView")]
    public ScrollRect scrollRect;

    [Tooltip("Texto donde se mostrará el puntaje final del PlayerPrefs")]
    public Text finalScoreText;

    [Header("Color de Resaltado")]
    [Tooltip("Color para resaltar el último puntaje agregado")]
    public Color highlightColor = Color.yellow;

    private int finalScore;

    void Start()
    {
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        finalScoreText.text = "Tu Puntaje: " + finalScore;

        MostrarListaDePuntajes();
    }

    void MostrarListaDePuntajes()
    {
        ScoreList scoreList = LocalScoreManager.LoadScores();

        // Caso: JSON vacío o inexistente
        if (scoreList == null || scoreList.scores == null || scoreList.scores.Count == 0)
        {
            contentText.text = "No hay puntajes guardados aún.";
            Debug.LogWarning("⚠ No se encontraron puntajes en el JSON.");
            return;
        }

        // Ordenar por puntaje descendente
        var listaOrdenada = scoreList.scores.OrderByDescending(s => s.score).ToList();
        ScoreData ultimoAgregado = scoreList.scores.Last();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("<b>🏆 Tabla de Puntajes 🏆</b>");
        sb.AppendLine("");

        int indiceUltimo = -1;

        for (int i = 0; i < listaOrdenada.Count; i++)
        {
            ScoreData data = listaOrdenada[i];
            string fecha = string.IsNullOrEmpty(data.date) ? "sin fecha" : data.date;

            if (data.playerName == ultimoAgregado.playerName && data.score == ultimoAgregado.score)
            {
                sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(highlightColor)}><b>{i + 1}. {data.playerName} - {data.score} puntos - {fecha}</b></color>");
                indiceUltimo = i;
            }
            else
            {
                sb.AppendLine($"{i + 1}. {data.playerName} - {data.score} puntos - {fecha}");
            }
        }

        // Mostrar el texto en pantalla
        contentText.text = sb.ToString();

        // Log completo del JSON
        Debug.Log($"✅ Puntajes cargados desde JSON ({listaOrdenada.Count} registros):");
        foreach (var s in listaOrdenada)
        {
            Debug.Log($"• {s.playerName} - {s.score} puntos - {s.date}");
        }

        // Desplazar automáticamente hacia el puntaje resaltado
        if (indiceUltimo != -1)
            StartCoroutine(ScrollToIndex(indiceUltimo, listaOrdenada.Count));
        else if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }

    // Corrutina para centrar el texto resaltado en el ScrollView
    private IEnumerator ScrollToIndex(int index, int total)
    {
        // Esperar un frame para que se actualice el layout
        yield return null;

        if (scrollRect == null)
            yield break;

        // Calcular posición relativa del ítem resaltado
        float normalizedPos = 1f - ((float)index / (float)(total - 1));

        // Asegurar límites válidos
        normalizedPos = Mathf.Clamp01(normalizedPos);

        scrollRect.verticalNormalizedPosition = normalizedPos;

        Debug.Log($"📜 Scroll automático al puntaje #{index + 1} (posición {normalizedPos:F2})");
    }
}
