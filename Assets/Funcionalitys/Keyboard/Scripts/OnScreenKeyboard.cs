using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrabdeepDhaliwal.OnScreenKeyboard
{
    public class OnScreenKeyboard : MonoBehaviour
    {
        [Header("Campo de texto destino")]
        public TMP_InputField targetInputField;

        [Header("Teclas normales")]
        public Key[] keys;

        [Header("Teclas especiales")]
        public Button shiftButton;
        public Button capsLockButton;
        public Button enterButton;
        public Button backspaceButton;
        public Button deleteButton;
        public Button spaceButton;
        public Button leftArrowButton;
        public Button rightArrowButton;

        private bool isCapsLockOn = false;
        private bool isShiftPressed = false;

        void Start()
        {
            if (targetInputField == null)
            {
                Debug.LogError("⚠️ No se ha asignado ningún TMP_InputField al teclado.");
                return;
            }

            // Configurar las teclas normales
            foreach (var key in keys)
            {
                key.Setup();
                key.button.onClick.AddListener(() => OnKeyPress(key));
            }

            // Configurar las teclas especiales
            capsLockButton?.onClick.AddListener(ToggleCapsLock);
            shiftButton?.onClick.AddListener(ToggleShift);
            backspaceButton?.onClick.AddListener(DeleteCharacter);
            deleteButton?.onClick.AddListener(DeleteCharacterAtCaret);
            spaceButton?.onClick.AddListener(() => InsertCharacter(" "));
            enterButton?.onClick.AddListener(SubmitText);
            leftArrowButton?.onClick.AddListener(() => MoveCaret(-1));
            rightArrowButton?.onClick.AddListener(() => MoveCaret(1));

            UpdateKeyLabels();
        }

        // --- Inserción de texto ---
        private void OnKeyPress(Key key)
        {
            if (targetInputField == null) return;

            string value = GetKeyText(key);
            InsertCharacter(value);

            if (isShiftPressed)
            {
                isShiftPressed = false;
                UpdateKeyLabels();
            }
        }

        private void InsertCharacter(string character)
        {
            if (string.IsNullOrEmpty(character)) return;

            int pos = targetInputField.caretPosition;
            targetInputField.text = targetInputField.text.Insert(pos, character);
            targetInputField.caretPosition = pos + character.Length;
        }

        private void DeleteCharacter()
        {
            if (string.IsNullOrEmpty(targetInputField.text) || targetInputField.caretPosition == 0) return;

            int pos = targetInputField.caretPosition;
            targetInputField.text = targetInputField.text.Remove(pos - 1, 1);
            targetInputField.caretPosition = pos - 1;
        }

        private void DeleteCharacterAtCaret()
        {
            if (string.IsNullOrEmpty(targetInputField.text)) return;
            int pos = targetInputField.caretPosition;
            if (pos < targetInputField.text.Length)
                targetInputField.text = targetInputField.text.Remove(pos, 1);
        }

        private void MoveCaret(int direction)
        {
            int newPos = Mathf.Clamp(targetInputField.caretPosition + direction, 0, targetInputField.text.Length);
            targetInputField.caretPosition = newPos;
        }

        // --- Teclas especiales ---
        private void SubmitText()
        {
            Debug.Log("Texto ingresado: " + targetInputField.text);
            // Aquí puedes agregar tu lógica de envío o limpieza del campo si lo deseas
        }

        private void ToggleCapsLock()
        {
            isCapsLockOn = !isCapsLockOn;
            UpdateKeyLabels();
        }

        private void ToggleShift()
        {
            isShiftPressed = !isShiftPressed;
            UpdateKeyLabels();
        }

        private string GetKeyText(Key key)
        {
            string text = key.primaryValue;

            if (text.Length == 1 && char.IsLetter(text[0]))
            {
                if (isShiftPressed ^ isCapsLockOn)
                    text = text.ToUpper();
                else
                    text = text.ToLower();
            }
            else if (isShiftPressed && !string.IsNullOrEmpty(key.secondaryValue))
            {
                text = key.secondaryValue;
            }

            return text;
        }

        private void UpdateKeyLabels()
        {
            foreach (var key in keys)
            {
                if (key.primaryKeyText == null) continue;

                if (key.primaryValue.Length == 1 && char.IsLetter(key.primaryValue[0]))
                {
                    key.primaryKeyText.text = (isShiftPressed ^ isCapsLockOn)
                        ? key.primaryValue.ToUpper()
                        : key.primaryValue.ToLower();
                }
                else
                {
                    key.primaryKeyText.text = isShiftPressed && !string.IsNullOrEmpty(key.secondaryValue)
                        ? key.secondaryValue
                        : key.primaryValue;
                }
            }
        }
    }
}
