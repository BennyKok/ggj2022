using UnityEngine;
using UnityEngine.Events;

namespace Blaze.Dialogue
{
    [AddComponentMenu("Blaze/Dialogue/Dialogue Events")]
    public class BlazeDialogueEvents : MonoBehaviour
    {
        [Title("Settings")]
        public bool isGlobalEvents = true;
        public bool useDefaultTypingEffect = true;
        public float typingEffectDelay = 0.05f;

        [Title("Dialogue")]
        // [CollapsedEvent]
        public UnityEvent onDialogueShow;
        // [CollapsedEvent]
        public UnityEvent onDialogueHide;

        [Title("Action")]

        // [CollapsedEvent]
        public UnityEvent onDialogueActionShow;
        // [CollapsedEvent]
        public UnityEvent onDialogueActionHide;

        // [CollapsedEvent]
        public UnityEvent onDialogueAction;

        [Title("Text")]

        // [CollapsedEvent]
        public StringEvent onDialogueContent;

        [Title("Line")]

        // [CollapsedEvent]
        public UnityEvent onDialogueLineBegin;

        // [CollapsedEvent]
        public UnityEvent onDialogueLineEnd;

        [Title("Audio")]

        // [CollapsedEvent]
        public AudioEvent onDialogueAudio;


        [Title("Actor")]

        // [CollapsedEvent]
        public StringEvent onActorName;
        // [CollapsedEvent]
        public SpriteEvent onActorIcon;


        [System.NonSerialized]
        public BlazeDialogue currentFocusDialogue;

        public static BlazeDialogueEvents Instance;

        private void Awake()
        {
            if (isGlobalEvents)
            {
                if (Instance != null)
                    Debug.LogWarning("Already has 1 BlazeDialogueEvents marked as isGlobalEvents");
                else
                    Instance = this;
            }
        }

        public void TriggerAction()
        {
            if (currentFocusDialogue)
            {
                currentFocusDialogue.TriggerAction();
                onDialogueAction.Invoke();
            }
        }

    }

    [System.Serializable]
    public class AudioEvent : UnityEvent<AudioClip>
    {

    }

    [System.Serializable]
    public class StringEvent : UnityEvent<string>
    {

    }

    [System.Serializable]
    public class SpriteEvent : UnityEvent<Sprite>
    {

    }
}
