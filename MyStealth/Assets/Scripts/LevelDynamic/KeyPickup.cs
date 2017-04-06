using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour {

	public AudioClip keyGrab;                       // 播放拾取钥匙卡音效的音频剪辑


	private GameObject player;                      // 引用玩家对象
	private PlayerInventory playerInventory;        // 引用"PlayerInventory"脚本


	void Awake ()
	{
		// 获取引用对象
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerInventory = player.GetComponent<PlayerInventory>();
	}


	void OnTriggerEnter (Collider other)
	{
		// 如果是玩家碰撞触发器
		if(other.gameObject == player)
		{
			// 在钥匙卡位置播放拾取音效
			AudioSource.PlayClipAtPoint(keyGrab, transform.position);

			// 判定玩家已拿到钥匙卡
			playerInventory.hasKey = true;

			// 摧毁钥匙卡
			Destroy(gameObject);
		}
	}
}
