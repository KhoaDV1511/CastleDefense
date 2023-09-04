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
public class ManaUse : ASignal<string, int>{}
public class ArcherSkills : ASignal{}
public class CombatantSkills : ASignal{}
public class CoolDownBarArcher : ASignal<int>{}
public class CoolDownBarCombatant : ASignal<int>{}