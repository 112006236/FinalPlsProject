using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ObjectiveType
{
    Shrine,
    EnemyArea,
    Cage
}

[System.Serializable]
public class ArenaObjective
{
    public ObjectiveType type;
    public int count;
}

public class ArenaControl : MonoBehaviour
{
    public static ArenaControl Instance;

    [Header("Arena Settings")]
    public int totalObjectives = 6;
    public List<ArenaObjective> assignedObjectives = new List<ArenaObjective>();

    [Header("Objective Text")]
    public TextMeshProUGUI shrineLabel;
    public TextMeshProUGUI enemyLabel;
    public TextMeshProUGUI cageLabel;

    [Header("Win UI")]
    public GameObject youWinUI;

    private Dictionary<ObjectiveType, int> completedByType = new Dictionary<ObjectiveType, int>();
    private HashSet<ObjectiveType> powerUpGrantedFor = new HashSet<ObjectiveType>();
    private int totalCompleted = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AssignRandomObjectives();
        InitializeCompletionTracking();
        UpdateUI();
        DebugObjectives();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            NotifyObjectiveCompleted(ObjectiveType.Cage);
        } 
        else if (Input.GetKeyDown(KeyCode.O))
        {
            NotifyObjectiveCompleted(ObjectiveType.EnemyArea);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            NotifyObjectiveCompleted(ObjectiveType.Shrine);
        }
    }

    #region Assignment
    private void AssignRandomObjectives()
    {
        assignedObjectives.Clear();
        int remaining = totalObjectives;

        int shrineCount = Random.Range(1, Mathf.Max(2, remaining - 1));
        remaining -= shrineCount;

        int enemyCount = Random.Range(1, Mathf.Max(2, remaining));
        remaining -= enemyCount;

        int cageCount = remaining;

        assignedObjectives.Add(new ArenaObjective { type = ObjectiveType.Shrine, count = shrineCount });
        assignedObjectives.Add(new ArenaObjective { type = ObjectiveType.EnemyArea, count = enemyCount });
        assignedObjectives.Add(new ArenaObjective { type = ObjectiveType.Cage, count = cageCount });
    }

    private void InitializeCompletionTracking()
    {
        completedByType.Clear();
        foreach (var obj in assignedObjectives)
            completedByType[obj.type] = 0;
    }

    private void DebugObjectives()
    {
        Debug.Log("🎯 Arena Objectives Assigned:");
        foreach (var obj in assignedObjectives)
            Debug.Log($"- {obj.type}: {obj.count}");
    }
    #endregion

    #region Registration
    public void RegisterShrine(CaptureShrine shrine)
    {
        shrine.OnShrineCaptured += () => NotifyObjectiveCompleted(ObjectiveType.Shrine);
    }

    public void RegisterArea(EnemySpawner spawner)
    {
        spawner.OnAreaCleared += () => NotifyObjectiveCompleted(ObjectiveType.EnemyArea);
    }

    public void RegisterCage(MultiHealthObject cage)
    {
        cage.OnDestroyed += () => NotifyObjectiveCompleted(ObjectiveType.Cage);
    }
    #endregion

    #region Completion
    private void NotifyObjectiveCompleted(ObjectiveType type)
    {
        int maxCount = assignedObjectives.Find(o => o.type == type).count;
        int currentCount = completedByType[type];

        if (currentCount >= maxCount)
            return;

        completedByType[type]++;
        totalCompleted++;

        UpdateUI();

        // Power-up per completed category
        if (completedByType[type] == maxCount && !powerUpGrantedFor.Contains(type))
        {
            powerUpGrantedFor.Add(type);

            bool levelWin = completedByType[ObjectiveType.Cage] + completedByType[ObjectiveType.EnemyArea] + completedByType[ObjectiveType.Shrine] == 6;

            PowerUpManager.Instance.ShowChoices(levelWin);
        }

        if (totalCompleted >= totalObjectives)
            OnArenaCompleted();
    }

    private void OnArenaCompleted()
    {
        Debug.Log("✅ All objectives completed! Arena cleared!");

        // if (youWinUI != null)
           // youWinUI.SetActive(true);

        // PowerUpManager.Instance.ShowChoices();
    }
    #endregion

    #region UI
    private void UpdateUI()
    {
        foreach (var obj in assignedObjectives)
        {
            int completed = completedByType[obj.type];
            int required = obj.count;

            string text = $"{completed}/{required} {obj.type} quests completed";

            switch (obj.type)
            {
                case ObjectiveType.Shrine:
                    if (shrineLabel != null)
                        shrineLabel.text = text;
                    break;

                case ObjectiveType.EnemyArea:
                    if (enemyLabel != null)
                        enemyLabel.text = text;
                    break;

                case ObjectiveType.Cage:
                    if (cageLabel != null)
                        cageLabel.text = text;
                    break;
            }
        }
    }
    #endregion
}
