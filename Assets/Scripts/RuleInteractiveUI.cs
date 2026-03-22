using UnityEngine;
using UnityEngine.UI;
using TMPro; // Thêm thư viện TextMeshPro

public class RuleInteractiveUI : MonoBehaviour
{
    public int ruleIndexToModify = 0;
    private Button button;
    private TextMeshProUGUI buttonText; // Đổi sang kiểu TextMeshProUGUI

    private void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>(); // Lấy component TextMeshProUGUI

        button.onClick.AddListener(OnRuleTapped);
        RuleManager.Instance.OnRuleChanged += UpdateUIText;

        UpdateUIText();
    }

    private void OnRuleTapped()
    {
        RuleManager.Instance.ToggleRuleModifier(ruleIndexToModify);
    }

    private void UpdateUIText()
    {
        if (RuleManager.Instance.activeRules.Count > ruleIndexToModify)
        {
            var rule = RuleManager.Instance.activeRules[ruleIndexToModify];
            buttonText.text = $"{rule.Subject} {rule.Modifier} {rule.Action}";
        }
    }

    private void OnDestroy()
    {
        if (RuleManager.Instance != null)
        {
            RuleManager.Instance.OnRuleChanged -= UpdateUIText;
        }
    }
}