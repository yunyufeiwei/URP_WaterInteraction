using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Speed;
    public float Gravity;
    public float JumpHeight;
    public GameObject Cameraman;
    public ParticleSystem ripple;
    
    private CharacterController cc;
    private float CameraY;
    private float GravityForce;
    private float Zoom = -7;
    private RaycastHit isGround;
    private Vector3 PlayerPos;
    private bool inWater;
    [SerializeField]private float VelocityXZ, VelocityY;
    
    void Start()
    {
        Application.targetFrameRate = 60;
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        PlayerMovement();
        CameraControl();
        
        if(Input.GetKeyDown(KeyCode.Space)) Jumping();
        if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        VelocityXZ = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(PlayerPos.x, 0, PlayerPos.z));
        VelocityY = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, PlayerPos.y, 0));
        PlayerPos = transform.position;

        //RippleCamera.transform.position = transform.position + Vector3.up * 10;
        if (isGround.collider)
        {
            ripple.transform.position = transform.position + transform.forward;
        }
        else
        {
            ripple.transform.position = transform.position;
        }
        Shader.SetGlobalVector("_Player", transform.position);
    }
    
    void PlayerMovement()
    {
        Vector3 camRight = Cameraman.transform.right;
        Vector3 camForward = Cameraman.transform.forward;
        camRight.y = 0;
        camForward.y = 0;

        Vector3 move = camRight.normalized * Input.GetAxis("Horizontal") + camForward.normalized * Input.GetAxis("Vertical");
        cc.Move(move.normalized * Time.deltaTime * 10 * Speed * ((Input.GetKey(KeyCode.LeftShift)?2:1)));
        if(move.magnitude > 0) transform.forward = move.normalized;

        GravityForce -= Gravity * Time.deltaTime * 5;
        if(isGround.collider && GravityForce < -2) GravityForce = -2;
        else if(GravityForce < -99) GravityForce = -99;
        cc.Move(new Vector3(0, GravityForce * Time.deltaTime, 0));

        if (inWater)
        {
            ripple.gameObject.SetActive(true);
        }
        else
        {
            ripple.gameObject.SetActive(false);
        }
        
        Physics.Raycast(transform.position, Vector3.down, out isGround, 2.7f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position, Vector3.down* 2.7f);

        float height = cc.height + cc.radius;
        inWater = Physics.Raycast(transform.position + Vector3.up * height, Vector3.down, height*2, LayerMask.GetMask("Water"));
        Debug.DrawRay(transform.position + Vector3.up * height, Vector3.down* height);
    }
    void Jumping()
    {
        // || !isGround.collider
        if(GravityForce > 2) return;
        float JumpMutify = 2;
        GravityForce = Mathf.Sqrt(JumpHeight * Gravity * JumpMutify);
        cc.Move(new Vector3(0, GravityForce * Time.deltaTime, 0));
    }
    
    void CameraControl()
    {
        //注释部分是自由视角，当前修改为用固定视角来使用，即最后的一行代码
        Cameraman.transform.position = transform.position;
        // CameraY -= Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * 300;
        // CameraY = Mathf.Clamp(CameraY, -45, 45);

        Zoom += Input.mouseScrollDelta.y * Time.fixedDeltaTime * 100;
        Zoom = Mathf.Clamp(Zoom, -12, -4);

        // Cameraman.transform.Rotate(0, Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 150, 0);
        // Cameraman.transform.eulerAngles = new Vector3(CameraY, Cameraman.transform.eulerAngles.y, 0);
        //Cameraman.transform.GetChild(0).transform.localPosition = new Vector3(0, 1.15f, Zoom);
        Cameraman.transform.GetChild(0).transform.localPosition = new Vector3(0, 10f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.layer == 4)
        {
            ripple.Emit(transform.position, Vector3.zero, 5, 0.01f, Color.white);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == 4)
        {
            ripple.Emit(transform.position, Vector3.zero, 5, 0.01f, Color.white);
        }
    }
}
