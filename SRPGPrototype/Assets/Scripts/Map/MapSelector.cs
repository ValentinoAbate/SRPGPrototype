using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    [SerializeField] private Toggle saveChoice;

    public void Show(MapManager mapManager, System.Action onComplete)
    {
        UIManager.main.TopBarUI.SetTitleText("Select Map");
        foreach(var mapData in mapManager.StartingMaps)
        {
            var button = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();
            void OnClick()
            {
                mapManager.SkipMapSelection = saveChoice.isOn;
                mapManager.Setup(mapData);
                Hide();
                onComplete?.Invoke();
            }
            button.onClick.AddListener(OnClick);
            var text = button.GetComponentInChildren<TMPro.TMP_Text>();
            text.text = mapData.MapName;
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
