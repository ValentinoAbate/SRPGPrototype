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

    private void SetRaycastBlockerActive(bool active)
    {
        rayCastBlocker.gameObject.SetActive(active);
    }
}
