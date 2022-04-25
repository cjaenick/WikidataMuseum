using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;



public class ScanScript : MonoBehaviour
{

    public static TextMesh textMesh;
    
    


  



    public void Start()
    {
       
        textMesh = GetComponent<TextMesh>();
        
        
    }
    
    public static void Text()
    {
        textMesh.text = " ";
      
}
}