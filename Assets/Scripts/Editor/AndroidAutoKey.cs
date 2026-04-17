using System.IO;
using UnityEditor;
using UnityEngine;

// Auto-configures keystore + passwords for local Unity Editor Android builds.
// CI builds ignore this and read from ANDROID_* GitHub secrets passed to GameCI.
// Keystore is throwable / for this template only.
[InitializeOnLoad]
public class AndroidAutoKey
{
    static AndroidAutoKey()
    {
        string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
        PlayerSettings.Android.keystoreName = Path.Combine(projectRoot, "user.keystore");
        PlayerSettings.Android.keystorePass = "750015";
        PlayerSettings.Android.keyaliasName = "upload-key";
        PlayerSettings.Android.keyaliasPass = "750015";
    }
}
