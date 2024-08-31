using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GateManager : MonoBehaviour
{
    public TextMeshPro GateNo;
    public int randomNumber;
    public bool multiply;
    public Material positiveMaterial;
    public Material negativeMaterial; 
    private Renderer gateChildRenderer; 

    void Start()
    {
   
        gateChildRenderer = transform.GetChild(0).GetComponent<Renderer>();

        if (multiply)
        {
            randomNumber = Random.Range(-3, 3); 
            GateNo.text = (randomNumber > 0 ? "X" : "") + randomNumber; 
        }
        else
        {
            randomNumber = Random.Range(-100, 100);
            if (randomNumber % 2 != 0)
                randomNumber += 1;
            GateNo.text = randomNumber.ToString();
        }

        if (randomNumber < 0)
        {
            gateChildRenderer.material = negativeMaterial; 
        }
        else
        {
            gateChildRenderer.material = positiveMaterial; 
        }
    }
    private void Update()
    {
        if (randomNumber < 0)
        {
            gateChildRenderer.material = negativeMaterial; 
        }
        else
        {
            gateChildRenderer.material = positiveMaterial; 

        }
        GateNo.text = randomNumber.ToString();
    }

}
