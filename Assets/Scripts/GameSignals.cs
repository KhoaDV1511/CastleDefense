using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPosProjectile : ASignal<Vector3>{}
public class EnemyPosArrow : ASignal<Vector3>{}
public class CastlePos : ASignal<Vector3>{}
public class PosAIMove : ASignal<Vector3>{}
public class StopAIMoveAround : ASignal<bool>{}