using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class InputLogger
{
    InputDiary inputLog;
    Player player;
    int debug;

    public InputLogger(Player p)
    {
        player = p;
        inputLog = new InputDiary();
    }


    [Serializable]
    public class InpDiaryEntry
    {
        public int frame;
        public Dictionary<string, double> inputs;

        public InpDiaryEntry(int i, Dictionary<string, double> d)
        {
            frame = i;
            inputs = d;
        }

        public void Add(String s, double d)
        {
            inputs.Add(s, d);
        }
    }

    [Serializable]
    public class InputDiary
    {
        public List<InpDiaryEntry> entryList;

        public InputDiary()
        {
            entryList = new List<InpDiaryEntry>();
        }

        public void Add(InpDiaryEntry entry)
        {
            entryList.Add(entry);
        }

        public bool checkifTime(int index, int pFrame)
        {
            if (entryList == null || index >= entryList.Count)
            {
                return false;
            }

            InpDiaryEntry entry = entryList[index];
            return entry.frame == pFrame;
        }

        public void interpretBinding(string s, double d, PlayerInput pi)
        {

            switch (s)
            {
                case "hj":
                    pi.jump = b(d);
                    break;

                case "j":
                    pi.justJumped = b(d);
                    break;

                case "mv":
                    pi.moveDirection = (float) d;
                    break;

                case "lc":
                    pi.justClicked = b(d);
                    break;

                case "rc":
                    pi.holdingRightClick = b(d);
                    break;

                case "scr":
                    pi.scrollDelta = (float) d;
                    break;

                case "Q":
                    pi.Q = b(d);
                    break;

                case "E":
                    pi.E = b(d);
                    break;

                case "R":
                    pi.R = b(d);
                    break;

                case "F":
                    pi.justF = b(d);
                    break;
            }
        }

        public void getEntry(int index, PlayerInput current)
        {
            InpDiaryEntry entry = entryList[index];

            foreach (var input in entry.inputs)
            {
                interpretBinding(input.Key, input.Value, current);
            }
        }
    }

    public void checkControllDiffs(int frame)
    {
        Dictionary<string, double> entryDict = new Dictionary<string, double>();
        InpDiaryEntry ibl = new InpDiaryEntry(frame, entryDict);
        bool changed = false;

        PlayerInput currentInput = player.currentInput;
        PlayerInput previousInput = player.previousInput;

        // jump
        if (currentInput.jump != previousInput.jump)
        {
            entryDict.Add("hj", d(currentInput.jump));
            // Debug.Log("changed hold jump status" + debug);
            changed = true;
        }

        // just jumped
        if (currentInput.justJumped != previousInput.justJumped)
        {
            entryDict.Add("j", d(currentInput.justJumped));
            // Debug.Log("changed just jumped status" + debug);
            changed = true;
        }

        // move
        if (currentInput.moveDirection != previousInput.moveDirection)
        {
            entryDict.Add("mv", currentInput.moveDirection);
            // Debug.Log("changed move status" + debug);
            changed = true;

        }

        // just left click
        if (currentInput.justClicked != previousInput.justClicked)
        {
            entryDict.Add("lc", d(currentInput.justClicked));
            // Debug.Log("changed left click status" + debug);
            changed = true;

        }

        // hold right click
        if (currentInput.holdingRightClick != previousInput.holdingRightClick)
        {
            entryDict.Add("lc", d(currentInput.holdingRightClick));
            // Debug.Log("changed right click status" + debug);
            changed = true;

        }

        // scroll
        if (currentInput.scrollDelta != previousInput.scrollDelta)
        {
            entryDict.Add("scr", currentInput.scrollDelta);
            // Debug.Log("changed scroll status" + debug);
            changed = true;

        }

        // Q
        if (currentInput.Q != previousInput.Q)
        {
            entryDict.Add("Q", d(currentInput.Q));
            // Debug.Log("changed Q status" + debug);
            changed = true;

        }

        // E
        if (currentInput.E != previousInput.E)
        {
            entryDict.Add("E", d(currentInput.E));
            // Debug.Log("changed E status" + debug);
            changed = true;

        }

        // R
        if (currentInput.R != previousInput.R)
        {
            entryDict.Add("R", d(currentInput.R));
            // Debug.Log("changed R status" + debug);
            changed = true;

        }

        // F
        if (currentInput.justF != previousInput.justF)
        {
            entryDict.Add("F", d(currentInput.justF));
            // Debug.Log("changed justF status" + debug);
            changed = true;
        }

        if (changed)
        {
            inputLog.Add(ibl);
            debug++;
        }

    }

    public double d(bool b)
    {
        if (b)
        {
            return 1;
        }

        return 0;
    }

    public static bool b(double d)
    {
        if (d == 0)
        {
            return false;
        }

        return true;
    }
    public string json()
    {
        return JsonConvert.SerializeObject(inputLog);
    }
}
