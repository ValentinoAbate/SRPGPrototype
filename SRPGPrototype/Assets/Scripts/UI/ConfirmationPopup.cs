using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfirmationPopup : MonoBehaviour
{
    private const string defaultTitleText = "Are you sure?";
    private const string defaultDescText = "";
    private const string defaultConfirmText = "Yes";
    private const string defaultCancelText = "Cancel";

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI yesText;
    [SerializeField] private TextMeshProUGUI noText;

    private System.Action<bool> onComplete;

    public void Show(System.Action<bool> onComplete, string titleText = null, string descriptionText = null, string confirmText = null, string cancelText = null)
    {
        this.titleText.text = titleText ?? defaultTitleText;
        this.descriptionText.text = descriptionText ?? defaultDescText;
        yesText.text = confirmText ?? defaultConfirmText;
        noText.text = cancelText ?? defaultCancelText;
        gameObject.SetActive(true);
        this.onComplete = onComplete;
    }

    public void Confirm()
    {
        Hide();
        onComplete?.Invoke(true);
    }

    public void Cancel()
    {
        Hide();
        onComplete?.Invoke(false);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
