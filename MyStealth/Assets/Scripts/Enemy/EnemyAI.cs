using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

	public float patrolSpeed = 2f;                          // 敌人巡逻速度
	public float chaseSpeed = 5f;                           // 敌人追踪速度
	public float chaseWaitTime = 5f;                        // 到达追踪地点后的等待时间
	public float patrolWaitTime = 1f;                       // 到达每个巡逻路径点后的等待时间
	public Transform[] patrolWayPoints;                     // 储存路径点坐标的数组


	private EnemySight enemySight;                          // EnemySight脚本引用
	private NavMeshAgent nav;                               // nav mesh agent组件引用
	private Transform player;                               // 玩家位置引用
	private PlayerHealth playerHealth;                      // PlayerHealth脚本引用
	private LastPlayerSighting lastPlayerSighting;          // LastPlayerSighting脚本引用
	private float chaseTimer;                               // 追踪等待时间的计时器
	private float patrolTimer;                              // 巡逻等待时间的计时器
	private int wayPointIndex;                              // 路径点数组的索引


	void Awake ()
	{
		// 获取引用对象和组件
		enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		playerHealth = player.GetComponent<PlayerHealth>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}


	void Update ()
	{
		// 如果玩家在敌人视线内 且玩家生命值大于0
		if(enemySight.playerInSight && playerHealth.health > 0f) {
			// 射他
			Shooting();
		}
			

		// 如果玩家被发现(被听见或触发警报) 且生命值大于0
		else if(enemySight.personalLastSighting != lastPlayerSighting.resetPosition && playerHealth.health > 0f) {
			// 追踪他
//			Chasing();
		}
			
		// 否则
		else {
			// 老老实实巡逻
			// 老老实实巡逻
			//			Patrolling();Patrolling();
		}

	}


	void Shooting ()
	{
		// 射击时不让敌人移动
		nav.Stop();
	}


	void Chasing ()
	{
		// 从敌人当前位置到最后发现玩家位位置 创建一个向量 
		Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;

		// 如果玩家距离敌人较远
		if(sightingDeltaPos.sqrMagnitude > 4f) {
			// 让敌人跑向追踪位置
			nav.destination = enemySight.personalLastSighting;
		}
		// 移动速度设为追踪速度
		nav.speed = chaseSpeed;

		// 当敌人与目标点的距离 小于 可停止距离
		if (nav.remainingDistance < nav.stoppingDistance) {
			// 追踪等待时间的计时器开始计时
			chaseTimer += Time.deltaTime;

			// 当敌人等待了足够时间后（设定的5秒）
			if (chaseTimer >= chaseWaitTime) {
				// 重置最后发现玩家的全局位置和独立位置 重置计时器
				lastPlayerSighting.position = lastPlayerSighting.resetPosition;
				enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
				chaseTimer = 0f;
			}
		} else {
			// 如果敌人没有靠近玩家最后出现的位置 那么重置计时器（因为追踪位置不是固定的 玩家可能还会在其他位置触发警报 从而刷新追踪位置）
			chaseTimer = 0f;
		}
			
	}


	void Patrolling ()
	{
		// 移动速度等于巡逻速度
		nav.speed = patrolSpeed;

		// 如果没有目标点 或者 已接近目标巡逻点
		if (nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < nav.stoppingDistance) {
			// 巡逻等待时间的计时器开始计时
			patrolTimer += Time.deltaTime;

			// 等待时间过后
			if (patrolTimer >= patrolWaitTime) {
				// 移动索引至下一个位置
				if (wayPointIndex == patrolWayPoints.Length - 1) {
					wayPointIndex = 0;
				} else {
					wayPointIndex++;
				}
				// 重置计时器
				patrolTimer = 0;
			}
		} else {
			// 如果没有靠近任何一个目标点 重置计时器
			patrolTimer = 0;
		}
			
		// 目标点设为路径点
		nav.destination = patrolWayPoints[wayPointIndex].position;
	}
}
