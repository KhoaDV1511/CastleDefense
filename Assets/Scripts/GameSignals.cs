using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPosProjectile : ASignal<Vector3>{}
public class CastlePos : ASignal<Vector3>{}
public class PosAIMove : ASignal<Vector3>{}
public class QuantityEnemy : ASignal<int>{}
public class DameEnemy : ASignal<int>{}
public class OnStopGame : ASignal{}
public class StartFindEnemy : ASignal{}