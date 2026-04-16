using UnityEditor;

[InitializeOnLoad]
static class IOSAutoSign
{
    static IOSAutoSign()
    {
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
        PlayerSettings.iOS.appleDeveloperTeamID = "5NSYQ9FH63";
    }
}
