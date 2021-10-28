using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ColorBlinkTextMeshPro : MonoBehaviour
{
    [Range(0,1)]
    public float minimumOpacity = 0.5f;
    public float speed = 5;
    TextMeshProUGUI blinkText;
    Color selfColor;

    private void Awake()
    {
        blinkText = GetComponent<TextMeshProUGUI>();
        selfColor = blinkText.color;
        speed = speed < 1f ? 1f : speed;
    }

    public void Update()
    {
        selfColor.a = Mathf.Lerp(minimumOpacity, 1, Mathf.Abs(Mathf.Sin(Time.time * speed)));
        blinkText.color = selfColor;
    }
}
