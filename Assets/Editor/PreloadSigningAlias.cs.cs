using UnityEngine;
using UnityEditor;
using System.IO;
 
[InitializeOnLoad]
public class PreloadSigningAlias {
    static PreloadSigningAlias ()
    {
        PlayerSettings.Android.keystorePass = "123qwe";
        PlayerSettings.Android.keyaliasName = "key0";
        PlayerSettings.Android.keyaliasPass = "123qwe";
    }
}