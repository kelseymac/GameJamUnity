using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour
{
    [SerializeField] private Transform target;

    public float rotSpeed = 8.0f;
    public float moveSpeed = 35.0f;
    public float jumpSpeed = 28.0f;
    public float gravity = -9.8f;
    public float terminalVelocity = -15.0f;
    public float minFall = -1.5f;


    private CharacterController _charController;
    private float _vertSpeed;
    //private Animator _animator;
    private ControllerColliderHit _contact;

    // Start is called before the first frame update
    void Start()
    {
        _charController = GetComponent<CharacterController>();
        _vertSpeed = minFall;
        //_animator = GetComponent<Animator>();
        // _animator.SetFloat("MoveSpeed", moveSpeed);
        // _animator.SetBool("Grounded", true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        float horizInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horizInput != 0 || vertInput != 0)
        {
            movement.x = horizInput * moveSpeed;
            movement.z = vertInput * moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);

            Quaternion tmp = target.rotation;

            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement);
            target.rotation = tmp;

            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
        }
        bool hitGround = false;
        RaycastHit hit;
        if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            float check = (_charController.height + _charController.radius) / 1.9f;
            hitGround = hit.distance <= check;
        }

        if (hitGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                // _animator.SetBool("Jumping", true);
                _vertSpeed = jumpSpeed;
            }
            else
            {
                _vertSpeed = minFall;
            }
        }
        else
        {
            _vertSpeed += gravity * 5 * Time.deltaTime;
            if (_vertSpeed < terminalVelocity)
            {
                _vertSpeed = terminalVelocity;
            }
            if (_charController.isGrounded)
            {
                if (Vector3.Dot(movement, _contact.normal) < 0)
                {
                    movement = _contact.normal * moveSpeed;
                }
                else
                {
                    movement += _contact.normal * moveSpeed;
                }
            }
        }
        movement.y = _vertSpeed;

        movement *= Time.deltaTime;
        _charController.Move(movement);
    }
    private void onControllerColliderHit(ControllerColliderHit hit)
    {
        _contact = hit;
    }
}

