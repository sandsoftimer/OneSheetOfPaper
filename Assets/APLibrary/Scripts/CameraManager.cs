/*
 * Developer Name: Md. Imran Hossain
 * E-mail: sandsoftimer@gmail.com
 * FB: https://www.facebook.com/md.imran.hossain.902
 * in: https://www.linkedin.com/in/md-imran-hossain-69768826/
 * 
 * This is a manager which will give common functional supports. 
 * like, Camera shaking, Slowmotioning, Vector from screen touch etc.
 *  
 */

using System.Collections;
using System.IO;
using UnityEngine;

namespace Com.AlphaPotato.Utility
{

    public class CameraManager : MonoBehaviour
    {

        public void ShakeDefaultCamera(float duration = 0.04f, float magnitude = 0.02f, bool isCameraLocal = false)
        {
            StartCoroutine(ShakeDefaultCam(Camera.main.transform, duration, magnitude, isCameraLocal));
        }

        public void ShakeParentOfCamera(float duration = 0.04f, float magnitude = 0.02f, bool isCameraLocal = false)
        {
            GameObject camParent = new GameObject("Temporary_CameraParent");
            camParent.transform.position = Camera.main.transform.position;
            Camera.main.transform.parent = camParent.transform;
            StartCoroutine(ShakeDefaultCam(camParent.transform, duration, magnitude, isCameraLocal));
        }

        IEnumerator ShakeDefaultCam(Transform cam, float duration, float magnitude, bool isCameraLocal = false)
        {
            if (isCameraLocal)
            {
                Vector3 initialPosition = cam.localPosition;

                float elapseTime = 0.0f;

                while (elapseTime <= duration)
                {

                    elapseTime += Time.deltaTime;

                    Vector3 noise = new Vector3(
                        Random.Range(-1f, 1f) * magnitude,
                        Random.Range(-1f, 1f) * magnitude,
                        Random.Range(-1f, 1f) * magnitude
                        );

                    noise.x += initialPosition.x;
                    noise.y += initialPosition.y;
                    noise.z += initialPosition.z;

                    cam.transform.localPosition = noise;

                    yield return null;
                }

                cam.localPosition = initialPosition;

            }
            else {

                Vector3 initialPosition = cam.position;

                float elapseTime = 0.0f;

                while (elapseTime <= duration)
                {

                    elapseTime += Time.deltaTime;

                    Vector3 noise = new Vector3(
                        Random.Range(-1f, 1f) * magnitude,
                        Random.Range(-1f, 1f) * magnitude,
                        Random.Range(-1f, 1f) * magnitude
                        );

                    noise.x += initialPosition.x;
                    noise.y += initialPosition.y;
                    noise.z += initialPosition.z;

                    cam.transform.position = noise;

                    yield return null;
                }

                cam.position = initialPosition;

            }
        }


        bool isSlowMotionActive = false;
        float slowMotionDuration;
        public void DoSlowMotion(float duration = 2f, float magnitude = 0.05f)
        {

            if (isSlowMotionActive)
                return;

            slowMotionDuration = duration;
            Time.timeScale = magnitude;
            Time.fixedDeltaTime = Time.timeScale * 0.2f;
            isSlowMotionActive = true;
        }

        private void Update()
        {
            UpdateSlowMotion();
            CaptureEditorGameViewScreenshot();
        }

        void UpdateSlowMotion()
        {

            if (isSlowMotionActive)
            {

                Time.timeScale += (1 / slowMotionDuration) * Time.unscaledDeltaTime;

                if (Time.timeScale > 1)
                {

                    Time.timeScale = 1;
                    isSlowMotionActive = false;
                }
            }
        }

        void CaptureEditorGameViewScreenshot()
        {

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                string path = Application.dataPath;
                path = path.Replace("/Assets", "");
                path += "/Screenshots/";

                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                catch (IOException ex)
                {
                    Debug.LogError(ex.Message);
                }

                //var theMonth = System.DateTime.Now.Month;
                //var theDay = System.DateTime.Now.Day;
                //var theYear = System.DateTime.Now.Year;
                var theTime = System.DateTime.Now.ToString("hh_mm_ss");
                var theDate = System.DateTime.Now.ToString("MM_dd_yyyy");
                path += UnityEditor.UnityStats.screenRes + "_" + theDate + "_" + theTime + ".png";

                ScreenCapture.CaptureScreenshot(path, 1);
            }
#endif
        }
    }
}
