using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChaiseEnemyAI : MonoBehaviour, IUnitTurnBehaviour
{
    [SerializeField] int DealDamage = 10;
    [SerializeField] int MaxMove = 4;

    public async UniTask ExecuteTurn(Unit self)
    {
        //プレイヤーが検知範囲内か？
        if (HexUtil.IsAdjacent(GameManager.AllUnits[1].CELL, self.CELL))
        {
            //隣接するので攻撃
            self.Attack(GameManager.AllUnits[1], DealDamage);
        }
        else
        {
            Vector3Int[] GoalCells = HexUtil.GetEnableMoveCell(GameManager.AllUnits[1].CELL, 1);
            Vector3Int GoalCell = GoalCells[Random.Range(0,GoalCells.Length - 1)];

            HexPathFinder pathF = GetComponent<HexPathFinder>();
            List<Vector3Int> root = pathF.FindPath(self.CELL, GoalCell);
            if (root == null) return;//ルートが無ければスキップ

            int i = 0;
            foreach (Vector3Int v in root)
            {
                if(self.CELL.z == v.z)
                {
                    await self.Move(v, 0.1f);
                }else
                {
                    await self.Jump(v, 0.5f, 0.1f);
                }
                i++;
                if(i>=MaxMove) break;
                
            }
        }
        
        //そうでなければ近づく
    }

}
