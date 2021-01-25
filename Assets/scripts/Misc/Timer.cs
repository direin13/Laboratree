using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiscFunctions;

public class Timer : MonoBehaviour
{
    private readonly Dictionary<string, float> timeStamps = new Dictionary<string, float>();
    private float prevSec;
    private bool secPassed;

    // Start is called before the first frame update
    void Start()
    {
        prevSec = (float)Time.time;
        secPassed = false;
    }

    public void Set(string name, float seconds)
    {
        seconds = NumOp.Cutoff(seconds, 0, seconds);
        timeStamps.Add(name, prevSec + seconds);
    }

    public float Get(string name)
    {
        return timeStamps[name];
    }

    public void Change(string name, float seconds)
    {
        seconds = NumOp.Cutoff(seconds, 0, seconds);
        timeStamps[name] = timeStamps[name] + (seconds - timeStamps[name]);
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

    public bool Tick()
    {
        return secPassed;
    }

    // Update is called once per frame
    void Update()
    {
        if ((float)Time.time - prevSec >= 1f)
        {
            secPassed = true;
            prevSec = (float)(Time.time);
        }
        else
        {
            secPassed = false;
        }
    }
}
