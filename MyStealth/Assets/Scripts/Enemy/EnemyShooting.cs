using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour {

	public float maximumDamage = 120f;                  // 射击最大伤害
	public float minimumDamage = 45f;                   // 射击最小伤害值
	public AudioClip shotClip;                          // 播放射击音效的音频剪辑
	public float flashIntensity = 3f;                   // 射击时枪口灯光闪烁的强度
	public float fadeSpeed = 10f;                       // 枪口灯光闪烁的速率


	private Animator anim;                              // 引用animator组件
	private HashIDs hash;                               // 引用HashIDs脚本
	private LineRenderer laserShotLine;                 // 引用fx_laserShot中的line renderer组件
	private Light laserShotLight;                       // 引用fx_laserShot中的light组件
	private SphereCollider col;                         // 引用球形触发
	private Transform player;                           // 引用玩家的transform
	private PlayerHealth playerHealth;                  // 引用playerHealth脚本
	private bool shooting;                              // 判断敌人是否射击
	private float scaledDamage;                         // 射击伤害范围值


	void Awake ()
	{
		// 获取引用对象和组件
		anim = GetComponent<Animator>();
		laserShotLine = GetComponentInChildren<LineRenderer>();
		laserShotLight = laserShotLine.gameObject.GetComponent<Light>();
		col = GetComponent<SphereCollider>();
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		playerHealth = player.gameObject.GetComponent<PlayerHealth>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		// 游戏初始时 关闭line renderer和light组件
		laserShotLine.enabled = false;
		laserShotLight.intensity = 0f;

		// 射击伤害范围 = 射击最大伤害 - 射击最小伤害
		scaledDamage = maximumDamage - minimumDamage;
	}


	void Update ()
	{
		// 缓存shot curve当前的参数值
		float shot = anim.GetFloat(hash.shotFloat);

		// 如果参数值大于0.5 且敌人之前没有射击过
		if(shot > 0.5f && !shooting)
			// 那么就...射！！！
			Shoot();

		// 如果参数值跌回至0.5一下
		if(shot < 0.5f)
		{
			// 那么就不让敌人再射了 且关闭line renderer组件
			shooting = false;
			laserShotLine.enabled = false;
		}

		// 让枪口灯光由当前强度值渐变至0
		laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0f, fadeSpeed * Time.deltaTime);
	}


	void OnAnimatorIK (int layerIndex)
	{
		// 缓存AimWeight curve当前的参数值
		float aimWeight = anim.GetFloat(hash.aimWeightFloat);

		// 让敌人右手指向玩家中心位置
		anim.SetIKPosition(AvatarIKGoal.RightHand, player.position + Vector3.up*1.5f);

		// 设置IK的权重等于缓存的aimWeight参数值
		anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
	}


	void Shoot ()
	{
		// 敌人射了
		shooting = true;

		// 射击距离的分数比例 如果敌人与玩家的距离为0 那么结果为1 如果敌人与玩家的距离为最大距离 那么结果为0
		float fractionalDistance = (col.radius - Vector3.Distance(transform.position, player.position)) / col.radius;

		// 射击伤害值 = 射击伤害范围 * 射击距离分数比例 + 射击最小伤害
		float damage = scaledDamage * fractionalDistance + minimumDamage;

		// 玩家被射后扣血
		playerHealth.TakeDamage(damage);

		// 显示射击特效
		ShotEffects();
	}


	void ShotEffects ()
	{
		// 设置激光的起始点为fx_laserShot的位置 也就是枪口位置
		laserShotLine.SetPosition(0, laserShotLine.transform.position);

		// 设置激光的终点为玩家的中心位置
		laserShotLine.SetPosition(1, player.position + Vector3.up * 1.5f);

		// 启用line renderer组件
		laserShotLine.enabled = true;

		// 枪口闪光特效
		laserShotLight.intensity = flashIntensity;

		// 在枪口位置播放射击音效
		AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);
	}
}
