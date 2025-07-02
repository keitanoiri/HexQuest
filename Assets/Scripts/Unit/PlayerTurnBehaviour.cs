using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurnBehaviour : MonoBehaviour,IUnitTurnBehaviour
{

    static UniTaskCompletionSource<bool> IsTurnEnd;
    [SerializeField] Button TurnEndButton;

    [SerializeField]PlayerController PlayerController;
    public async UniTask ExecuteTurn(Unit self)
    {
        Debug.Log("ExcutePlayer");
        PlayerController.StartTurn();
        //ターンエンドするまで待機
        IsTurnEnd = new UniTaskCompletionSource<bool>();
        await IsTurnEnd.Task;
    }

    public static void OnTurnEnd() => IsTurnEnd.TrySetResult(true);
    
}
