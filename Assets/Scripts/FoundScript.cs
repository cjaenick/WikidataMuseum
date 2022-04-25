using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;



public class FoundScript : MonoBehaviour
{

    public static TextMesh textMesh;
    public static GameObject Found;
    


  



    public void Start()
    {
        Found = GameObject.Find ("Found"); 
        textMesh = GetComponent<TextMesh>();
        
    }
     public static void SetParent(GameObject newParent)
    {       
        
        Found.transform.parent = newParent.transform;
        textMesh.transform.localPosition = new Vector3(-0.13f, 0, 0.132f);
        textMesh.transform.localRotation = Quaternion.Euler (90,0,0);
        

       
    }
    public static void Text()
    {
        
        
               
                textMesh.text = "Found: ";
                
               }
           

}