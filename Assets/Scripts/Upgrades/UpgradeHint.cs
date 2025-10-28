using UnityEngine;
using TMPro;

public class UpgradeHint : MonoBehaviour
{
    [Header("Refs")]
    public UpgradeFlow flow;
    public TMP_Text hintText;          // ��� TMP_Text "������� E..."

    [Header("Visual")]
    public string message = "������� E ��� ��������";
    public bool blink = true;
    public float blinkSpeed = 2f;
    public Color visibleColor = new Color(1, 1, 1, 1);
    public Color hiddenColor = new Color(1, 1, 1, 0);

    bool _wantShow;

    void Reset()
    {
        hintText = GetComponent<TMP_Text>();
    }

    void Awake()
    {
        if (!hintText) hintText = GetComponent<TMP_Text>();
        if (hintText) { hintText.text = message; hintText.color = hiddenColor; }
    }

    void Update()
    {
        if (!flow || !hintText) return;

        // ����������, ����� �� ������� (��� �� �������, ��� � ��� ����)
        _wantShow = flow.CanOpenNow();

        if (!_wantShow)
        {
            // ������ ������
            hintText.color = Color.Lerp(hintText.color, hiddenColor, Time.unscaledDeltaTime * 10f);
            return;
        }

        // ���������� (������, ���� �����)
        if (blink)
        {
            float a = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * blinkSpeed);
            var c = visibleColor; c.a = Mathf.Lerp(0.25f, 1f, a);
            hintText.color = c;
        }
        else
        {
            hintText.color = visibleColor;
        }

        // ������ ���������� ����� (����� ������/������ ��������)
        if (hintText.text != message) hintText.text = message;
    }
}