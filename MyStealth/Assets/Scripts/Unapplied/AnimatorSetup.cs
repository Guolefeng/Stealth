using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSetup {

	public float speedDampTime = 0.1f;              // 速度缓冲时间
	public float angularSpeedDampTime = 0.7f;       // 角速度缓冲时间
	public float angleResponseTime = 0.6f;          // 把角度转为角速度的反应时间


	private Animator anim;                          // 引用Animator组件
	private HashIDs hash;                           // 应用HashIDs脚本


	// 构造器
	public AnimatorSetup(Animator animator, HashIDs hashIDs)
	{
		anim = animator;
		hash = hashIDs;
	}


	public void Setup(float speed, float angle)
	{
		// 角速度等于 = 角度 / 角度转换时间
		float angularSpeed = angle / angleResponseTime;

		// 设置动画参数 并设置缓冲时间
		anim.SetFloat(hash.speedFloat, speed, speedDampTime, Time.deltaTime);
		anim.SetFloat(hash.angularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);
	}
}
