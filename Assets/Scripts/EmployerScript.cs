using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;



public class EmployerScript : MonoBehaviour
{

    public static TextMesh textMesh;
    public static GameObject Employer;
    


  



    public void Start()
    {
        Employer = GameObject.Find ("Employer"); 
        textMesh = GetComponent<TextMesh>();
        
    }
     public static void SetParent(GameObject newParent)
    {       
        
        Employer.transform.parent = newParent.transform;
        textMesh.transform.localPosition = new Vector3(-0.15f, 0, -0.072f);
        textMesh.transform.localRotation = Quaternion.Euler (90,0,0);
        
       
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
                   reader.GetAttribute("name") == "employerLabel")
                   
               {
                reader.ReadToDescendant("literal");
                string text= reader.ReadElementContentAsString();
                textMesh.text = "Employer: " + text;
                
               }
               else if(reader.NodeType == XmlNodeType.Element && reader.GetAttribute("name") == "manufacturers")
               {
                reader.ReadToDescendant("literal");
                string text= reader.ReadElementContentAsString();
                textMesh.text = "Manufacturers: " + text;
                

                   }
           }
        }
        }
      }
    }

}