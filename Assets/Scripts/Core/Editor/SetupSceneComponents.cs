using UnityEngine;
using UnityEditor;
using Glovebox.Interaction;
using Glovebox.CameraControl;

namespace Glovebox.Editor
{
    public static class SetupSceneComponents
    {
        [MenuItem("Tools/Glovebox/Attach Camera Controllers")]
        public static void AttachControllers()
        {
            GameObject mainCam = GameObject.Find("Main Camera");
            if (mainCam != null)
            {
                if (mainCam.GetComponent<SmoothCameraController>() == null)
                {
                    Undo.AddComponent<SmoothCameraController>(mainCam);
                }
                if (mainCam.GetComponent<ObjectInteractionController>() == null)
                {
                    Undo.AddComponent<ObjectInteractionController>(mainCam);
                }

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene());

                Debug.Log("[SetupSceneComponents] Successfully attached SmoothCameraController & ObjectInteractionController to Main Camera!");
            }
        }
    }
}
