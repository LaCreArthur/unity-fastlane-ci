using UnityEditor;

[InitializeOnLoad]
public class AndroidAutoKey
{
    #if UNITY_EDITOR

    static AndroidAutoKey()
    {
        #if UNITY_EDITOR_OSX
        PlayerSettings.Android.keystoreName = "/Users/arthur/unity-projects/unity-fastlane-ci/user.keystore";
        #endif
        PlayerSettings.Android.keystorePass = "750015";
        PlayerSettings.Android.keyaliasPass = "750015";
    }

    #endif
}
