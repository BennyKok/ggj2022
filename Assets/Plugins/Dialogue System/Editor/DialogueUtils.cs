using UnityEditor;
using UnityEngine;

namespace Blaze.Dialogue.Editor
{
    public static class DialogueUtils
    {
        [MenuItem("GameObject/Create Dialogue", false, 0)]

        static void CreateObjective()
        {
            var dialogue = new GameObject("New Dialogue");
            if (SceneView.lastActiveSceneView)
            {
                dialogue.transform.position = SceneView.lastActiveSceneView.pivot;
            }
            dialogue.AddComponent<BlazeDialogue>();
            dialogue.AddComponent<BoxCollider>().isTrigger = true;

            //Register undo
            Undo.RegisterCreatedObjectUndo(dialogue, "Create Dialogue");

            //Select the created objective
            Selection.activeGameObject = dialogue;
        }
    }
}