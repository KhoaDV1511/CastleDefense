using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPosProjectile : ASignal<Vector3>{}
public class CastlePos : ASignal<Vector3>{}
public class PosAIMove : ASignal<Vector3>{}
public class QuantityEnemy : ASignal<int>{} // số lượng enemy
public class DameEnemy : ASignal<int>{}
public class OnStopGame : ASignal{}
public class StartFindEnemy : ASignal{}
public class ManaUse : ASignal<string, int>{}
public class ArcherSkills : ASignal{}
public class CombatantSkills : ASignal{}
public class ThunderSkills : ASignal{}
public class MagicianSkills : ASignal{}
public class CoolDownBarArcher : ASignal<int>{}
public class CoolDownBarCombatant : ASignal<int>{}
public class CoolDownBarThunder : ASignal<int>{}
public class CoolDownBarMagician : ASignal<int>{}
public class TimeReduce : ASignal<float>{}