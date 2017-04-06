using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftDoorsTracking : MonoBehaviour {

	public float doorSpeed = 7f;            // 内层门的开关速度


	private Transform leftOuterDoor;        // 引用左半边外层门的"Transform"
	private Transform rightOuterDoor;       // 引用右半边外层门的"Transform"
	private Transform leftInnerDoor;        // 引用左半边内层门的"Transform"
	private Transform rightInnerDoor;       // 引用右半边内层门的"Transform"
	private float leftClosedPosX;           // 左半边边门关闭时的X坐标
	private float rightClosedPosX;          // 右半边边门关闭时的X坐标


	void Awake ()
	{
		// 获取引用
		leftOuterDoor = GameObject.Find("door_exitOuter_left_001").transform;
		rightOuterDoor = GameObject.Find("door_exitOuter_right_001").transform;
		leftInnerDoor = GameObject.Find("door_exitInner_left_001").transform;
		rightInnerDoor = GameObject.Find("door_exitInner_right_001").transform;

		// 设置当前门的坐标为关闭时的X坐标(因为门初始状态是关闭的)
		leftClosedPosX = leftInnerDoor.position.x;
		rightClosedPosX = rightInnerDoor.position.x;
	}


	void MoveDoors (float newLeftXTarget, float newRightXTarget)
	{
		// newX为左半边内层门移动时的X坐标
		float newX = Mathf.Lerp(leftInnerDoor.position.x, newLeftXTarget, doorSpeed * Time.deltaTime);

		// 让左半边内层门移动到相应的X坐标
		leftInnerDoor.position = new Vector3(newX, leftInnerDoor.position.y, leftInnerDoor.position.z);

		// 再让newX为右半边内层门移动的X坐标
		newX = Mathf.Lerp(rightInnerDoor.position.x, newRightXTarget, doorSpeed * Time.deltaTime);

		// 让右半边内层门移动到相应的X坐标
		rightInnerDoor.position = new Vector3(newX, rightInnerDoor.position.y, rightInnerDoor.position.z);
	}


	public void DoorFollowing ()
	{
		// 让内层门随着外层门移动
		MoveDoors(leftOuterDoor.position.x, rightOuterDoor.position.x);
	}


	public void CloseDoors ()
	{
		// 让内层门移动至关闭位置
		MoveDoors(leftClosedPosX, rightClosedPosX);
	}

}
