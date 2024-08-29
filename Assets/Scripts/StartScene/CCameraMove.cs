using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 시작화면에서 초반 카메라를 움직이는 스크립트 입니다.
 */

public class CCameraMove : MonoBehaviour
{
    public Transform[] pos;
    public GameObject mainCamera;

    private Transform originCameraTrans;
    private int cameraIndex;


    private void Start()
    {
        originCameraTrans = mainCamera.transform;
        PanelManager.Instance.toMainMenu += TurnCameraOnMainMenu;

        StartCoroutine(CameraMove());
    }

    private void TurnCameraOnMainMenu()
    {
        StartCoroutine(CameraTurn());
    }

    private IEnumerator CameraMove()
    {
        int cameraIndex = 0;

        float moveTime = 1f;
        float sumTime = 0f;

        // position01까지 움직임
        while(sumTime <= moveTime)
        {
            float time = sumTime / moveTime;
            mainCamera.transform.position = Vector3.Lerp(originCameraTrans.position, pos[cameraIndex].position, time);

            sumTime += Time.deltaTime;

            yield return null;
        }

        cameraIndex++;
        sumTime = 0f;

        // position02까지 움직임
        while (sumTime <= moveTime)
        {
            float time = sumTime / moveTime;
            mainCamera.transform.position = Vector3.Lerp(originCameraTrans.position, pos[cameraIndex].position, time);

            sumTime += Time.deltaTime;

            yield return null;
        }

    }

    private IEnumerator CameraTurn()
    {
        cameraIndex = pos.Length - 1;

        float moveTime = 1f;
        float sumTime = 0f;

        // position01까지 움직임
        while (sumTime <= moveTime)
        {
            float time = sumTime / moveTime;
            mainCamera.transform.rotation = Quaternion.Lerp(originCameraTrans.rotation, pos[cameraIndex].rotation, time);

            sumTime += Time.deltaTime;

            yield return null;
        }
    }

}
