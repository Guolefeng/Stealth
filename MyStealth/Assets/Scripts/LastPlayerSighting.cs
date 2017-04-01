using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPlayerSighting : MonoBehaviour {
	public Vector3 position = new Vector3(1000f, 1000f, 1000f);         // 敌人或摄像机最后发现玩家的位置
	public Vector3 resetPosition = new Vector3(1000f, 1000f, 1000f);    // 重置坐标为默认位置 也就是说玩家没有被发现
	public float lightHighIntensity = 0.25f;                            // 主灯光的最大强度
	public float lightLowIntensity = 0f;                                // 主灯光的最小强度
	public float fadeSpeed = 7f;                                        // 主灯光的强度变化速度
	public float musicFadeSpeed = 1f;                                   // 背景音乐音量的变化速度

	private AlarmLight alarm;                                           // "AlarmLight"脚本引用
	private Light mainLight;                                            // 主灯光引用
	private AudioSource panicAudio;                                     // 紧张氛围的背景音乐"Audio Source"组件引用
	private AudioSource[] sirens;                                       // 警报喇叭引用（数组）

	void Awake() {
		// 找到"Tag"为"alarm"的对象，获取包含的"AlarmLight"脚本组件引用
		alarm = GameObject.FindGameObjectWithTag(Tags.alarm).GetComponent<AlarmLight>();

		// 找到"Tag"为"mainLight"的对象，获取包含的灯光组件引用
		mainLight = GameObject.FindGameObjectWithTag(Tags.mainLight).GetComponent<Light>();

		// 在"Game Controller"和其子元素中查找名字为"secondaryMusic"的对象，获取音频组件引用
		panicAudio = transform.Find("secondaryMusic").GetComponent<AudioSource>();

		// 查找所有"Tag"为"siren"的对象，并依次储存到一个数组中
		GameObject[] sirenGameObjects = GameObject.FindGameObjectsWithTag(Tags.siren);

		// 让数组"sirens"与"sirenGameObejcets"的长度相同
		sirens = new AudioSource[sirenGameObjects.Length];

		// 获取所有"sirenGameObjects"数组中的对象的音频组件引用，并储存到"sirens"数组中
		for(int i = 0; i < sirens.Length; i++)
		{
			sirens[i] = sirenGameObjects[i].GetComponent<AudioSource>();
		}

	}

	void Update ()
	{
		// 调用两个函数控制警报开关和背景音乐切换
		SwitchAlarms();
		MusicFading();
	}

	void SwitchAlarms ()
	{
		// 通过玩家位置判断警报器是否开启，当玩家坐标为（1000, 1000, 1000）时 返回值为false 不开启，否则开启警报器
		alarm.alarmOn = (position != resetPosition);

		//主灯光的目标强度
		float newIntensity;

		// 如果警报灯开启
		if(position != resetPosition)
			// 主灯光的目标强度设为最小强度
			newIntensity = lightLowIntensity;
		else
			// 否则 主灯光的目标强度设为最大强度
			newIntensity = lightHighIntensity;

		// 让主灯光从当前强度渐变至目标强度
		mainLight.intensity = Mathf.Lerp(mainLight.intensity, newIntensity, fadeSpeed * Time.deltaTime);

		// 对于所有警报喇叭
		for(int i = 0; i < sirens.Length; i++)
		{
			// 如果警报开启且警笛声没有播放，则播放警笛声
			if(position != resetPosition && !sirens[i].isPlaying)
				sirens[i].Play();
			// 如果警报没有开启，则停止播放警笛声
			else if(position == resetPosition)
				sirens[i].Stop();
		}
	}

	void MusicFading ()
	{
		// 如果警报开启
		if(position != resetPosition)
		{
			// 减小背景音乐音量至0,
			GetComponent<AudioSource>().volume = Mathf.Lerp(GetComponent<AudioSource>().volume, 0f, musicFadeSpeed * Time.deltaTime);

			// 增大紧张气氛的音乐至0.8
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0.8f, musicFadeSpeed * Time.deltaTime);
		}
		else
		{
			// 否则 增大背景音乐至0.8，减小紧张气氛的音乐至0
			GetComponent<AudioSource>().volume = Mathf.Lerp(GetComponent<AudioSource>().volume, 0.8f, musicFadeSpeed * Time.deltaTime);
			panicAudio.volume = Mathf.Lerp(panicAudio.volume, 0f, musicFadeSpeed * Time.deltaTime);
		}
	}

}
