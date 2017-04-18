using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimation : MonoBehaviour {

	public float deadZone = 5f;             // 当角度小于这个变量时 转向将不再使用动画系统控制


	private Transform player;               // 引用玩家Transform
	private EnemySight enemySight;          // 引用EnemySight脚本
	private NavMeshAgent nav;               // 引用nav mesh agent组件
	private Animator anim;                  // 引用Animator组件
	private HashIDs hash;                   // 引用HashIDs脚本
	private AnimatorSetup animSetup;        // AnimatorSetup辅助类的实例


	void Awake ()
	{
		// 获取引用对象和组件
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		// 让选项使用动画系统控制
		nav.updateRotation = false;

		// 创建AnimatorSetup类的实例并调用它的构造器
		animSetup = new AnimatorSetup(anim, hash);

		// 把Shooting和Gun动画层的权重都设为1
		anim.SetLayerWeight(1, 1f);
		anim.SetLayerWeight(2, 1f);

		// 把deadZone角度转化为弧度
		deadZone *= Mathf.Deg2Rad;
	}


	void Update () 
	{
		// 计算需要传递给Animator组件的参数
		NavAnimSetup();
	}


	void OnAnimatorMove ()
	{
		// 设置NavMeshAgent的速度 等于上一帧的移动速度
		nav.velocity = anim.deltaPosition / Time.deltaTime;

		// 敌人转向由动画控制
		transform.rotation = anim.rootRotation;
	}


	void NavAnimSetup ()
	{
		// 创建需要传递给辅助类的参数
		float speed;
		float angle;

		// 如果玩家被发现了
		if(enemySight.playerInSight)
		{
			// 敌人停止移动
			speed = 0f;

			//转向角度等于 敌人正前方向量 与 敌人到玩家位置的向量 的夹角 让敌人面向玩家位置
			angle = FindAngle(transform.forward, player.position - transform.position, transform.up);
		}
		else
		{
			// 否则 速度等于期望速度向量在敌人正前方向量上的投影向量
			speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;

			// 转向角度等于 敌人正前方向量 与 期望速度向量 的夹角
			angle = FindAngle(transform.forward, nav.desiredVelocity, transform.up);

			// 当夹角小于deadZone时
			if(Mathf.Abs(angle) < deadZone)
			{
				// 让敌人面向期望速度方向 把角度设为0 停止转向
				transform.LookAt(transform.position + nav.desiredVelocity);
				angle = 0f;
			}
		}

		// 传递参数并调用辅助类中的Setup函数
		animSetup.Setup(speed, angle);
	}


	float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		// 如果目标向量等于0
		if(toVector == Vector3.zero)
			// 夹角也设为0
			return 0f;

		// 创建变量 变量等于当前向量与目标向量的夹角
		float angle = Vector3.Angle(fromVector, toVector);

		// 计算当前向量与目标向量的向量积 得到法向量(如果目标向量在当前向量的右边 那么法向量的方向向上).
		Vector3 normal = Vector3.Cross(fromVector, toVector);

		// 计算法向量与敌人正上方向量的点积 如果是同方向 值为正 反之则为负
		angle *= Mathf.Sign(Vector3.Dot(normal, upVector));

		// 把角度转换为弧度
		angle *= Mathf.Deg2Rad;

		return angle;
	}
}
