using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPlayerDetection : MonoBehaviour {

	private GameObject player;                          // 玩家对象引用
	private LastPlayerSighting lastPlayerSighting;      // lastPlayerSighting变量引用


	void Awake ()
	{
		// 获取引用对象
		player = GameObject.FindGameObjectWithTag(Tags.player);
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	}


	void OnTriggerStay(Collider other)
	{
		Renderer rendererLaser = this.GetComponent<Renderer> ();
		// 如果激光门开启
		if(rendererLaser.enabled)
			// 并且探测到玩家时
		if(other.gameObject == player)
			// 更新最后发现玩家的位置 为 当前位置
			lastPlayerSighting.position = other.transform.position;
	}
}
