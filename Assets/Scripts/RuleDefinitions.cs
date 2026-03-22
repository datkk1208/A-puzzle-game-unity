using System;

public enum SubjectType { Player, Block }
public enum ModifierType { Can, Cannot }
public enum ActionType { Move, Touch }
public enum TargetType { None, Red, Blue, Goal }

[Serializable]
public class GameRule
{
    public SubjectType Subject;
    public ModifierType Modifier;
    public ActionType Action;
    public TargetType Target;

    public GameRule(SubjectType subject, ModifierType modifier, ActionType action, TargetType target = TargetType.None)
    {
        Subject = subject;
        Modifier = modifier;
        Action = action;
        Target = target;
    }
}