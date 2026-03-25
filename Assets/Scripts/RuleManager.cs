using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    public static RuleManager Instance { get; private set; }

    public List<GameRule> activeRules = new List<GameRule>();

    // Observer Pattern: Phát sự kiện khi luật thay đổi
    public event Action OnRuleChanged;

    //private void Awake()
    //{
    //    if (Instance == null) Instance = this;
    //    else Destroy(gameObject);
    //}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Khởi tạo luật ngay trong Awake để UI kịp lấy dữ liệu
            activeRules.Add(new GameRule(SubjectType.Player, ModifierType.Cannot, ActionType.Move));
        }
        else Destroy(gameObject);
    }

    // Kiểm tra xem một hành động có bị cấm bởi luật nào không [cite: 113, 124]
    public bool IsActionAllowed(SubjectType subject, ActionType action, TargetType target = TargetType.None)
    {
        foreach (var rule in activeRules)
        {
            if (rule.Subject == subject && rule.Action == action && rule.Target == target)
            {
                if (rule.Modifier == ModifierType.Cannot) return false;
            }
        }
        return true; // Mặc định là Can nếu không có luật cấm
    }

    // UI gọi hàm này để đổi Modifier (Can <-> Cannot) [cite: 22, 105]
    public void ToggleRuleModifier(int ruleIndex)
    {
        if (ruleIndex >= 0 && ruleIndex < activeRules.Count)
        {
            var rule = activeRules[ruleIndex];
            rule.Modifier = (rule.Modifier == ModifierType.Can) ? ModifierType.Cannot : ModifierType.Can;

            Debug.Log($"Rule changed: {rule.Subject} {rule.Modifier} {rule.Action}");
            OnRuleChanged?.Invoke(); // Cập nhật game state [cite: 111]
        }
    }
}
    //commit again
    //Commit 2
    //Commit 3
    //commit 4