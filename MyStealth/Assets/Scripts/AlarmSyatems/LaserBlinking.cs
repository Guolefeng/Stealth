using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBlinking : MonoBehaviour {

	public float onTime;            // 激光门开启时间
	public float offTime;           // 激光门关闭时间


	private float timer;            // 激光门开关间隔时间


	void Update ()
	{
		Renderer rendererLaser = this.GetComponent<Renderer> ();
		// 计时器开始计时
		timer += Time.deltaTime;

		// 如果激光门渲染组件启用 且计时器时间大于或等于激光门开启时间
		if(rendererLaser.enabled && timer >= onTime)
			// 切换激光门状态（关闭）
			SwitchBeam();

		// 如果激光门渲染组件禁用 且计时器时间大于或等于激光门关闭时间
		if(!rendererLaser.enabled && timer >= offTime)
			// 切换激光门状态（开启）
			SwitchBeam();
	}


	void SwitchBeam ()
	{
		Renderer rendererLaser = this.GetComponent<Renderer> ();
		Light lightLaser = this.GetComponent<Light> ();

		// 重置计时器为0
		timer = 0f;

		// 切换激光门的渲染组件和灯光组件 为与当前相反的状态（已启用则禁用 已禁用则启用）
		rendererLaser.enabled = !rendererLaser.enabled;
		lightLaser.enabled = !lightLaser.enabled;
	}
}
