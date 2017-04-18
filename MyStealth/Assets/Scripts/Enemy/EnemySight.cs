using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySight : MonoBehaviour {

	public float fieldOfViewAngle = 110f;           // 敌人的视角范围
	public bool playerInSight;                      // 判断敌人是否看见玩家
	public Vector3 personalLastSighting;            // 每个敌人独立的变量 用于储存敌人听到玩家脚步声/喊叫声的位置

	private NavMeshAgent nav;                       // 引用NavMeshAgent组件
	private SphereCollider col;                     // 引用sphere collider触发器组件
	private Animator anim;                          // 引用Animator组件
	private LastPlayerSighting lastPlayerSighting;  // 引用LastPlayerSighting脚本
	private GameObject player;                      // 引用玩家对象
	private Animator playerAnim;                    // 引用玩家的Animator组件
	private PlayerHealth playerHealth;              // 引用PlayerHealth脚本
	private HashIDs hash;                           // 引用HashIDs脚本
	private Vector3 previousSighting;               // 上一帧玩家被发现的位置


	void Awake ()
	{
		// 获取引用对象和组件
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerAnim = player.GetComponent<Animator>();
		playerHealth = player.GetComponent<PlayerHealth>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		// 把敌人单独发现的位置 和上一帧发现玩家的位置都重置（1000,1000,1000）
		personalLastSighting = lastPlayerSighting.resetPosition;
		previousSighting = lastPlayerSighting.resetPosition;
	}


	void Update ()
	{
		// 如果发现玩家位置的全局变量发生改变
		if(lastPlayerSighting.position != previousSighting)
			// 那么每个敌人的单独位置变量都将等于全局变量
			personalLastSighting = lastPlayerSighting.position;

		// 并把上一帧发现玩家的位置等于全局变量
		previousSighting = lastPlayerSighting.position;

		// 如果玩家HP值等于0 也就是玩家还活着
		if(playerHealth.health > 0f)
			// 把playerInSight的值传递到Animator中相应的参数 也就是设为true
			anim.SetBool(hash.playerInSightBool, playerInSight);
		else
			// 否则设为false
			anim.SetBool(hash.playerInSightBool, false);
	}


	void OnTriggerStay (Collider other)
	{
		// 如果玩家进入触发器范围内
		if(other.gameObject == player)
		{
			// 默认玩家还没有被发现
			playerInSight = false;

			// 创建敌人到玩家位置的向量 然后获取它与敌人正前方向量的夹角
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);

			// 如果夹角小于敌人视角的二分之一
			if(angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;

				// 如果从敌人到玩家的光线投射碰撞到某个对象
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
				{
					// 如果碰撞对向是玩家
					if(hit.collider.gameObject == player)
					{
						// 判定玩家被发现
						playerInSight = true;

						// 让玩家最后被发现的位置的全局变量 等于玩家当前位置
						lastPlayerSighting.position = player.transform.position;
					}
				}
			}

			// 储存Animator中的跑步状态和喊叫状态为hash
			int playerLayerZeroStateHash = playerAnim.GetCurrentAnimatorStateInfo(0).nameHash;
			int playerLayerOneStateHash = playerAnim.GetCurrentAnimatorStateInfo(1).nameHash;

			// 如果玩家在跑步状态 或者在喊叫状态时
			if(playerLayerZeroStateHash == hash.locomotionState || playerLayerOneStateHash == hash.shoutState)
			{
				// 并且玩家在敌人听觉范围内
				if(CalculatePathLength(player.transform.position) <= col.radius)
					// 敌人单独储存玩家位置的变量 等于玩家当前位置
					personalLastSighting = player.transform.position;
			}
		}
	}


	void OnTriggerExit (Collider other)
	{
		// 如果玩家离开触发器范围
		if(other.gameObject == player)
			// 判定玩家没有被发现
			playerInSight = false;
	}


	float CalculatePathLength (Vector3 targetPosition)
	{
		
		// 创建路径 并让路径基于目标位置 (玩家位置)
		NavMeshPath path = new NavMeshPath();

		if(nav.enabled)
			nav.CalculatePath(targetPosition, path);
		// 创建一个数组 长度等于path.cornners.Length+2
		Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

		// 数组第一个值为敌人位置
		allWayPoints[0] = transform.position;

		// 最后一个值为玩家位置
		allWayPoints[allWayPoints.Length - 1] = targetPosition;

		// 把path.corners数组的值 添加到新数组中
		for(int i = 0; i < path.corners.Length; i++)
		{
			allWayPoints[i + 1] = path.corners[i];
		}

		// 创建路径长度变量 并赋予初始值0
		float pathLength = 0;

		// 迭代计算路径总长度
		for(int i = 0; i < allWayPoints.Length - 1; i++)
		{
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
		}

		return pathLength;
	}

	public float speedDampTime = 0.1f;              // 速度缓冲时间
	public float angularSpeedDampTime = 0.7f;       // 角速度缓冲时间
	public float angleResponseTime = 0.6f;          // 把角度转为角速度的反应时间



	// 构造器
	public void AnimatorSetup(Animator animator, HashIDs hashIDs)
	{
		anim = animator;
		hash = hashIDs;
	}


	public void Setup(float speed, float angle)
	{
		// 角速度等于 = 角度 / 角度转换时间
		float angularSpeed = angle / angleResponseTime;

		// 设置动画参数 并设置缓冲时间
		anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
		anim.SetFloat(hash.angularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);
	} 

}
