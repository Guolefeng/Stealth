using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLight : MonoBehaviour {

	public float fadeSpeed = 2f;            // 警报灯强度的变化速度
	public float highIntensity = 2f;        // 警报灯最大强度值
	public float lowIntensity = 0.5f;       // 警报灯最小强度值
	public float changeMargin = 0.2f;       // 阈值，决定何时改变强度变化的方向
	public bool alarmOn;                    // 检测警报是否开启

	private float targetIntensity;          // 警报灯强度改变的目标强度值

//	Light alarmLight = GetComponent<Light> ();

	void Awake ()
	{
		Light alarmLight = this.GetComponent<Light> ();
		// 场景刚开始时，警报灯强度设为0
		alarmLight.intensity = 0;

		// 第一次，警报灯强度值会向最大强度值改变
		targetIntensity = highIntensity;
	}


	void Update ()
	{
		Light alarmLight = this.GetComponent<Light> ();
		// 如果警报开启
		if(alarmOn)
		{
			// 警报灯强度会 以设定的变化速度从当前强度向目标强度改变 
			alarmLight.intensity = Mathf.Lerp(alarmLight.intensity, targetIntensity, fadeSpeed * Time.deltaTime);

			// 这个函数检查目标强度是否需要改变 如果需要则进行改变
			CheckTargetIntensity();
		}
		else
			// 如果警报被关闭 警报灯强度渐变为0
			alarmLight.intensity = Mathf.Lerp(alarmLight.intensity, 0f, fadeSpeed * Time.deltaTime);
	}


	void CheckTargetIntensity ()
	{
		Light alarmLight = this.GetComponent<Light> ();
		// 如果 当前强度 与 目标强度 的差值的绝对值 小于 阈值
		if(Mathf.Abs(targetIntensity - alarmLight.intensity) < changeMargin)
		{
			// 如果当前目标强度为 最大强度
			if(targetIntensity == highIntensity)
				// 那么就改设为 最小强度
				targetIntensity = lowIntensity;
			else
				// 否则 设为 最大强度
				targetIntensity = highIntensity;
		}
	}
}
