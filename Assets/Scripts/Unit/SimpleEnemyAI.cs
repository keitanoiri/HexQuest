using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyAI : MonoBehaviour, IUnitTurnBehaviour
{
    public async UniTask ExecuteTurn(Unit self)
    {
        Debug.Log("ExcuteEnemy");
        await UniTask.Delay(100);//0.1�b�ҋ@
        self.HP += 1;//���^�[����
        await UniTask.Delay(100);
        self.HP += 1;//���^�[����
    }

}
