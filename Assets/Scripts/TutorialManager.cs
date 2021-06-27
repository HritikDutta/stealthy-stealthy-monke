using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public bool skipTutorial = false;
    public GameObject[] promptPanels;

    private List<TutorialTrigger> triggers = new List<TutorialTrigger>();
    private int currentStageIndex = 0;

    private Transform oldTarget;
    private Vector3 oldScale;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        if (skipTutorial)
            return;

        Transform triggerParent = transform.Find("Triggers");
        foreach(Transform trig in triggerParent)
        {
            TutorialTrigger trigger = trig.GetComponent<TutorialTrigger>();
            trigger.Disable();
            triggers.Add(trigger);
        }
    }

    void Start()
    {
        if (skipTutorial)
            return;

        currentStageIndex = 0;
        triggers[currentStageIndex].Enable();
    }

    void Update()
    {
        if (skipTutorial)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            promptPanels[currentStageIndex].SetActive(false);
            Time.timeScale = 1f;

            currentStageIndex++;
            if (currentStageIndex < triggers.Count)
                triggers[currentStageIndex].Enable();
        }
    }

    private void _TriggerNext()
    {
        Time.timeScale = 0f;
        promptPanels[currentStageIndex].SetActive(true);
        triggers[currentStageIndex].Disable();
    }

    public static void TriggerNext()
    {
        if (instance.skipTutorial)
            return;

        instance._TriggerNext();
    }
}
