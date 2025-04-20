using System;
using System.Collections.Generic;
using UnityEngine;

public class InputLogger
{
    InputDiary inputLog;
    Player player;
    int debug;

    public InputLogger(Player p)
    {
        player = p;

        List<InputBinding<int, InpBindList>> entryList = new List<InputBinding<int, InpBindList>>();
        inputLog = new InputDiary(entryList);
    }

    [Serializable]
    public class InputBinding<S, T>
    {
        public InputBinding(S k, T v)
        {
            key = k;
            value = v;
        }

        public S key;
        public T value;
    }

    [Serializable]
    public class InpBindList
    {
        public List<InputBinding<string, double>> list;

        public InpBindList(List<InputBinding<string, double>> l)
        {
            list = l;
        }

        public void Add(InputBinding<string, double> binding)
        {
            list.Add(binding);
        }
    }

    [Serializable]
    public class InputDiary
    {
        public List<InputBinding<int, InpBindList>> list;

        public InputDiary(List<InputBinding<int, InpBindList>> l)
        {
            list = l;
        }

        public void Add(InputBinding<int, InpBindList> entry)
        {
            list.Add(entry);
        }

        public bool checkifTime(int index, int pFrame)
        {
            if (index >= list.Count)
            {
                return false;
            }

            InputBinding<int, InpBindList> binding = list[index];
            return binding.key == pFrame;
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
            InputBinding<int, InpBindList> binding = list[index];
            InpBindList ibl = binding.value;

            foreach (InputBinding<string, double> b in ibl.list)
            {
                interpretBinding(b.key, b.value, current);
            }
        }
    }

    InputBinding<string, double> newBinding(string s, double o)
    {
        return new InputBinding<string, double>(s, o);
    }

    public void checkControllDiffs()
    {
        InpBindList ibl = new InpBindList(new List<InputBinding<string, double>>());
        bool changed = false;

        PlayerInput currentInput = player.currentInput;
        PlayerInput previousInput = player.previousInput;

        // jump
        if (currentInput.jump != previousInput.jump)
        {
            ibl.Add(newBinding("hj", d(currentInput.jump)));
            // Debug.Log("changed hold jump status" + debug);
            changed = true;
        }

        // just jumped
        if (currentInput.justJumped != previousInput.justJumped)
        {
            ibl.Add(newBinding("j", d(currentInput.justJumped)));
            // Debug.Log("changed just jumped status" + debug);
            changed = true;
        }

        // move
        if (currentInput.moveDirection != previousInput.moveDirection)
        {
            ibl.Add(newBinding("mv", currentInput.moveDirection));
            // Debug.Log("changed move status" + debug);
            changed = true;

        }

        // just left click
        if (currentInput.justClicked != previousInput.justClicked)
        {
            ibl.Add(newBinding("lc", d(currentInput.justClicked)));
            // Debug.Log("changed left click status" + debug);
            changed = true;

        }

        // hold right click
        if (currentInput.holdingRightClick != previousInput.holdingRightClick)
        {
            ibl.Add(newBinding("rc", d(currentInput.holdingRightClick)));
            // Debug.Log("changed right click status" + debug);
            changed = true;

        }

        // scroll
        if (currentInput.scrollDelta != previousInput.scrollDelta)
        {
            ibl.Add(newBinding("scr", currentInput.scrollDelta));
            // Debug.Log("changed scroll status" + debug);
            changed = true;

        }

        // Q
        if (currentInput.Q != previousInput.Q)
        {
            ibl.Add(newBinding("Q", d(currentInput.Q)));
            // Debug.Log("changed Q status" + debug);
            changed = true;

        }

        // E
        if (currentInput.E != previousInput.E)
        {
            ibl.Add(newBinding("E", d(currentInput.E)));
            // Debug.Log("changed E status" + debug);
            changed = true;

        }

        // R
        if (currentInput.R != previousInput.R)
        {
            ibl.Add(newBinding("R", d(currentInput.R)));
            // Debug.Log("changed R status" + debug);
            changed = true;

        }

        // F
        if (currentInput.justF != previousInput.justF)
        {
            ibl.Add(newBinding("F", d(currentInput.justF)));
            // Debug.Log("changed justF status" + debug);
            changed = true;
        }

        if (changed)
        {
            InputBinding<int, InpBindList> entry = new InputBinding<int, InpBindList>(player.phyFrame, ibl);
            inputLog.Add(entry);
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
        return JsonUtility.ToJson(inputLog);
    }
}
