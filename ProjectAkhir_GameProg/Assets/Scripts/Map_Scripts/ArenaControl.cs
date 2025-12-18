using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public int count; // How many of this type are required
}

public class ArenaControl : MonoBehaviour
{
    public static ArenaControl Instance;

    [Header("Arena Settings")]
    public int totalObjectives = 6;
    public List<ArenaObjective> assignedObjectives = new List<ArenaObjective>();

    [Header("Progress Bars")]
    public Image shrineBarFill;
    public Image enemyBarFill;
    public Image cageBarFill;

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
        foreach (var obj in assignedObjectives)
            completedByType[obj.type] = 0;
    }

    private void DebugObjectives()
    {
        Debug.Log("🎯 Arena Objectives Assigned:");
        foreach (var obj in assignedObjectives)
        {
            Debug.Log($"- {obj.type}: {obj.count}");
        }
    }
    #endregion

    #region Registration
    public void RegisterShrine(CaptureShrine shrine)
    {
        shrine.OnShrineCaptured += () => NotifyObjectiveCompleted(ObjectiveType.Shrine);
        Debug.Log($"✔ Registered Shrine: {shrine.gameObject.name}");
    }

    public void RegisterArea(EnemySpawner spawner)
    {
        spawner.OnAreaCleared += () => NotifyObjectiveCompleted(ObjectiveType.EnemyArea);
        Debug.Log($"✔ Registered Enemy Area: {spawner.gameObject.name}");
    }

    public void RegisterCage(MultiHealthObject cage)
    {
        cage.OnDestroyed += () => NotifyObjectiveCompleted(ObjectiveType.Cage);
        Debug.Log($"✔ Registered Cage: {cage.gameObject.name}");
    }
    #endregion

    #region Completion
    private void NotifyObjectiveCompleted(ObjectiveType type)
    {
        int maxCount = assignedObjectives.Find(o => o.type == type).count;
        int currentCount = completedByType[type];

        if (currentCount >= maxCount)
        {
            Debug.Log($"❌ {type} completion ignored (already fulfilled).");
            return;
        }

        completedByType[type]++;
        totalCompleted++;

        Debug.Log($"🔥 {type} completed! {completedByType[type]}/{maxCount}");

        UpdateUI();

        // ⭐ NEW: Check if THIS objective type just finished
        if (completedByType[type] == maxCount && !powerUpGrantedFor.Contains(type))
        {
            powerUpGrantedFor.Add(type);
            Debug.Log($"PowerUpManager Instance = {PowerUpManager.Instance}");
            Debug.Log($"🎁 Power-Up triggered for completing {type}");
            PowerUpManager.Instance.ShowChoices();
        }

        // Arena fully completed (optional)
        if (totalCompleted >= totalObjectives)
        {
            OnArenaCompleted();
        }
    }


    private void OnArenaCompleted()
    {
        Debug.Log("✅ All objectives completed! Arena cleared!");

        if (youWinUI != null)
            youWinUI.SetActive(true);

        PowerUpManager.Instance.ShowChoices();
    }
    #endregion

    #region UI
    private void UpdateUI()
    {
        foreach (var obj in assignedObjectives)
        {
            int completed = completedByType[obj.type];
            int required = obj.count;

            float fillAmount = (required > 0) ? (float)completed / required : 0f;

            bool isDone = completed >= required;
            Color barColor = isDone ? Color.green : Color.white;

            switch (obj.type)
            {
                case ObjectiveType.Shrine:
                    if (shrineBarFill != null)
                    {
                        shrineBarFill.fillAmount = fillAmount;
                        shrineBarFill.color = barColor;
                    }
                    if (shrineLabel != null)
                        shrineLabel.text = $"{completed}/{required}";
                        Debug.Log($"{obj.type} FillAmount = {fillAmount}");
                    break;

                case ObjectiveType.EnemyArea:
                    if (enemyBarFill != null)
                    {
                        enemyBarFill.fillAmount = fillAmount;
                        enemyBarFill.color = barColor;
                                Debug.Log($"{obj.type} FillAmount = {fillAmount}");
                    }
                    if (enemyLabel != null)
                        enemyLabel.text = $"{completed}/{required}";
                    break;

                case ObjectiveType.Cage:
                    if (cageBarFill != null)
                    {
                        cageBarFill.fillAmount = fillAmount;
                        cageBarFill.color = barColor;
                    }
                    if (cageLabel != null)
                        cageLabel.text = $"{completed}/{required}";
                        Debug.Log($"{obj.type} FillAmount = {fillAmount}");
                    break;
            }
        }
    }
    #endregion
}
