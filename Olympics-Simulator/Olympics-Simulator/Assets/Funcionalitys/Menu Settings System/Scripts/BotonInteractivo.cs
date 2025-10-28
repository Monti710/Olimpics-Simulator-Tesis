using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections; // Necesario para usar Corutinas

public class BotonInteractivoVR : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- Variables Configurables en el Inspector ---
    [Header("Control de Velocidad")]
    [Tooltip("Controla la velocidad de la animación. Un valor más alto es más rápido. Sugerido: 8 a 15.")]
    public float velocidadDeTransicion = 10f;

    [Header("Referencias UI")]
    [Tooltip("Arrastra aquí los RectTransform adicionales que quieres modificar su ANCHO (Imágenes, Paneles, etc.)")]
    public List<RectTransform> componentesAdicionalesParaModificarWidth;
    [Tooltip("Arrastra aquí el TextMeshPro que quieres mostrar/ocultar")]
    public Text textoDescriptivo;

    [Header("Componente con Tamaño/Posición Variable")]
    public RectTransform componenteTamanoPosicionVariable;
    public float aumentoAnchoVariable = 0f;
    public float aumentoAltoVariable = 0f;
    public float cambioPosXVariable = 0f;
    public float cambioPosYVariable = 0f;
    public float cambioPosZVariable = 0f;

    [Header("Modificación de Ancho (Width) Estándar")]
    public float anchoAumentoEstandar = 50f;

    // --- Variables Internas ---
    private RectTransform botonRectTransform;
    private Button botonComponente;

    // Variables de Estado (Objetivos)
    private bool estaHighlighted = false;

    // Variables de Tamaño y Posición Originales
    private float anchoOriginalBoton;
    private Dictionary<RectTransform, float> anchosOriginalesAdicionales = new Dictionary<RectTransform, float>();
    private Vector2 sizeDeltaOriginalVariable;
    private Vector3 posicionOriginalVariable;

    // Variables de Tamaño y Posición Objetivo (Final del Lerp)
    private float anchoTargetBoton;
    private Dictionary<RectTransform, float> anchosTargetAdicionales = new Dictionary<RectTransform, float>();
    private Vector2 sizeDeltaTargetVariable;
    private Vector3 posicionTargetVariable;

    // Referencia a la corutina para detenerla si es necesario
    private Coroutine controlTextoCoroutine;

    //---------------------------------------------------------

    void Awake()
    {
        // 1. OBTENER REFERENCIAS Y GUARDAR ORIGINALES
        botonRectTransform = GetComponent<RectTransform>();
        botonComponente = GetComponent<Button>();

        if (botonRectTransform != null) anchoOriginalBoton = botonRectTransform.sizeDelta.x;

        foreach (RectTransform rect in componentesAdicionalesParaModificarWidth)
        {
            if (rect != null) anchosOriginalesAdicionales.Add(rect, rect.sizeDelta.x);
        }

        if (componenteTamanoPosicionVariable != null)
        {
            sizeDeltaOriginalVariable = componenteTamanoPosicionVariable.sizeDelta;
            posicionOriginalVariable = componenteTamanoPosicionVariable.localPosition;
        }

        ResetTargets();

        if (textoDescriptivo != null)
        {
            textoDescriptivo.gameObject.SetActive(false);
        }
    }

    // 🎯 NUEVA FUNCIÓN: Restaura la apariencia si el botón se desactiva
    void OnDisable()
    {
        // Asegurar que cualquier animación o texto se detenga inmediatamente
        if (controlTextoCoroutine != null)
        {
            StopCoroutine(controlTextoCoroutine);
        }

        // Restablecer los targets a los valores originales (Estado normal)
        ResetTargets();

        // Aplicar los valores originales de forma INSTANTÁNEA (sin Lerp)
        AplicarValoresOriginalesInstantaneo();
    }

    void Update()
    {
        // Si el botón no es interactivo, no se ejecuta el Lerp de animación
        if (botonComponente != null && !botonComponente.interactable && estaHighlighted == false)
        {
            // Opcional: Si está deshabilitado y ya está en estado normal, detener la ejecución para ahorrar rendimiento.
            return;
        }

        // 1. ANCHOS (Botón propio y Lista) - Lógica de Lerp
        if (botonRectTransform != null)
        {
            Vector2 currentSize = botonRectTransform.sizeDelta;
            currentSize.x = Mathf.Lerp(currentSize.x, anchoTargetBoton, Time.deltaTime * velocidadDeTransicion);
            botonRectTransform.sizeDelta = currentSize;
        }

        foreach (KeyValuePair<RectTransform, float> item in anchosOriginalesAdicionales)
        {
            RectTransform rect = item.Key;
            float targetWidth = anchosTargetAdicionales.ContainsKey(rect) ? anchosTargetAdicionales[rect] : item.Value;

            Vector2 currentSize = rect.sizeDelta;
            currentSize.x = Mathf.Lerp(currentSize.x, targetWidth, Time.deltaTime * velocidadDeTransicion);
            rect.sizeDelta = currentSize;
        }

        // 2. TAMAÑO Y POSICIÓN (Componente Variable) - Lógica de Lerp
        if (componenteTamanoPosicionVariable != null)
        {
            componenteTamanoPosicionVariable.sizeDelta = Vector2.Lerp(
                componenteTamanoPosicionVariable.sizeDelta,
                sizeDeltaTargetVariable,
                Time.deltaTime * velocidadDeTransicion
            );

            componenteTamanoPosicionVariable.localPosition = Vector3.Lerp(
                componenteTamanoPosicionVariable.localPosition,
                posicionTargetVariable,
                Time.deltaTime * velocidadDeTransicion
            );
        }
    }

    // Función que aplica los valores originales de forma INMEDIATA
    private void AplicarValoresOriginalesInstantaneo()
    {
        // 1. Restaurar ancho del botón propio
        if (botonRectTransform != null)
        {
            Vector2 size = botonRectTransform.sizeDelta;
            size.x = anchoOriginalBoton;
            botonRectTransform.sizeDelta = size;
        }

        // 2. Restaurar anchos de la lista
        foreach (KeyValuePair<RectTransform, float> item in anchosOriginalesAdicionales)
        {
            RectTransform rect = item.Key;
            Vector2 size = rect.sizeDelta;
            size.x = item.Value; // Usamos el ancho original guardado
            rect.sizeDelta = size;
        }

        // 3. Restaurar tamaño y posición del componente variable
        if (componenteTamanoPosicionVariable != null)
        {
            componenteTamanoPosicionVariable.sizeDelta = sizeDeltaOriginalVariable;
            componenteTamanoPosicionVariable.localPosition = posicionOriginalVariable;
        }

        // 4. Ocultar el texto
        if (textoDescriptivo != null)
        {
            textoDescriptivo.gameObject.SetActive(false);
        }

        // El estado interno de Highlighted debe ser falso
        estaHighlighted = false;
    }

    // --- Definición de Estados Objetivo (Targets) ---

    private void SetEstadoActivo(bool activo)
    {
        // Si el botón no es interactivo, ignora el evento de hover
        if (botonComponente != null && !botonComponente.interactable)
        {
            // Siempre restaurar si el evento ocurre cuando está deshabilitado
            if (activo) AplicarValoresOriginalesInstantaneo();
            return;
        }

        if (activo == estaHighlighted) return;

        estaHighlighted = activo;

        // Detener corutina anterior
        if (controlTextoCoroutine != null)
        {
            StopCoroutine(controlTextoCoroutine);
        }

        // 1. Definir los objetivos (TARGETS)
        if (activo)
        {
            // Objetivos de Aumento
            anchoTargetBoton = anchoOriginalBoton + anchoAumentoEstandar;
            foreach (var pair in anchosOriginalesAdicionales)
            {
                anchosTargetAdicionales[pair.Key] = pair.Value + anchoAumentoEstandar;
            }
            sizeDeltaTargetVariable = new Vector2(sizeDeltaOriginalVariable.x + aumentoAnchoVariable, sizeDeltaOriginalVariable.y + aumentoAltoVariable);
            posicionTargetVariable = new Vector3(posicionOriginalVariable.x + cambioPosXVariable, posicionOriginalVariable.y + cambioPosYVariable, posicionOriginalVariable.z + cambioPosZVariable);
        }
        else
        {
            // Objetivos de Restauración
            ResetTargets();
        }

        // 2. Iniciar la Corutina para controlar el texto con retardo
        if (textoDescriptivo != null)
        {
            controlTextoCoroutine = StartCoroutine(ControlarVisibilidadTexto(activo));
        }
    }

    private IEnumerator ControlarVisibilidadTexto(bool debeEstarActivo)
    {
        if (debeEstarActivo)
        {
            float retardo = 0.5f / velocidadDeTransicion;
            yield return new WaitForSeconds(retardo);

            if (estaHighlighted)
            {
                textoDescriptivo.gameObject.SetActive(true);
            }
        }
        else
        {
            // Apagar texto de forma inmediata
            textoDescriptivo.gameObject.SetActive(false);
        }
    }

    private void ResetTargets()
    {
        anchoTargetBoton = anchoOriginalBoton;

        foreach (var pair in anchosOriginalesAdicionales)
        {
            anchosTargetAdicionales[pair.Key] = pair.Value;
        }

        sizeDeltaTargetVariable = sizeDeltaOriginalVariable;
        posicionTargetVariable = posicionOriginalVariable;
    }

    // --- Eventos de Interacción VR (Raycast sobre el botón) ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (botonComponente != null && botonComponente.interactable)
        {
            SetEstadoActivo(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetEstadoActivo(false);
    }
}