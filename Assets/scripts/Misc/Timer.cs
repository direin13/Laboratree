using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiscFunctions;

public class Timer : MonoBehaviour
{
    private readonly Dictionary<string, float> timeStamps = new Dictionary<string, float>();
    private readonly Dictionary<string, float> onGoingTimeStamps = new Dictionary<string, float>();
    private readonly Dictionary<string, float> tickSpeeds = new Dictionary<string, float>();
    public float speed;
    public readonly float maxSpeed = 0.00001f;
    private bool hasTicked = false;
    public bool getTicks;
    public int timeElapsed; //reference of how many units of time has passed

    // Start is called before the first frame update
    void Start()
    {
        Set("<<tick>>", maxSpeed, maxSpeed);
        speed = NumOp.Cutoff(speed, 0f, 1f);
    }

    public bool Tick()
    {
        return hasTicked;
    }

    public void Set(string name, float newTime, float tickSpeed)
    {
        //set a countdown timer
        newTime = NumOp.Cutoff(newTime, 0, newTime);
        if (timeStamps.ContainsKey(name) != true)
        {
            timeStamps.Add(name, newTime);
            onGoingTimeStamps.Add(name, Time.time);
            tickSpeeds.Add(name, tickSpeed);
        }
        else
        {
            timeStamps[name] = newTime;
            onGoingTimeStamps[name] = Time.time;
            tickSpeeds[name] = tickSpeed;
        }
    }

    public float Get(string name)
    {
        return timeStamps[name];
    }

    public void Change(string name, float amount)
    {
        //changes current time stamp by given amount
        amount = NumOp.Cutoff(amount, 0, amount);
        timeStamps[name] = timeStamps[name] + (amount - timeStamps[name]);
    }

    public bool TimeUp(string name)
    {
        return (timeStamps[name] <= 0);
    }

    public void Remove(string name)
    {
        timeStamps.Remove(name);
    }

    public void PrintTime(string name)
    {
        print(name + ": " + timeStamps[name].ToString() + " seconds");
    }


    // Update is called once per frame
    void Update()
    {
        string[] keys = new string[timeStamps.Count];
        int i = 0;

        foreach (KeyValuePair<string, float> kvp in timeStamps)
        {
            keys[i] = kvp.Key;
            i++;
        }

        foreach (string key in keys)
        {
            float dist = Time.time - onGoingTimeStamps[key];
            if (dist >= tickSpeeds[key] * speed)
            {
                //countdown the timer
                onGoingTimeStamps[key] = onGoingTimeStamps[key] + dist;
                timeStamps[key] = NumOp.Cutoff(timeStamps[key] - (tickSpeeds[key] * speed), 0, timeStamps[key]);

                if (key == "<<tick>>" && getTicks)
                {
                    hasTicked = true;
                    timeElapsed++;
                }
            }

            else if (key == "<<tick>>")
            {
                hasTicked = false;
            }
        }

        if (getTicks != true)
        {
            hasTicked = false;
        }

        if (TimeUp("<<tick>>"))
        {
            Set("<<tick>>", maxSpeed, maxSpeed);
        }
    }
}