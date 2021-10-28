using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ClassPropertyReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ReadProperty(new GameplayData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadProperty(object _object)
    {
        
        TypeInfo tpi = _object.GetType().GetTypeInfo();

        FieldInfo[] finfos = tpi.GetFields();
        for (int i = 0; i < finfos.Length; i++)
        {
            FieldInfo fi = finfos[i];

            //Debug.LogError("Name: " + fi.Name + " = " + fi.GetValue(_object) + "\tData Type: " + fi.FieldType);

            if (fi.FieldType.Equals(typeof(GameplayData)))
            {
                Debug.LogError("GameplayData Detected");
            }

            switch (Type.GetTypeCode(fi.FieldType))
            {
                case TypeCode.Object:
                    ReadProperty(fi.GetValue(_object));
                    break;
                case TypeCode.Int64:
                    
                    break;
            }
        }
    }

    public static String[] GetAllPublicPropertyNames(System.Type _object)
    {
        List<string> allPublicProperties = new List<string>();

        //TypeInfo tpi = _object.GetType().GetTypeInfo();
        //FieldInfo[] fieldInfo = tpi.GetFields();

        //UnityEditor.MonoScript
        
        Debug.LogError(_object.GetProperties().Length);
        return allPublicProperties.ToArray();
    }
}

