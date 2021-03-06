using UnityEngine;

namespace Blaze.Dialogue
{
    public class BlazeDialogueSettings : ScriptableObject
    {
        public const string settingsAssetName = "BlazeDialogueSettings";

        public const string settingsFolderPath = "Assets/Resources/";

        public const string settingsPath = settingsFolderPath + settingsAssetName + ".asset";
        public string[] languageDefinition = { "English" };
    }
}