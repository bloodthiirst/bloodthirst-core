using Bloodthirst.Core.Singleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputUtils : BSingleton<InputUtils>
{
    [ShowInInspector]
    [ReadOnly]
    private Dictionary<KeyCode, List<float>> pressHistory;

    [ShowInInspector]
    [ReadOnly]
    private IReadOnlyList<KeyCode> allKeyCodes;

    [SerializeField]
    [ReadOnly]
    private bool isEnabled;

    [SerializeField]
    [Tooltip("maximum interval time between presses to be considered multiple click (in seconds)")]
    [Range(0, 1)]
    private float pressInterval = default;

    [Header("Debug / Test section")]

    [SerializeField]
    private bool isDebugTestEnabled = default;

    [SerializeField]
    [Range(1, 5)]
    private int testCount = default;

    public override void OnSetupSingleton()
    {
        Initialize();
    }

    private void Initialize()
    {
        // cache all the keycode enum values

        allKeyCodes = new List<KeyCode>((KeyCode[])Enum.GetValues(typeof(KeyCode)));

        // create teh dictionary to collect the press time

        pressHistory = new Dictionary<KeyCode, List<float>>();

        foreach (KeyCode keycode in allKeyCodes)
        {
            if (!pressHistory.ContainsKey(keycode))
                pressHistory.Add(keycode, new List<float>());
        }

        isEnabled = true;
    }


    void Update()
    {
        if (!isEnabled)
            return;

        foreach (KeyValuePair<KeyCode, List<float>> kv in pressHistory)
        {
            if (Input.GetKeyDown(kv.Key))
            {
                kv.Value.Add(Time.unscaledTime);
            }
        }

        if (isDebugTestEnabled)
        {
            foreach (KeyCode key in allKeyCodes)
            {
                if (GetMultiClick(key, testCount))
                {
                    Debug.Log(key.ToString() + " has been pressed " + testCount + " times");
                }
            }
        }

    }

    /// <summary>
    /// Check for multi-press for a specific key
    /// </summary>
    /// <param name="keyCode">keycode to check</param>
    /// <param name="pressCount">number of clicks to check</param>
    /// <returns></returns>
    public bool GetMultiClick(KeyCode keyCode, int pressCount)
    {
        if (pressCount < 1)
        {
            Debug.Log("pressCount can't be less than 1");
        }

        if (pressHistory[keyCode].Count < pressCount)
            return false;

        if (pressHistory[keyCode].Last() != Time.unscaledTime)
            return false;

        List<float> listRef = pressHistory[keyCode];

        int counter = 1;

        for (int i = listRef.Count - 2; i > -1; i--)
        {
            float diff = listRef[i + 1] - listRef[i];

            if (diff > pressInterval)
            {
                return false;
            }

            counter++;

            if (counter == pressCount)
                break;
        }

        return true;
    }
}
