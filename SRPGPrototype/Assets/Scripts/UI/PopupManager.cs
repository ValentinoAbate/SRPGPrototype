using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager main;
    private void Awake()
    {
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(transform);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private ConfirmationPopup confirmationPopup;
    [SerializeField] private InputPopup inputPopup;
    [SerializeField] private Image rayCastBlocker;

    public void ShowConfirmationPopup(System.Action<bool> onComplete, string titleText = null, string descriptionText = null, string confirmText = null, string cancelText = null)
    {
        void OnComplete(bool success)
        {
            onComplete?.Invoke(success);
            SetRaycastBlockerActive(false);
        }
        SetRaycastBlockerActive(true);
        confirmationPopup.Show(OnComplete, titleText, descriptionText, confirmText, cancelText);
    }

    public void ShowInputPopup(System.Action<string> onComplete, System.Action onCancel, int characterLimit = int.MaxValue, bool isNumber = false, string titleText = null, string confirmText = null, string cancelText = null, string defaultInput = "")
    {
        void OnComplete(string text)
        {
            onComplete?.Invoke(text);
            SetRaycastBlockerActive(false);
        }
        void OnCancel()
        {
            onCancel?.Invoke();
            SetRaycastBlockerActive(false);
        }
        SetRaycastBlockerActive(true);
        inputPopup.Show(OnComplete, OnCancel, characterLimit, isNumber, titleText, confirmText, cancelText, defaultInput);
    }

    private void SetRaycastBlockerActive(bool active)
    {
        rayCastBlocker.gameObject.SetActive(active);
    }
}
