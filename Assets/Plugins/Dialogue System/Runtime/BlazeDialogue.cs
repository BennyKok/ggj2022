using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Blaze.Dialogue
{
    [AddComponentMenu("Blaze/Dialogue/Dialogue")]
    public class BlazeDialogue : MonoBehaviour
    {
        public enum DialogueTriggerType
        {
            None, OnStart, OnTrigger, OnCollision, OnTriggerInput
        }

        public BlazeDialogueEvents targetEvents;

        public Dialogue dialogue;

        public DialogueTriggerType triggerType;

        public string triggerInputKey;

        public bool invokeOnce;
        public float startDelay;
        public bool cancelDialogueOnExit;

        public bool checkObjectTag;
        public bool appendActorName = true;

        public string targetObjectTag;

        [CollapsedEvent]
        public UnityEvent onFinished;

        private bool receivedAction;

        private BlazeDialogueEvents events;

        private bool dialogueShowing;

        private bool dialogueStarted;

        private GameObject targetInTrigger;

        public void StartDialogue()
        {
            if (invokeOnce && dialogueStarted) return;
            
            dialogueStarted = true;
            if (targetEvents)
            {
                events = targetEvents;
            }
            else
            {
                events = BlazeDialogueEvents.Instance;
            }

            if (!events)
            {
                Debug.LogError("BlazeDialogueEvents doesn't exist, the dialogue couldn't be played.");
                return;
            }
            events.currentFocusDialogue = this;
            StopCoroutine("StartDialogueCoroutine");
            StartCoroutine("StartDialogueCoroutine");
        }

        public void StopDialogue()
        {
            StopCoroutine("StartDialogueCoroutine");
            HandleDialogueFinish();
        }

        public void TriggerAction()
        {
            receivedAction = true;
        }

        public IEnumerator StartDialogueCoroutine()
        {
            if (startDelay > 0)
            {
                yield return new WaitForSecondsRealtime(startDelay);
            }
            // print("Dialogue Begin");
            dialogueShowing = true;
            //the dialog should be displayed
            events.onDialogueShow.Invoke();
            foreach (var item in dialogue.contents)
            {
                //Out of luck, we skip this line
                if (Random.value > item.chance)
                    continue;

                events.onDialogueLineBegin.Invoke();

                if (item.actor)
                {
                    events.onActorName.Invoke(item.actor.actorName);
                    events.onActorIcon.Invoke(item.actor.icon);
                }

                if (item.clip)
                    events.onDialogueAudio.Invoke(item.clip);

                string content = item.content;
                if (item.actor && appendActorName)
                {
                    content = $"<b>{item.actor.GetActorNameTint()} {content}</b>";
                }
                if (events.useDefaultTypingEffect)
                {
                    var time = Time.time;
                    yield return StartCoroutine(TypeText(content));
                    var timeUsedForTyping = Time.time - time;
                }
                else
                {
                    events.onDialogueContent.Invoke(content);
                }
                events.onDialogueLineEnd.Invoke();

                if (item.waitForAction)
                {
                    events.onDialogueActionShow.Invoke();
                    yield return new WaitUntil(() => receivedAction == true);
                    receivedAction = false;
                    events.onDialogueActionHide.Invoke();
                }
                else
                {
                    // if (item.delay - timeUsedForTyping > 0)
                    yield return new WaitForSecondsRealtime(item.delay);
                }
            }
            HandleDialogueFinish();
        }

        private void HandleDialogueFinish()
        {
            if (!dialogueShowing) return;

            onFinished.Invoke();
            events.onDialogueHide.Invoke();
            events.onDialogueActionHide.Invoke();
            dialogueShowing = false;
        }

        public IEnumerator TypeText(string text)
        {
            var skipIndex = -1;
            for (int i = 0; i < text.Length; i++)
            {
                if (i < skipIndex + 1)
                {
                    continue;
                }

                var currentCharacter = text.Substring(i, 1);
                if (currentCharacter == "<")
                {
                    var nextCloseTagIndex = text.Substring(i).IndexOf(">") + i;
                    skipIndex = nextCloseTagIndex;
                    // print("tag detected " + skipIndex);
                    continue;
                }

                yield return new WaitForSecondsRealtime(events.typingEffectDelay);
                if (currentCharacter == " ")
                {
                    continue;
                }
                events.onDialogueContent.Invoke(text.Substring(0, i + 1));
            }
        }

        /////////Lifecycle hook

        private void Start()
        {
            if (triggerType == DialogueTriggerType.OnStart)
                StartDialogue();
        }

        private void Update()
        {
            if (targetInTrigger && triggerType == DialogueTriggerType.OnTriggerInput)
            {
                if (Input.GetKeyDown(triggerInputKey))
                {
                    StartDialogue();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var isTag = MatchTag(other.gameObject);
            if (isTag && targetInTrigger == null) targetInTrigger = other.gameObject;
            
            if (triggerType == DialogueTriggerType.OnTrigger && isTag)
                StartDialogue();
        }

        private void OnTriggerExit(Collider other)
        {
            var isTag = MatchTag(other.gameObject);
            if (targetInTrigger == other.gameObject) targetInTrigger = null;
            
            if (triggerType == DialogueTriggerType.OnTrigger && cancelDialogueOnExit && isTag)
                StopDialogue();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerType == DialogueTriggerType.OnTrigger && MatchTag(other.gameObject))
                StartDialogue();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (triggerType == DialogueTriggerType.OnTrigger && cancelDialogueOnExit && MatchTag(other.gameObject))
                StopDialogue();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (triggerType == DialogueTriggerType.OnCollision && MatchTag(other.gameObject))
                StartDialogue();
        }

        private void OnCollisionExit(Collision other)
        {
            if (triggerType == DialogueTriggerType.OnCollision && cancelDialogueOnExit && MatchTag(other.gameObject))
                StopDialogue();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (triggerType == DialogueTriggerType.OnCollision && MatchTag(other.gameObject))
                StartDialogue();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (triggerType == DialogueTriggerType.OnCollision && cancelDialogueOnExit && MatchTag(other.gameObject))
                StopDialogue();
        }

        public bool MatchTag(GameObject other)
        {
            if (!checkObjectTag)
                return true;

            return other.CompareTag(targetObjectTag);
        }
    }
}