using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Utils
{
    public class Screenshot : MonoBehaviour
    {
        /* ======================================================================================================================== */
        /* VARIABLE DECLARATIONS                                                                                                    */
        /* ======================================================================================================================== */
    
        [SerializeField] private int superSize = 1;
        
        [Header("Input Actions")]
        [SerializeField] private InputActionReference screenshotInputAction;
        
        /* ======================================================================================================================== */
        /* UNITY CALLBACKS                                                                                                          */
        /* ======================================================================================================================== */

        private void OnEnable()
        {
            screenshotInputAction.action.performed += OnScreenshot;
        }

        private void OnDisable()
        {
            screenshotInputAction.action.performed -= OnScreenshot;
        }

        /* ======================================================================================================================== */
        /* COROUTINES                                                                                                               */
        /* ======================================================================================================================== */

        /* ======================================================================================================================== */
        /* PRIVATE FUNCTIONS                                                                                                        */
        /* ======================================================================================================================== */

        private void TakeScreenshot()
        {
#if UNITY_EDITOR
            // create screenshot folder if it doesn't exists
            string folderpath = Application.dataPath + "/../Screenshots";
            if (Directory.Exists(folderpath) == false)
            {
                Directory.CreateDirectory(folderpath);
            }

            // save screenshot with date, time and resolution
            string resolution = Screen.width * superSize + "x" + Screen.height * superSize + "_";
            string dateAndTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");
            string filename = folderpath + "/screen_" + dateAndTime + "_" + resolution + ".png";
            ScreenCapture.CaptureScreenshot(filename, superSize);
            Debug.Log("Screenshot saved to: " + filename);
#endif
        }
        
        /* ======================================================================================================================== */
        /* PUBLIC FUNCTIONS                                                                                                         */
        /* ======================================================================================================================== */

        /* ======================================================================================================================== */
        /* EVENT CALLERS                                                                                                            */
        /* ======================================================================================================================== */

        /* ======================================================================================================================== */
        /* EVENT LISTENERS                                                                                                          */
        /* ======================================================================================================================== */
        
        private void OnScreenshot(InputAction.CallbackContext ctx)
        {
            if (ctx.ReadValueAsButton() == true)
            {
                TakeScreenshot();
            }
        }
    }
}