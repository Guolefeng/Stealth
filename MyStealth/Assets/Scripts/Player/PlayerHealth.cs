using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	public float health = 100f;                         //角色生命值
	public float resetAfterDeathTime = 5f;              //角色死亡后多久才重置场景
	public AudioClip deathClip;                         // 角色死亡音效


	private Animator anim;                              // 引用"Animator"组件
	private PlayerMovement playerMovement;              // 引用"PlayerMovement"脚本
	private HashIDs hash;                               // 引用"HashIDs"脚本
	private SceneFadeInOut sceneFadeInOut;              // 引用"SceneFadeInOut"脚本
	private LastPlayerSighting lastPlayerSighting;      // 引用"LastPlayerSighting"脚本
	private float timer;                                // 计时器 当角色死亡开始计时
	private bool playerDead;                            // 判断角色是否死亡


	void Awake ()
	{
		// 获取引用组件或脚本
		anim = GetComponent<Animator>();
		playerMovement = GetComponent<PlayerMovement>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}


	void Update ()
	{
		// 如果角色生命值小于等于0时
		if(health <= 0f)
		{
			// 并且玩家还没死的话
			if(!playerDead)
				// 调用"PlayerDying()"函数 让角色去死...
				PlayerDying();
			else
			{
				// 如果角色已经死了 调用"PlayerDead()"和"LevelReset()"函数
				PlayerDead();
				LevelReset();
			}
		}
	}


	void PlayerDying ()
	{
		// 判定玩家已死亡
		playerDead = true;

		// 设置"Animator"中的"Dead"参数也为true 从而使角色死亡的动画播放
		anim.SetBool(hash.deadBool, playerDead);

		// 在角色死亡地点 播放死亡音效
		AudioSource.PlayClipAtPoint(deathClip, transform.position);
	}


	void PlayerDead ()
	{
		// 如果角色处于"Dying"状态 那么把"Animator"中的"Dead"参数再设为false 防止角色重复播放死亡动画
		if(anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hash.dyingState)
			anim.SetBool(hash.deadBool, false);

		// 把角色速度设为0 并禁用"playerMovement"脚本 防止角色死亡后继续移动
		anim.SetFloat(hash.speedFloat, 0f);
		playerMovement.enabled = false;

		// 重置最后探测到玩家的位置为 (1000f, 1000f, 1000f)  从而使警报器关闭
		lastPlayerSighting.position = lastPlayerSighting.resetPosition;

		AudioSource myAudio = this.GetComponent<AudioSource> ();

		// 停止播放角色脚步声
		myAudio.Stop();
	}


	void LevelReset ()
	{
		// 玩家死亡后 计时器开始计时
		timer += Time.deltaTime;

		//当计时器时间 大于等于 设定的重置场景的等待时间
		if(timer >= resetAfterDeathTime)
			// 调用"sceneFadeInOut"脚本中的"EndScene()"函数 重置场景
			sceneFadeInOut.EndScene();
	}


	public void TakeDamage (float amount)
	{
		// 角色生命值 = 角色生命值 - 应损失的生命值
		// 例如敌人射击一次损血为20 就调用这个函数 TakeDamage(20f)  角色生命值 = 默认值100 - 20
		health -= amount;
	}
}
