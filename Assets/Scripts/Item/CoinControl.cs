using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CoinControl : MonoBehaviour
{

    //public Text textComponent;
    private static int coinNum;
    TextMeshProUGUI textMeshPro;

    private static CoinControl instance;

    // Start is called before the first frame update
    void Start()
    {
        coinNum = 0;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = coinNum.ToString();
        instance = this;
    }

    public static void AddCoin(int newCoin)
    {
        coinNum += newCoin;
        instance.textMeshPro.text = coinNum.ToString();

    }
}
