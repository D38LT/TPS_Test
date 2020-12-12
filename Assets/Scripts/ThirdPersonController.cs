using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{

    [SerializeField]
    float Gravity = 9.81f;
    [SerializeField]
    float runSpeed = 5.0f;
    [SerializeField]


    public Transform myTransform;
    [SerializeField]
    Transform model;

    [SerializeField]
    CharacterController cc;

    [SerializeField]
    Animator ani;

    [SerializeField]
    Vector3 move;
    [SerializeField]
    Transform cameraParentTransfomr;
    [SerializeField]
    Transform cameratTrasform;
    float xSpeed = 220.0f;
    float ySpeed = 100.0f;
    float x = 0.0f;
    float y = 0.0f;
    float yMinLimit = -20f;
    float yMaxLimit = 80f;

    float xMinLimit = -90f;
    float xMaxLimit = 90f;
    float check = -5;
    float dist = -2.0f;
    float jumpspeed = 8.0f;

    
    
    #region coverVariable
    public bool iscover = false;
    Vector3 angle;
    Vector3 hitpos;

    Vector3 hitpoint;
    Vector3 inputMoveX;
    RaycastHit hit;
    Vector3 direc;

    GameObject touchobjec;
    GameObject targetcoverobjec;
    Vector3 target;
    public bool covercheck;
    int covercount;
    #endregion
    public bool rightlock;
    public bool leftlock;

    public Vector3 crouchSet;
    public Vector3 standSet;
    public bool stand;

    #region raydriection
    Vector3 direcf;
    Vector3 direcb;
    Vector3 direcl;
    Vector3 direcr;

    Vector3 direcfl;
    Vector3 direcfr;
    Vector3 direcbl;
    Vector3 direcbr;
    #endregion
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
    void Awake()
    {
        crouchSet = new Vector3(0, 1.0f, 0);
        standSet = new Vector3(0, 1.7f, 0);
        stand = true;
        myTransform = transform;
        cc = GetComponent<CharacterController>();
        model = transform.GetChild(0);
        ani = model.GetComponent<Animator>();
        cameratTrasform = Camera.main.transform;
        cameraParentTransfomr = cameratTrasform.parent;
        Vector3 angles = cameraParentTransfomr.eulerAngles;
        x = angles.y;
        y = angles.x;
        covercount = 0;
    }
    void Start()
    {
        Camera.main.transform.localPosition = new Vector3(0, 0, -2.55f);
        CamMove();
    }
    // Update is called once per frame
    void Update()
    {
        Balance();
        CameraDistanceCtrl();
        Vector3 CamPosition = model.position + new Vector3(0, 1.25f, 0);
        cameraParentTransfomr.localPosition = CamPosition;
        jump();
        covercasthit();
        Setdirection();

        if (cc.isGrounded && covercheck == false)
        {
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (Physics.Raycast(transform.position + crouchSet, direcf, out hit, 2f))
                {
                    direc = direcf;
                    //chekcstand();
                    //iscover = true;
                }
                else if (Physics.Raycast(transform.position + crouchSet, direcfl, out hit, 2f))
                {
                    direc = direcfl;
                    //chekcstand();
                    //iscover = true;
                }
                else if (Physics.Raycast(transform.position + crouchSet, direcfr, out hit, 2f))
                {
                    direc = direcfr;
                    //chekcstand();
                    //iscover = true;
                }
                else if (Physics.Raycast(transform.position + crouchSet, direcb, out hit, 2f))
                {
                    direc = direcb;
                    //chekcstand();
                    //iscover = true;
                }
                else if (Physics.Raycast(transform.position + crouchSet, direcbl, out hit, 2f))
                {
                    direc = direcbl;
                    //chekcstand();
                    //iscover = true;
                }
                else if (Physics.Raycast(transform.position + crouchSet, direcbr, out hit, 2f))
                {
                    direc = direcbr;
                    //chekcstand();
                    //iscover = true;
                }
                else if (Physics.Raycast(transform.position + crouchSet, direcr, out hit, 2f))
                {
                    direc = direcr;
                    //chekcstand();
                    //iscover = true;
                }
                else if (Physics.Raycast(transform.position + crouchSet, direcl, out hit, 2f))
                {
                    direc = direcl;
                    //chekcstand();
                    //iscover = true;
                }
            }
            ani.SetBool("isGrounded", true);
            crouch();
            MoveCalc(1.5f);
        }
        else if (cc.isGrounded && covercheck == true)
        {
            ani.SetBool("isGrounded", true);
            crouch();
            MoveCalc(1.5f);
        }
        else if (!cc.isGrounded)
        {
            ani.SetBool("isGrounded", false);
            move.y -= Gravity * Time.deltaTime;

            MoveCalc(0.01f);
            Debug.Log("in air");
        }
    }
    void chekcstand()
    {
        if (Physics.Raycast(transform.position + standSet, direc, out hit, 2f))
        {
            Debug.Log("stand true");
            stand = true;
        }
    }
    void crouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(stand == true)
            {
                Debug.Log("stand true");
                stand = false;
                ani.SetBool("isCrouched",true);
            }
            else if(stand == false)
            {
                Debug.Log("stand false");
                stand = true;
                ani.SetBool("isCrouched", false);
            }
        }
    }

    void LateUpdate()
    {
        CamMove();
    }

    void Balance()
    {
        if (myTransform.eulerAngles.x != 0 || myTransform.eulerAngles.z != 0)
        {
            myTransform.eulerAngles = new Vector3(0, myTransform.eulerAngles.y, 0);
        }
    }

    void CameraDistanceCtrl()
    {
        Camera.main.transform.localPosition += new Vector3(0, 0, Input.GetAxisRaw("Mouse ScrollWheel") * 1.0f);

        if (-1 < Camera.main.transform.localPosition.z)
        {
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, -1);
        }
        else if (Camera.main.transform.localPosition.z < -3)
        {
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, Camera.main.transform.localPosition.y, -3);
        }
        dist = Camera.main.transform.localPosition.z;
    }

    void MoveCalc(float ratio)
    {

        if (cc.isGrounded)
        {
            if (ani.GetCurrentAnimatorStateInfo(0).IsName("JumpEnd01"))
            {
                move = Vector3.zero;
                return;
            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(iscover == false)
            {
                iscover = true;
                covercast();
            }
            else if(iscover==true)
            {
                Debug.Log("test");
                covercount = 0;
                iscover = false;
                covercheck = false;
                ani.SetBool("isCover", false);
            }
            //else if(iscover == true)
        }

        if (iscover == true && covercheck == false)
        {
            /* if (Input.GetKeyDown(KeyCode.C))
            {
                covercast();
                Debug.Log("test1");
            }*/
            float tempMoveY = move.y;
            move.y = 0;
            runSpeed = Mathf.Lerp(runSpeed, 4.0f, Time.deltaTime * 5.0f);
            move = Vector3.MoveTowards(move, target.normalized * runSpeed, ratio * runSpeed);
            Quaternion characterRoation = Quaternion.LookRotation(move);
            characterRoation.x = characterRoation.z = 0;
            //model.rotation = Quaternion.Slerp(model.rotation, characterRoation, 10.0f * Time.deltaTime);
            float speed = move.sqrMagnitude;
            ani.SetFloat("Speed", 16f);
            move.y = tempMoveY;
            cc.Move(move * Time.deltaTime);
        }
        if (iscover == true && covercheck == true)
        {

            /*if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("test");
                covercount = 0;
                iscover = false;
                covercheck = false;
                ani.SetBool("isCover", false);

            }*/

            float tempMoveY = move.y;
            move.y = 0;
            Vector3 inputMoveXx = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            float inputMoveXMgnitude = inputMoveXx.sqrMagnitude;
            inputMoveX = myTransform.TransformDirection(-inputMoveXx);
            runSpeed = Mathf.Lerp(runSpeed, 2.0f, Time.deltaTime * 5.0f);

            inputMoveX = inputMoveX.normalized * runSpeed;
            if (Input.GetButton("Horizontal"))
            {
                move = Vector3.MoveTowards(move, inputMoveX, ratio * runSpeed);
            }
            else
            {
                move = Vector3.MoveTowards(move, Vector3.zero, (1 - inputMoveXMgnitude) * runSpeed * ratio);
            }
            if (rightlock == true)
            {
                if (inputMoveXx.x > 0)
                {
                    move = Vector3.zero;
                    inputMoveXx.x = 0;
                }
            }
            if (leftlock == true)
            {
                if (inputMoveXx.x < 0)
                {
                    move = Vector3.zero;
                    inputMoveXx.x = 0;
                }
            }
            float speed = inputMoveXx.x;
            ani.SetFloat("Speed", speed);
            move.y = tempMoveY;
            cc.Move(move * Time.deltaTime);
        }
        else if (iscover == false)
        {
            float tempMoveY = move.y;
            move.y = 0;
            Vector3 inputMoveXZ = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            float inputMoveXZMgnitude = inputMoveXZ.sqrMagnitude;
            inputMoveXZ = myTransform.TransformDirection(inputMoveXZ);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                runSpeed = Mathf.Lerp(runSpeed, 4.0f, Time.deltaTime * 5.0f);
            }
            else
            {
                runSpeed = Mathf.Lerp(runSpeed, 2.0f, Time.deltaTime * 5.0f);
            }
            if (inputMoveXZMgnitude <= 1)
            {
                inputMoveXZ *= runSpeed;
            }
            else
            {
                inputMoveXZ = inputMoveXZ.normalized * runSpeed;
            }

            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                Quaternion cameraRotation = cameraParentTransfomr.rotation;
                cameraRotation.x = cameraRotation.z = 0;
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, cameraRotation, 10.0f * Time.deltaTime);

                if (move != Vector3.zero)
                {
                    Quaternion characterRoation = Quaternion.LookRotation(move);
                    characterRoation.x = characterRoation.z = 0;
                    model.rotation = Quaternion.Slerp(model.rotation, characterRoation, 10.0f * Time.deltaTime);
                }
                move = Vector3.MoveTowards(move, inputMoveXZ, ratio * runSpeed);
            }
            else
            {
                move = Vector3.MoveTowards(move, Vector3.zero, (1 - inputMoveXZMgnitude) * runSpeed * ratio);
            }
            float speed = move.sqrMagnitude;

            ani.SetFloat("Speed", speed);
            move.y = tempMoveY;
            cc.Move(move * Time.deltaTime);
        }

    }

    void GradientCheck()
    {
        ani.SetBool("isGrounded", true);
        if (Physics.Raycast(myTransform.position, Vector3.down, 0.2f))
        {
            move.y = check;
        }
        else
        {
            move.y = -1;
        }
    }

    void CamMove()
    {
        if (model)
        {

            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);
            if (covercheck == true)
            {
                //x = myTransform.localEulerAngles.y;
                //x = Mathf.Clamp(x,myTransform.localEulerAngles.y + xMinLimit, myTransform.localEulerAngles.y + xMaxLimit);

                if (x >= myTransform.localEulerAngles.y + xMaxLimit - 180f)
                    x = myTransform.localEulerAngles.y + xMaxLimit - 180f;

                if (x <= myTransform.localEulerAngles.y + xMinLimit - 180f)
                    x = myTransform.localEulerAngles.y + xMinLimit - 180f;
            }
            Quaternion Camrotation = Quaternion.Euler(y, x, 0);
            cameraParentTransfomr.rotation = Camrotation;
        }
    }

    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            move.y = jumpspeed;

            ani.SetBool("isGrounded", false);
            ani.CrossFadeInFixedTime("JumpStart01", 0.1f);
        }
    }

    public void cover()
    {
        Debug.Log("cover function");
        ani.SetBool("isCover", true);
    }
    void Setdirection()
    {
        direcf = transform.TransformDirection(Vector3.forward) * 2f;
        direcb = transform.TransformDirection(Vector3.back) * 2f;
        direcl = transform.TransformDirection(Vector3.left) * 2f;
        direcr = transform.TransformDirection(Vector3.right) * 2f;

        direcfl = transform.TransformDirection(Vector3.forward + Vector3.left) * 2f;
        direcfr = transform.TransformDirection(Vector3.forward + Vector3.right) * 2f;
        direcbl = transform.TransformDirection(Vector3.back + Vector3.left) * 2f;
        direcbr = transform.TransformDirection(Vector3.back + Vector3.right) * 2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + crouchSet, direcf);
        Gizmos.DrawRay(transform.position + crouchSet, direcfl);
        Gizmos.DrawRay(transform.position + crouchSet, direcfr);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + crouchSet, direcr);
        Gizmos.DrawRay(transform.position + crouchSet, direcl);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + crouchSet, direcb);
        Gizmos.DrawRay(transform.position + crouchSet, direcbl);
        Gizmos.DrawRay(transform.position + crouchSet, direcbr);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + standSet, direcf);
        Gizmos.DrawRay(transform.position + standSet, direcfl);
        Gizmos.DrawRay(transform.position + standSet, direcfr);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position + standSet, direcr);
        Gizmos.DrawRay(transform.position + standSet, direcl);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + standSet, direcb);
        Gizmos.DrawRay(transform.position + standSet, direcbl);
        Gizmos.DrawRay(transform.position + standSet, direcbr);


    }
    void OnControllerColliderHit(ControllerColliderHit hitcc)
    {
        touchobjec = hitcc.gameObject;
        if (hitcc.gameObject == targetcoverobjec && covercount == 0 && iscover == true)
        {
            coverturn();
            covercheck = true;
            covercount = 1;
            iscover = true;
            cover();
        }
    }

    public void covercast()
    {
        //if (Physics.Raycast(transform.position + crouchSet, direc, out hit, 2f))
        //{
            targetcoverobjec = hit.transform.gameObject;
            hitpos = hit.normal + crouchSet;
            angle = new Vector3(0, getdgree(Vector3.zero, hitpos), 0);
            model.transform.localEulerAngles = new Vector3(0, -20.0f, 0);
            hitpoint = new Vector3(hit.point.x, myTransform.position.y, hit.point.z - 0.2f);
            target = hitpoint - myTransform.position;
        //}
    }
    public float getdgree(Vector3 from, Vector3 to)
    {
        return Mathf.Atan2(to.x - from.x, to.z - from.z) * 180 / Mathf.PI;
    }
    public void coverturn()
    {
        myTransform.localEulerAngles = new Vector3(angle.x, angle.y, angle.z);
    }
    public void covercasthit()
    {
        rightlock = true;
        leftlock = true;

        //stand = false;

        if (Physics.Raycast(transform.position + standSet, direcf, out hit, 2f))
        {
            direc = direcf;
            //stand = true;
        }

        if (Physics.Raycast(transform.position + crouchSet, direcf, out hit, 2f))
        {
            direc = direcf;
        }
        if (Physics.Raycast(transform.position + crouchSet, direcfl, out hit, 2f))
        {
            direc = direcfl;
        }
        if (Physics.Raycast(transform.position + crouchSet, direcfr, out hit, 2f))
        {
            direc = direcfr;
        }
        if (Physics.Raycast(transform.position + crouchSet, direcb, out hit, 2f))
        {
            direc = direcb;
        }
        if (Physics.Raycast(transform.position + crouchSet, direcbl, out hit, 2f))
        {
            direc = direcbl;

            rightlock = false;
        }
        if (Physics.Raycast(transform.position + crouchSet, direcbr, out hit, 2f))
        {
            direc = direcbr;
            leftlock = false;
        }
        if (Physics.Raycast(transform.position + crouchSet, direcr, out hit, 2f))
        {
            direc = direcr;
        }
        if (Physics.Raycast(transform.position + crouchSet, direcl, out hit, 2f))
        {
            direc = direcl;
        }
    }


}


