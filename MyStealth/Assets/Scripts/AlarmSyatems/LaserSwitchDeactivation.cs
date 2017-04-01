using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitchDeactivation : MonoBehaviour {

	public GameObject laser;                // 开关将控制的激光门对象的引用
	public Material unlockedMat;            // 开关屏幕 提示激光门解锁标志的材质

	private GameObject player;              // 玩家对象引用

	void Awake ()
	{
		// 获取引用对象
		player = GameObject.FindGameObjectWithTag(Tags.player);
	}


	void OnTriggerStay (Collider other)
	{
		// 如果探测到玩家进入开关控制范围
		if(other.gameObject == player)
			// 且按下了开关按钮
		if(Input.GetButton("Switch"))
			// 关闭对应的激光门
			LaserDeactivation();
	}


	void LaserDeactivation ()
	{
		// 禁用激光门
		laser.SetActive(false);

		// 获取开关上屏幕的渲染组件
		Renderer screen = transform.Find("prop_switchUnit_screen_001").GetComponent<Renderer>();

		// 替换屏幕材质
		screen.material = unlockedMat;

		// 播放解锁成功的音效
		GetComponent<AudioSource>().Play();
	}
}
