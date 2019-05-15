using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Author: MaxLykoS
//UpdateTime: 2017/10/28

public class CameraController : MonoBehaviour {

    public static Point CurrentClickPos
    {
        get { return currentClickPos; }
    }
    private static Point currentClickPos;

    public static float XLimit;
    public static float ZLimit;

    private Transform camRig;    //摄像机的父物体
    private Transform CircleCursor;  //光标
    private Transform cam;      //摄像机本身

    RaycastHit clickHit = new RaycastHit();
    RaycastHit pointHit = new RaycastHit();

    public EditorController editorController;
    public CombatController combatController;

    void Start ()
    {
        cam = Camera.main.transform;
        camRig = transform;
        CircleCursor = Instantiate( Resources.Load<GameObject>("Prefabs/Cursor/CircleCursor")).GetComponent<Transform>();
	}

    /// <summary>
    /// 设置摄像机的移动范围
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    static public void SetXZLimit(float x,float z)
    {
        XLimit = x;
        ZLimit = z;
    }

	void LateUpdate ()
    {
        if (!GridContainer.GameStartKey) return;

        CameraMove();
    }
    void Update()
    {
        if (!GridContainer.GameStartKey) return;

        Zoom();
        ClickEvent();
        PointEvent();
        CancelClickEvent();
    }

    #region 摄像机移动
    /// <summary>
    /// 摄像机移动
    /// </summary>
    void CameraMove()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (v == 0 && h == 0) return;

        camRig.Translate(new Vector3(h*0.2f, 0, v*0.2f),Space.Self);

        Vector3 pos = camRig.position;
        pos.x = Mathf.Clamp(pos.x, -1, XLimit-10);
        pos.z = Mathf.Clamp(pos.z, 3, ZLimit-4f);
        camRig.position = pos;
    }
    #endregion

    /// <summary>
    /// 鼠标滚轮移动缩放视角
    /// </summary>
    void Zoom()
    {
        float msw = Input.GetAxis("Mouse ScrollWheel");

        if (msw == 0) return;

        #region 视角缩放
        camRig.Translate(new Vector3(0,msw*-4.5f,0),Space.Self);
        //限制范围
        Vector3 pos = camRig.position;
        pos.y = Mathf.Clamp(pos.y, 2, 7);
        camRig.position = pos;
        #endregion

        #region 视角转动
        cam.Rotate(msw*-25, 0, 0);  
        //限制范围
        Vector3 rot = cam.transform.localEulerAngles;
        rot.x = Mathf.Clamp(rot.x, 30, 60);
        cam.localEulerAngles = rot;
        #endregion
    }

    #region 让UI阻挡鼠标点击
    public EventSystem eventSystem;
    public UnityEngine.UI.GraphicRaycaster graphicRaycaster;
    /// <summary>
    /// 让UI阻挡鼠标点击
    /// </summary>
    /// <returns></returns>
    bool CheckGuiRaycastObjects()
    {
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;

        List<RaycastResult> list = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, list);
        return list.Count > 0;
    }
    #endregion

    /// <summary>
    /// 点击事件
    /// </summary>
    void ClickEvent()
    {
        if (CheckGuiRaycastObjects()) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out clickHit, 100);
            if (clickHit.transform != null)
            {
                Debug.Log("ClickGrid");
                currentClickPos = Point.StringToPoint(clickHit.transform.name);

                if (GridContainer.isEditorMode)   //Editor Mode
                {
                    editorController.ClickChangeTerrainEventHandler(currentClickPos);
                }
                else                              //Combat Mode
                {
                    combatController.ClickChooseGridEventHandler(currentClickPos);
                }
            }
        }
    }

    #region 鼠标移动过去时
    private Vector3 pointPos;
    /// <summary>
    /// 鼠标移动过去时
    /// </summary>
    void PointEvent()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out pointHit, 100);
        if (pointHit.transform != null&&pointHit.transform.position!= pointPos)
        {
            Debug.Log("PointGrid");

            pointPos = GridContainer.Instance.TerrainDic[Point.StringToPoint(pointHit.transform.name)].transform.position;
            pointPos.y += 0.6f;
            CircleCursor.transform.position = pointPos;
            pointPos.y -= 0.6f;

            #region 显示点击格子的详细资料
            if(!GridContainer.isEditorMode)
                combatController.ShowGridDetailPanel(Point.StringToPoint(pointHit.transform.name));
            #endregion
        }
    }
    #endregion

    /// <summary>
    /// 取消操作事件
    /// </summary>
    public void CancelClickEvent()
    {
        if (!GridContainer.isEditorMode)
        {
            if (Input.GetMouseButton(1))
            {
                currentClickPos = null;

                combatController.CancelChooseGridEventHandler();
            }
        }
    }

}
