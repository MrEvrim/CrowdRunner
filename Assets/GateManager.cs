using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GateManager : MonoBehaviour
{
    public TextMeshPro GateNo;
    public int randomNumber;
    public bool multiply;
    public Material positiveMaterial; // Pozitif sayı için materyal (yeşil)
    public Material negativeMaterial; // Negatif sayı için materyal (kırmızı)
    private Renderer gateChildRenderer; // Child objenin renderer bileşeni

    void Start()
    {
        // Child objenin Renderer bileşenini al (örneğin ilk child)
        gateChildRenderer = transform.GetChild(0).GetComponent<Renderer>();

        if (multiply)
        {
            randomNumber = Random.Range(-3, 3); // Negatif ve pozitif değerler arası seçim yap
            GateNo.text = (randomNumber > 0 ? "X" : "") + randomNumber; // "X" ifadesini sadece pozitifler için ekle
        }
        else
        {
            randomNumber = Random.Range(-100, 100);
            if (randomNumber % 2 != 0)
                randomNumber += 1;
            GateNo.text = randomNumber.ToString();
        }

        // Materyali belirle
        if (randomNumber < 0)
        {
            gateChildRenderer.material = negativeMaterial; // Negatifse kırmızı materyal
        }
        else
        {
            gateChildRenderer.material = positiveMaterial; // Pozitifse yeşil materyal
        }
    }
    private void Update()
    {
        if (randomNumber < 0)
        {
            gateChildRenderer.material = negativeMaterial; // Negatifse kırmızı materyal
        }
        else
        {
            gateChildRenderer.material = positiveMaterial; // Pozitifse yeşil materyal

        }
        GateNo.text = randomNumber.ToString();
    }
}
