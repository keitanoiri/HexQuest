using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitTurnBehaviour
{
    UniTask ExecuteTurn(Unit self);
}
