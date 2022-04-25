using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;
using System.Text.RegularExpressions;



public class WorkScript : MonoBehaviour
{

    public static TextMesh textMesh;
    public static GameObject Work;
    


  



    public void Start()
    {
        Work = GameObject.Find ("Work"); 
        textMesh = GetComponent<TextMesh>();
        
    }
     public static void SetParent(GameObject newParent)
    {       
        
        Work.transform.parent = newParent.transform;
        textMesh.transform.localPosition = new Vector3(-0.2f, 0, -0.102f);
        textMesh.transform.localRotation = Quaternion.Euler (90,0,0);
        
       
    }
    public static string SplitText(string text, int lineLength) 
  {
  return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
}
    public static void Text(string result)
    {
        
        var res = result;
        using (var reader = XmlReader.Create(new StringReader(res)))
{
    while(reader.Read())
    {
        if(reader.NodeType == XmlNodeType.Element && 
           reader.Name == "binding")
        {
           if(reader.HasAttributes)
           {
               if (reader.NodeType == XmlNodeType.Element && 
                   reader.GetAttribute("name") == "workLabel")
               {
                reader.ReadToDescendant("literal");
                string text= reader.ReadElementContentAsString();
                string splitText= SplitText(text, 61);
                textMesh.text = "              Notable Work: " + "\n" + splitText;
                
               }
              
           }
        }
        }
      }
    }
    public static void noText()
    {
    textMesh.text = " ";
}
}