/*
 * Developer Name: Md. Imran Hossain
 * E-mail: sandsoftimer@gmail.com
 * FB: https://www.facebook.com/md.imran.hossain.902
 * in: https://www.linkedin.com/in/md-imran-hossain-69768826/
 * 
 * This is a manager which will give all possible input response at runtime. 
 * like, 
 * DRAGGING, SWIPPING, TAPPING, TAP & HOLD etc.
 * 
 * N.B: If any script need to recieve input response then
 *      script should be under APBehaviour rather Monobehaviour
 *      & Awake should call this --> Registar_For_Input_Callback();
 *      Now just override input response functions to get notified about inputtype.
 *      Example_1: "public override void OnTapStart()"
 *      Example_2: "public override void OnSwipRight()"
 *      Example_3: "public override void OnDraggingInput(Vector3 dragAmount)"
 */

using UnityEngine;

namespace Com.AlphaPotato.Utility
{
    public class InputManager : APBehaviour
    {
        float tapStartedTime;
        float tap_n_hold_threshold_time = ConstantManager.TAP_N_HOLD_THRESHOLD;
        float dragging_threshold = ConstantManager.DRAGGING_THRESHOLD;

        Vector3 startMousePosition, lastMousePosition;
        TappingType inputType = TappingType.NONE;

        public bool inputTestingModeOn;

        #region ALL UNITY FUNCTIONS
        public override void Awake()
        {
            base.Awake();
            Registar_For_Input_Callback();
        }

        void Update()
        {
            if (gameManager == null)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                tapStartedTime = Time.time;
                startMousePosition = Input.mousePosition;
                lastMousePosition = Input.mousePosition;
                inputType = TappingType.TAP_START;
                gameManager.ProcessTapping(inputType, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (Input.GetMouseButton(0))
            {
                if (Vector3.Distance(Input.mousePosition, lastMousePosition) > dragging_threshold)
                {
                    gameManager.ProcessDragging(Input.mousePosition - lastMousePosition);
                }
                if((Time.time - tapStartedTime) >= tap_n_hold_threshold_time)
                {
                    inputType = TappingType.TAP_N_HOLD;
                    gameManager.ProcessTapping(inputType, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                inputType = TappingType.TAP_END;
                gameManager.ProcessTapping(inputType, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                float dist = (Input.mousePosition - startMousePosition).magnitude;
                if (dist >= ConstantManager.SWIPPING_THRESHOLD)
                {
                    float dX = Input.mousePosition.x - startMousePosition.x;
                    float dY = Input.mousePosition.y - startMousePosition.y;
                    if (Mathf.Abs(dX) > Mathf.Abs(dY))
                    {
                        if (dX > 0)
                            gameManager.ProcessSwipping(SwippingType.SWIPE_RIGHT);
                        else
                            gameManager.ProcessSwipping(SwippingType.SWIPE_LEFT);
                    }
                    else
                    {
                        if (dY > 0)
                            gameManager.ProcessSwipping(SwippingType.SWIPE_UP);
                        else
                            gameManager.ProcessSwipping(SwippingType.SWIPE_DOWN);
                    }
                }
            }

            #region DEBUG COMMAND

            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Break();
            }

            #endregion DEBUG COMMAND

        }

        void LateUpdate()
        {
            lastMousePosition = Input.mousePosition;
        }

        #endregion ALL UNITY FUNCTIONS
        //=================================   
        #region ALL OVERRIDING FUNCTIONS

        public override void OnTapStart(Vector3 tapOnWorldSpace)
        {
            base.OnTapStart(tapOnWorldSpace);

            if(inputTestingModeOn)
                Debug.Log("TAP START");
        }
        public override void OnTapEnd(Vector3 tapOnWorldSpace)
        {
            base.OnTapEnd(tapOnWorldSpace);

            if (inputTestingModeOn)
                Debug.Log("TAP END");
        }
        public override void OnTapAndHold(Vector3 tapOnWorldSpace)
        {
            base.OnTapAndHold(tapOnWorldSpace);

            if (inputTestingModeOn)
                Debug.Log("TAP N Hold");
        }
        public override void OnDrag(Vector3 dragAmount)
        {
            base.OnDrag(dragAmount);

            if (inputTestingModeOn)
                Debug.Log("Dragging");
        }
        public override void OnSwipeUp()
        {
            base.OnSwipeUp();

            if (inputTestingModeOn)
                Debug.Log("Swip Up");
        }
        public override void OnSwipeDown()
        {
            base.OnSwipeDown();

            if (inputTestingModeOn)
                Debug.Log("Swip Down");
        }
        public override void OnSwipeLeft()
        {
            base.OnSwipeLeft();

            if (inputTestingModeOn)
                Debug.Log("Swip Left");
        }
        public override void OnSwipeRight()
        {
            base.OnSwipeRight();

            if (inputTestingModeOn)
                Debug.Log("Swip Right");
        }
        #endregion ALL OVERRIDING FUNCTIONS
        //=================================
        #region ALL SELF DECLEAR FUNCTIONS


        #endregion ALL SELF DECLEAR FUNCTIONS
    }
}
