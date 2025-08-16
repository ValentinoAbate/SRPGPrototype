using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InputPopup : MonoBehaviour
{
    private const string defaultTitleText = "Input Text";
    private const string defaultConfirmText = "Confirm";
    private const string defaultCancelText = "Cancel";

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI confirmText;
    [SerializeField] private TextMeshProUGUI cancelText;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;

    private System.Action<string> onComplete;
    private System.Action onCancel;

    public void Show(System.Action<string> onComplete, System.Action onCancel, int characterLimit = int.MaxValue, bool isNumber = false, string titleText = null, string confirmText = null, string cancelText = null, string defaultInput = "")
    {
        this.titleText.text = titleText ?? defaultTitleText;
        this.confirmText.text = confirmText ?? defaultConfirmText;
        this.cancelText.text = cancelText ?? defaultCancelText;
        gameObject.SetActive(true);
        this.onComplete = onComplete;
        this.onCancel = onCancel;

        inputField.onValidateInput = ValidateInput;
        inputField.characterLimit = characterLimit;
        inputField.contentType = isNumber ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.Standard;
        inputField.text = defaultInput;
        OnValueChanged(defaultInput);
    }

    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        if(inputField.contentType == TMP_InputField.ContentType.Standard)
        {
            if (addedChar == ' ' || char.IsLetterOrDigit(addedChar))
                return addedChar;
            return '\0';
        }
        return addedChar;
    }

    public void OnValueChanged(string newValue)
    {
        confirmButton.interactable = newValue.Length > 0;
    }

    public void Confirm()
    {
        Hide();
        onComplete?.Invoke(inputField.text);
    }

    public void Cancel()
    {
        Hide();
        onCancel?.Invoke();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
