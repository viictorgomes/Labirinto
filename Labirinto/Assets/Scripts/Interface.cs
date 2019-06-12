using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface : MonoBehaviour
{

    public GameObject configMenu;
    
    void Start()
    {
        configMenu.SetActive(false); //esconder o menu "configurações" quando iniciar a interface
    }
    
    void Update()
    {
        
    }
}
