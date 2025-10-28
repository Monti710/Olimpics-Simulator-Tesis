using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrabdeepDhaliwal.OnScreenKeyboard
{
    public class ManagedButton : MonoBehaviour
    {
        #region Fields
        private Button button;

        [SerializeField]
        private bool selectOnStart = false;
        [SerializeField]
        private bool delayInteractionOnStart = false;
        #endregion

        #region Init
        private void Awake()
        {
            button = GetComponent<Button>();
            Debug.Log("Button initialized: " + button.name); // Log para verificar que el botón se ha inicializado
        }

        private void OnEnable()
        {
            if (gameObject.activeInHierarchy && selectOnStart)
            {
                Debug.Log("Button is selected on start: " + button.name);
                if (!delayInteractionOnStart)
                {
                    OnSelect(button.gameObject);
                }
                else
                {
                    Debug.Log("Delaying interaction on start for button: " + button.name);
                    StartCoroutine(IDelayedAction(() => OnSelect(button.gameObject), 0.15f));
                }
            }
        }

        public void OnSelect(GameObject go)
        {
            EventSystem.current.SetSelectedGameObject(go);
        }
        private IEnumerator IDelayedAction(System.Action a, float f)
        {
            yield return new WaitForSecondsRealtime(f);
            a?.Invoke();
        }
        #endregion
    }
}