using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVPlayerDetection : MonoBehaviour {

	private GameObject player;                          // 玩家对象引用
	private LastPlayerSighting lastPlayerSighting;      // LastPlayerSighting脚本引用


	void Awake ()
	{
		// 设置引用对象
		player = GameObject.FindGameObjectWithTag(Tags.player);
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}


	void OnTriggerStay (Collider other)
	{
		// 如果检测到碰撞对象为玩家
		if(other.gameObject == player)
		{
			// 摄像机位置光线投射到玩家位置
			Vector3 relPlayerPos = player.transform.position - transform.position;
			RaycastHit hit;

			if(Physics.Raycast(transform.position, relPlayerPos, out hit))
				// 如果光线投射到玩家
			if(hit.collider.gameObject == player)
				// 更新玩家当前位置等于 最后发现玩家位置
				lastPlayerSighting.position = player.transform.position;
		}
	}
}
