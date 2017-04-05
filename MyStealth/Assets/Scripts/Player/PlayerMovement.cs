using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public AudioClip shoutingClip;      // 角色喊叫声的音频剪辑
	public float turnSmoothing = 15f;   // 玩家平滑转向的速度
	public float speedDampTime = 0.1f;  // 速度缓冲时间


	private Animator anim;              // Animator组件引用
	private HashIDs hash;               // HashIDs脚本引用


	void Awake ()
	{
		// 获取引用
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();

		// 设置Shouting Layer的权重为1 
		anim.SetLayerWeight(1, 1f); //函数参数解释： anim.SetLayerWeight(动画层序号, 权重值);
	}


	void FixedUpdate ()
	{
		// 缓存用户输入
		float h = Input.GetAxis("Horizontal");       //横向移动键
		float v = Input.GetAxis("Vertical");           //纵向移动键
		bool sneak = Input.GetButton("Sneak"); //潜行键

		MovementManagement(h, v, sneak); 
	}


	void Update ()
	{
		// 缓存用户输入
		bool shout = Input.GetButtonDown("Attract");

		//设置 animator 中的 shout 参数
		anim.SetBool(hash.shoutingBool, shout); //函数参数解释 anim.SetBool(shout当前值,  改变后的值)

		AudioManagement(shout);
	}


	void MovementManagement (float horizontal, float vertical, bool sneaking)
	{
		// 设置 animator 中的 sneaking 参数
		anim.SetBool(hash.sneakingBool, sneaking); //函数参数解释 anim.SetBool(sneaking当前值,  改变后的值)

		// 如果横向或纵向按键被按下 也就是说角色处于移动中
		if(horizontal != 0f || vertical != 0f)
		{
			// 设置玩家的旋转 并把速度设为5.5
			Rotating(horizontal, vertical);
			anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);  //函数参数解释 anim.SetFloat (当前速度,  最大速度, 加速缓冲时间, 增量时间)
		}
		else
			// 否则 设置角色速度为0
			anim.SetFloat(hash.speedFloat, 0);
	}


	void Rotating (float horizontal, float vertical)
	{
		// 创建角色目标方向的向量
		Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);

		// 创建目标旋转值 并假设Y轴正方向为"上"方向
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up); //函数参数解释: LookRotation(目标方向为"前方向", 定义声明"上方向")

		Rigidbody myBody = this.GetComponent<Rigidbody> ();

		// 创建新旋转值 并根据转向速度平滑转至目标旋转值
		//函数参数解释: Lerp(角色刚体当前旋转值, 目标旋转值, 根据旋转速度平滑转向)
		Quaternion newRotation = Quaternion.Lerp(myBody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

		// 更新刚体旋转值为 新旋转值
		myBody.MoveRotation(newRotation);
	}


	void AudioManagement (bool shout)
	{
		AudioSource myAudioSource = this.GetComponent<AudioSource> ();
		// 如果角色处于移动状态
		if(anim.GetCurrentAnimatorStateInfo(0).fullPathHash == hash.locomotionState)
		{
			// 并且脚步声没有播放
			if(!myAudioSource.isPlaying)
				// 那么播放脚步声
				myAudioSource.Play();
		}
		else
			// 否则停止播放脚步声
			myAudioSource.Stop();

		// 如果玩家按下喊叫键
		if(shout)
			// 播放指定的喊叫声
			AudioSource.PlayClipAtPoint(shoutingClip, transform.position); //函数参数解释: PlayClipAtPoint(要播放的音频剪辑, 播放位置);
	}
}
