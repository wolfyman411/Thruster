using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class HUD : MonoBehaviour
{
    public GameObject leaderboard;
    Vector3 leaderboardpos;
    Vector3 newboardpos;

    Image lapRef;
    public Sprite[] lapImages;

    int minutes = 0;
    int seconds = 0;
    int miliseconds = 0;
    //Split
    public int s_minutes = 0;
    public int s_seconds = 0;
    public int s_miliseconds = 0;

    Image startRef;
    public Sprite[] startImages;
    int startFrame = 0;
    Vector3 startScale;
    public float startTime;

    Image speedRef;
    public Sprite[] speedImages;
    int numberOfFrames = 11;
    int currentFrame = 0;

    Image inventoryImageRef;
    public Sprite[] attackImages;
    public Sprite[] boostImages;
    public Sprite[] supportImages;
    public Sprite emptyImage;

    Image placeImageRef;
    public Sprite[] placeImages;

    VehicleMovement vm;
    public bool raceStarted = false;

    private void Start()
    {
        leaderboardpos = leaderboard.transform.position;
        newboardpos = new Vector3(leaderboardpos.x + Screen.width/1.15f, leaderboardpos.y);

        lapRef = GameObject.Find("LapCounter").GetComponent<Image>();
        startRef = GameObject.Find("RaceCounter").GetComponent<Image>();
        speedRef = GameObject.Find("Speedbar").GetComponent<Image>();
        inventoryImageRef = GameObject.Find("Inventory").GetComponent<Image>();
        placeImageRef = GameObject.Find("PlacePos").GetComponent<Image>();
        vm = GameObject.Find("Thruster").GetComponent<VehicleMovement>();

        //Start Countdown
        startScale = startRef.transform.localScale;

        startRef.transform.localScale = Vector2.zero;
        Invoke("StartCountDown",2.0f);
    }

    void StartCountDown()
    {
        startRef.transform.localScale = startScale;
        StartCoroutine(FadeAndShrink());
    }

    // Update is called once per frame
    void Update()
    {
        //Placement
        if (placeImageRef && vm)
        {
            Debug.Log(vm.curPosition - 1);
            placeImageRef.sprite = placeImages[vm.curPosition];
        }

        //Inventory
        if (vm.GetComponent<itemHandler>().itemType == 0)
        {
            inventoryImageRef.sprite = attackImages[vm.GetComponent<itemHandler>().itemLevel];
        }
        else if (vm.GetComponent<itemHandler>().itemType == 1)
        {
            inventoryImageRef.sprite = boostImages[vm.GetComponent<itemHandler>().itemLevel];
        }
        else if (vm.GetComponent<itemHandler>().itemType == 2)
        {
            inventoryImageRef.sprite = supportImages[vm.GetComponent<itemHandler>().itemLevel];
        }
        else
        {
            inventoryImageRef.sprite = emptyImage;
        }

        //Speedometer
        GameObject.Find("Speedometer").GetComponent<Text>().text = vm.curSpeed.ToString();
        if (vm.bonusSpeed > 5)
        {
            int random = Random.Range(1, 20);
            if (random == 4)
            {
                speedRef.sprite = speedImages[numberOfFrames + 2];
            }
            else if (random == 5)
            {
                speedRef.sprite = speedImages[numberOfFrames + 1];
            }
        }
        else if (Mathf.RoundToInt(numberOfFrames * (vm.curSpeed / vm.maxSpeed)) >= 0)
        {
            speedRef.sprite = speedImages[Mathf.RoundToInt(numberOfFrames * (vm.curSpeed / vm.maxSpeed))];
        }

        //Lap Counter
        if (vm.lap < 3)
        {
            lapRef.sprite = lapImages[vm.lap];
        }
    }

    //Race Start
    private IEnumerator FadeAndShrink()
    {
        GameObject.Find("GameManager").GetComponent<AnnouncerSystem>().PlaySound("Count", startFrame);
        float startAlpha = startRef.color.a;
        Vector3 startScale = startRef.transform.localScale;

        float elapsedTime = 0.0f;
        while (elapsedTime < 1.0f)
        {
            // Fade Out
            float alpha = Mathf.Lerp(startAlpha, 0.0f, elapsedTime / 1.0f);
            Color newColor = startRef.color;
            newColor.a = alpha;
            startRef.color = newColor;

            // Shrink
            Vector3 targetScale = new Vector3(0.5f, 0.5f, 1.0f);
            startRef.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / 1.0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Optionally, you can destroy the GameObject after fading out
        if (startFrame < startImages.Length-1)
        {
            //Play announcer sound

            if (startFrame == startImages.Length - 2)//Game START!
            {
                raceStarted = true;
                startTime = Time.time;
                InvokeRepeating("Timer", 0.01f, 0.01f);
            }
            startFrame++;
            startRef.sprite = startImages[startFrame];
            startRef.transform.localScale = startScale;
            startRef.color = new Color(1, 1, 1, 1);
            StartCoroutine(FadeAndShrink());
        }
        else
        {
            Destroy(startRef);
        }
    }

    //Timer
    void Timer()
    {
        if (!raceStarted)
        {
            return;
        }
        string TimerDisplay = string.Format("{0:00}:{1:00}:{2:00}",minutes, seconds, miliseconds);
        miliseconds++;
        s_miliseconds++;
        if (miliseconds > 99)
        {
            miliseconds = 0;
            s_miliseconds = 0;
            seconds++;
            s_seconds++;
            if (seconds > 59)
            {
                seconds = 0;
                s_seconds = 0;
                minutes++;
                s_minutes++;
            }
        }
        // Set Timer
        GameObject.Find("Timer").GetComponent<Text>().text = TimerDisplay;
    }

    //Move Leaderboard
    public IEnumerator MoveLeaderboard()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < 1f)
        {
            // Calculate the normalized time (a value between 0 and 1) based on the elapsed time and duration
            float t = elapsedTime / 1f;

            // Use Lerp to interpolate the position between the start and end points
            leaderboard.transform.position = Vector3.Lerp(leaderboardpos, newboardpos, t);

            // Increment the elapsed time based on the time since the last frame
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the object reaches the exact end position
        leaderboard.transform.position = newboardpos;

        // Optionally, perform any additional actions when the interpolation is complete
        Debug.Log("Interpolation Complete!");
    }
}
