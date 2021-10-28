using UnityEngine;
using TMPro;

public class FPSDisplay : APBehaviour
{
    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    float deltaTime = 0.0f;

    TextMeshProUGUI guiText;
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();
        guiText = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (!guiText)
        {
            Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            guiText.text = format + " (" + (int)(deltaTime * 1000.0f) + " ms)";

            if (fps < 35)
                guiText.color = Color.yellow;
            else
                if (fps < 20)
                guiText.color = Color.red;
            else
                guiText.color = Color.white;
            //	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
    #endregion ALL UNITY FUNCTIONS
    //=================================   
    #region ALL OVERRIDING FUNCTIONS

    public override void OnGameInitializing()
    {
        base.OnGameInitializing();

        gameObject.SetActive(gameManager.debugModeOn);
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS


    #endregion ALL SELF DECLEAR FUNCTIONS

}