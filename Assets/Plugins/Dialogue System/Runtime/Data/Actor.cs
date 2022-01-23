using UnityEngine;

namespace Blaze.Dialogue
{
    [CreateAssetMenu(fileName = "Actor", menuName = "Blaze/Dialogue/Actor", order = 100)]
    public class Actor : ScriptableObject
    {
        public Translatable actorName;
        public Sprite icon;
        public Color actorColorTint;

        public string GetActorNameTint()
        {
            return "<#" + ColorUtility.ToHtmlStringRGBA(actorColorTint) + ">" + actorName + ":</color>";
        }
    }
}