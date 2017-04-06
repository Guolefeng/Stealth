using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimation : MonoBehaviour {

	public bool requireKey;                     // 判断门是否是需要钥匙卡开启
	public AudioClip doorSwishClip;             // 播放开关门的音频
	public AudioClip accessDeniedClip;          // 播放电梯门无法开启的提示音


	private Animator anim;                      // 引用"Animator"组件
	private HashIDs hash;                       // 引用"HashIDs"脚本
	private GameObject player;                  // 引用角色对象
	private PlayerInventory playerInventory;    // 引用"PlayerInventory"脚本
	private int count;                          // 进入触发器范围的collider的数量


	void Awake ()
	{
		// 获取引用对象
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerInventory = player.GetComponent<PlayerInventory>();
	}


	void OnTriggerEnter (Collider other)
	{
		AudioSource myAudio = this.GetComponent<AudioSource> ();
		// 如果角色进入房间门的触发器范围
		if(other.gameObject == player)
		{
			// 且这个门需要钥匙卡开启
			if(requireKey)
			{
				// 且玩家持有钥匙卡
				if(playerInventory.hasKey)
					// count值+1
					count++;
				else
				{
					// 如果玩家没有钥匙卡 那么就播放无法开启的提示音
					myAudio.clip = accessDeniedClip;
					myAudio.Play();
				}
			}
			else
				// 如果门不需要钥匙卡开启 那么count值+1
				count++;
		}
		// 如果敌人的碰撞器进入触发器范围
		else if(other.gameObject.tag == Tags.enemy)
		{
			// 且进入的碰撞器为"Capsule Collider"类型时
			if(other is CapsuleCollider)
				// count值+1
				count++;
		}
	}


	void OnTriggerExit (Collider other)
	{
		// 如果玩家离开触发器范围 或敌人的"Capsule Collider"离开触发器范围
		if(other.gameObject == player || (other.gameObject.tag == Tags.enemy && other is CapsuleCollider))
			// count值-1
			count = Mathf.Max(0, count-1);
	}


	void Update ()
	{
		AudioSource myAudio = this.GetComponent<AudioSource> ();
		// 判断（count>0）的真假 并把结果传递给"Open"参数
		anim.SetBool(hash.openBool,count > 0);

		// 如果门处于开关状态转换中 并且 开关门的音效没有播放时
		if(anim.IsInTransition(0) && !myAudio.isPlaying)
		{
			// 播放开关门音效
			myAudio.clip = doorSwishClip;
			myAudio.Play();
		}
	}
}
