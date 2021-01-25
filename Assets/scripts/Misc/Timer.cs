using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiscFunctions;

public class Timer : MonoBehaviour
{
    private readonly Dictionary<string, int> timeStamps = new Dictionary<string, int>();
    private int prevSec;
    private bool secPassed;

    // Start is called before the first frame update
    void Start()
    {
        prevSec = (int)Time.time;
        secPassed = false;
    }

    public void Set(string name, int seconds)
    {
        seconds = NumOp.Cutoff(seconds, 0, seconds);
        timeStamps.Add(name, prevSec + seconds);
    }

    public void Change(string name, int seconds)
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
        if ((int)Time.time > prevSec)
        {
            secPassed = true;
            prevSec = (int)(Time.time);
        }
        else
        {
            secPassed = false;
        }
    }
}
