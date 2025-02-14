using UnityEngine;

public class RCC_ControllerDetector : MonoBehaviour
{

    void Awake()
    {
        // Этот объект будет сохраняться между сценами, чтобы настройки не сбрасывались
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Определяем платформу: если мобильное устройство, то ставим Mobile, иначе – Keyboard
        if (Application.isMobilePlatform)
        {
            RCC_Settings.Instance.controllerType = RCC_Settings.ControllerType.Mobile;
        }
        else
        {
            RCC_Settings.Instance.controllerType = RCC_Settings.ControllerType.Keyboard;
        }

        Debug.Log("Определено устройство: " + (Application.isMobilePlatform ? "Мобильное" : "ПК") +
                  ". Выбран тип управления: " + RCC_Settings.Instance.controllerType);
    }
}
