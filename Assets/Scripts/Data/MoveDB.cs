using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();
        var moveArray = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveArray)
        {
            if (moves.ContainsKey(move.Name))
            {
                Debug.LogError($"같은 이름의 스킬이 존재함 {move.Name}");
                continue;
            }
            moves[move.Name] = move;
        }
    }

    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"해당 스킬은 없습니다{name}");
            return null;
        }

        return moves[name];
    }
}
