using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Xml;



public class NameScript : MonoBehaviour
{

    public static TextMesh textMesh;
    public static GameObject Name;
    


  



    public void Start()
    {
        Name = GameObject.Find ("Name"); 
        textMesh = GetComponent<TextMesh>();
        
    }
     public static void SetParent(GameObject newParent)
    {       
        
        Name.transform.parent = newParent.transform;
        textMesh.transform.localPosition = new Vector3(-0.042f, 0, 0.132f);
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
                   reader.GetAttribute("name") == "personLabel")
               {
                reader.ReadToDescendant("literal");
                string text= reader.ReadElementContentAsString();
                textMesh.text = text;
                
               }
           }
        }
        }
      }
    }

}