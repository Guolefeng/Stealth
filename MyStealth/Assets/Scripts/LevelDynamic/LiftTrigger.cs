using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftTrigger : MonoBehaviour {

	public float timeToDoorsClose = 2f;             // 玩家进入电梯2秒后 关闭内层门
	public float timeToLiftStart = 3f;              // 玩家进入电梯3秒后 电梯启动
	public float timeToEndLevel = 6f;               // 玩家进入电梯6秒后 场景结束
	public float liftSpeed = 3f;                    // 电梯移动速度


	private GameObject player;                      // 引用玩家对象
	private Animator playerAnim;                    // 引用角色的"Animator"组件
	private HashIDs hash;                           // 引用"HashIDs"脚本
	private CameraMovement camMovement;             // 引用"CameraMovement"脚本
	private SceneFadeInOut sceneFadeInOut;          // 引用"SceneFadeInOut"脚本
	private LiftDoorsTracking liftDoorsTracking;    // 引用"LiftDoorsTracking"脚本
	private bool playerInLift;                      // 判断玩家是否在电梯内
	private float timer;                            // 计时器 用于决定何时关闭内层门、何时启动电梯、何时结束场景

	void Awake ()
	{
		// 获取引用对象
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerAnim = player.GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		camMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
		sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
		liftDoorsTracking = GetComponent<LiftDoorsTracking>();
	}


	void OnTriggerEnter (Collider other)
	{
		// 如果玩家进入触发器范围
		if(other.gameObject == player)
			// 判定玩家在电梯内
			playerInLift = true;
	}


	void OnTriggerExit (Collider other)
	{
		// 如果玩家离开触发器范围
		if(other.gameObject == player)
		{
			// 判定玩家不在电梯内 并让计时器归零
			playerInLift = false;
			timer = 0;
		}
	}


	void Update ()
	{
		// 如果玩家在电梯内
		if(playerInLift)
			// 调用函数 启动电梯
			LiftActivation();

		// 如果计时器小于 应关闭内层电梯门的时间
		if(timer < timeToDoorsClose)
			// 内层门应随外层门移动
			liftDoorsTracking.DoorFollowing();
		else
			// 否则关闭内层门
			liftDoorsTracking.CloseDoors();
	}


	void LiftActivation ()
	{
		AudioSource myAudio = this.GetComponent<AudioSource> ();
		// 玩家进入电梯后 计时器开始计时
		timer += Time.deltaTime;

		// 如果计时器 大于等于 电梯应启动的时间
		if(timer >= timeToLiftStart)
		{
			// 角色速度设为0 禁止角色移动 禁用摄像机移动脚本 并让角色成为电梯的子对象
			playerAnim.SetFloat(hash.speedFloat,0f);
			camMovement.enabled = false;
			player.transform.parent = transform;

			// 让电梯匀速向上移动
			transform.Translate(Vector3.up * liftSpeed * Time.deltaTime);

			// 如果电梯移动音效没有播放
			if(!myAudio.isPlaying)
				// 那么播放音效
				myAudio.Play();

			// 如果计时器 大于等于 该结束场景的时间
			if(timer >= timeToEndLevel)
				// 调用函数结束场景 
				sceneFadeInOut.EndScene();
		}
	}
}
