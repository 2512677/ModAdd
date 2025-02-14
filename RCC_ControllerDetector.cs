using UnityEngine;

public class RCC_ControllerDetector : MonoBehaviour
{

    void Awake()
    {
        // ���� ������ ����� ����������� ����� �������, ����� ��������� �� ������������
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // ���������� ���������: ���� ��������� ����������, �� ������ Mobile, ����� � Keyboard
        if (Application.isMobilePlatform)
        {
            RCC_Settings.Instance.controllerType = RCC_Settings.ControllerType.Mobile;
        }
        else
        {
            RCC_Settings.Instance.controllerType = RCC_Settings.ControllerType.Keyboard;
        }

        Debug.Log("���������� ����������: " + (Application.isMobilePlatform ? "���������" : "��") +
                  ". ������ ��� ����������: " + RCC_Settings.Instance.controllerType);
    }
}
