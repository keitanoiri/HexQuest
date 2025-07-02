using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.GraphicsBuffer;
using System.Threading.Tasks;




namespace ArtsHandlers
{
    interface IArtEffectHandler
    {
        Task Apply(Unit caster, Vector3Int target, string arg);
    }

    public class ArtsActionDatabase
    {
        static readonly Dictionary<string, IArtEffectHandler> ACTIONHANDLERS = new()
        {
            { "DMG",   new DamageHandler()  },
            //{ "BLEED", new BleedHandler()   },
            { "BLK",   new BlockHandler()   },
            //{ "PUSH",  new PushHandler()    }
            {"RESET", new ResetHandler()},
            {"ZEROPOINT", new ZeroPointHandler() },
            {"TURNEND",new TurnEndHandler()},
            {"MOVE",new MoveHandler()},
            {"MODECHENGE",new ModeChengeHandler()}
        };

        public static async Task ResolveArt(Arts arts, Unit user, Vector3Int target)
        {
            foreach (var tag in arts.statusOps)          //["DMG:4","BLEED:2"]
            {
                
                var parts = tag.Split(':');
                var key = parts[0];                   // "DMG"
                var arg = parts.Length > 1 ? parts[1] : null;

                

                if (ACTIONHANDLERS.TryGetValue(key, out var h))
                    await h.Apply(user, target, arg);      // ���ʔ����I
                else
                    Debug.LogWarning($"���o�^�^�O {key}");
            }
        }

        public static List<Vector3Int> ArtsTargets(Arts arts, Unit user)
        {
            List<Vector3Int> TargetCells = new List<Vector3Int>();
            switch (arts.rangeKey)
            {
                case "Adjacent":
                    TargetCells.AddRange(HexUtil.GetNeighborsCell(user.CELL));
                    break;
                case "Self":
                    TargetCells.Add(user.CELL);
                    break;
            }

            foreach (string tag in arts.ConditionOps)        
            {
                switch (tag)
                {
                    case "ALL": break;
                    case "EMPTY":
                        /* �w�蔻��ɍ��v����Z�������O ---------------*/
                        TargetCells.RemoveAll(cell =>
                        {
                            // TODO: �����Ɂg���O�����h���������Ă�������
                            // ��: ���̃Z���Ƀ��j�b�g�����邩�^�n�`����Q���� ��
                            bool shouldRemove = GameManager.UnitPos.ContainsKey(cell);
                            return shouldRemove;
                        });
                        break;
                    case "GROUND":
                        /* �w�蔻��ɍ��v����Z�������O ---------------*/
                        TargetCells.RemoveAll(cell =>
                        {
                            // TODO: �����Ɂg���O�����h���������Ă�������
                            // ��: ���̃Z���Ƀ��j�b�g�����邩�^�n�`����Q���� ��
                            TerrainTile tile = HexUtil.GetTerrainTile(cell);
                            if(tile == null || !tile.walkable) return true;
                            else return false;
                        });
                        break;
                }
            }

            return TargetCells;
        }





    }

    


    public class DamageHandler : IArtEffectHandler
    {
        public Task Apply(Unit c, Vector3Int t, string arg)
        {
            int dmg = int.Parse(arg);
            int EnemyID;
            if (GameManager.UnitPos.TryGetValue(t, out EnemyID))
            {
                Unit targetUnit = GameManager.AllUnits[EnemyID];
                Debug.Log("�U��");
                if (targetUnit != null)c.Attack(targetUnit,dmg);
            }
            return Task.CompletedTask;
        }
    }

    public class BlockHandler : IArtEffectHandler
    {
        public Task Apply(Unit c, Vector3Int t, string arg)
        {
            Debug.Log("Guard");
            int stack = int.Parse(arg);
            c.status.Add("Guard", stack);

            return Task.CompletedTask;
        }
    }

    public class MoveHandler : IArtEffectHandler
    {
        public async Task Apply(Unit c, Vector3Int t, string arg)
        {
            //t�s�������ꏊ
            await c.Move(t, 0.1f);
        }
    }

    public class ResetHandler : IArtEffectHandler
    {
        public Task Apply(Unit c, Vector3Int t, string arg)
        {
            foreach(KeyValuePair<Vector2Int,Arts> resetarts in GameManager.AllArts)
            {
                if (resetarts.Value.type != ArtsType.Special)
                {
                    resetarts.Value.CANUSE = true;
                }
            }
            return Task.CompletedTask;
        }
    }

    public class ZeroPointHandler: IArtEffectHandler
    {
        public Task Apply(Unit c, Vector3Int t,string arg)
        {
            GameManager.PlayerCombatInsex = new Vector2Int(0, 0);
            return Task.CompletedTask;
        }
    }

    public class TurnEndHandler : IArtEffectHandler
    {
        public Task Apply(Unit c, Vector3Int t, string arg)
        {
            PlayerTurnBehaviour.OnTurnEnd();//�^�[�����I���鏈��
            return Task.CompletedTask;
        }
    }


    public class ModeChengeHandler : IArtEffectHandler
    {
        public Task Apply(Unit c, Vector3Int t, string arg)
        {
            foreach (KeyValuePair<Vector2Int, Arts> resetarts in GameManager.AllArts)
            {
                resetarts.Value.CANUSE = true; 
            }
            PlayerController.IsCombat = false;
            return Task.CompletedTask;
        }
    }

}
