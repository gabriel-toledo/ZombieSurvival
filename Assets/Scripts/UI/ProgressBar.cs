using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProgressBar : MonoBehaviour
{
    private int baseValue;
    private int maxValue;

    [SerializeField] private Image fill;
    [SerializeField] private Text amount;

    public void SetValues(int _baseValue, int _maxValue)
    {
        baseValue = _baseValue;
        maxValue = _maxValue;

        amount.text = baseValue.ToString();

        CalculateFillAmount();
    }

    private void CalculateFillAmount()
    {
        float fillAmount = (float)baseValue / (float)maxValue;
        fill.fillAmount = fillAmount;
    }
}
