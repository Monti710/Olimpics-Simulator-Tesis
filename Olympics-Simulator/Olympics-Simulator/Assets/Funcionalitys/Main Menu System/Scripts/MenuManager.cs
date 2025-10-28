using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MenuManager : MonoBehaviour
{
    [Serializable]
    public struct Entry { public string key; public GameObject root; }

    [Header("Menus")]
    [SerializeField] private Entry[] entries;

    [Header("Default (opcional)")]
    [Tooltip("Si está activo, se abrirá un menú al iniciar")]
    [SerializeField] private bool openDefaultOnStart = true;
    [Tooltip("Clave del menú por defecto (debe existir en Entries)")]
    [SerializeField] private string defaultKey = "";
    [Tooltip("Alternativa: referencia directa al menú por defecto")]
    [SerializeField] private GameObject defaultMenuRoot;

    [Header("XR (opcional)")]
    [SerializeField] private XRRayInteractor uiRay;
    [SerializeField] private XRDirectInteractor leftDirect;
    [SerializeField] private XRDirectInteractor rightDirect;

    private Dictionary<string, GameObject> map;
    private GameObject current;

    void Awake()
    {
        map = entries.Where(e => e.root != null)
                     .ToDictionary(e => e.key, e => e.root);

        foreach (var go in map.Values) go.SetActive(false);
        SetUiMode(false);
    }

    void Start()
    {
        if (!openDefaultOnStart) return;

        // 1) Prioridad: defaultKey si está definido y existe
        if (!string.IsNullOrEmpty(defaultKey) && map.TryGetValue(defaultKey, out var byKey))
        {
            Show(defaultKey);
            return;
        }

        // 2) Alternativa: defaultMenuRoot (referencia directa)
        if (defaultMenuRoot != null)
        {
            Show(defaultMenuRoot);
            return;
        }

        // 3) Si no hay nada definido, no se abre ningún menú
    }

    public void Show(string key)
    {
        if (!map.TryGetValue(key, out var target)) return;
        if (current == target) return;

        if (current) current.SetActive(false);
        current = target;
        current.SetActive(true);
        SetUiMode(true);
    }

    // Overload por referencia directa (útil para defaultMenuRoot)
    public void Show(GameObject menuRoot)
    {
        if (menuRoot == null) return;
        if (current == menuRoot) return;

        if (current) current.SetActive(false);
        current = menuRoot;
        current.SetActive(true);
        SetUiMode(true);
    }

    public void HideCurrent()
    {
        if (!current) return;
        current.SetActive(false);
        current = null;
        SetUiMode(false);
    }

    private void SetUiMode(bool uiOpen)
    {
        if (uiRay) uiRay.gameObject.SetActive(uiOpen);
        if (leftDirect) leftDirect.enabled = !uiOpen;
        if (rightDirect) rightDirect.enabled = !uiOpen;
    }
}
