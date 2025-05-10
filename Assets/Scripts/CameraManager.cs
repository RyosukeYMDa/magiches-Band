using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    enum PlayerState
    {
        Idle,
        Move
    }
    
    private PlayerState playerState = PlayerState.Idle;
    
    private float speed = 10f;
    private Vector3 moveInput;
    [SerializeField] private string testAnimationName = "Move";

    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spAnimationState;

    private bool shouldPlayAnimation = false;

    private Quaternion targetRotation;
    private float rotationSpeed = 10f;

    [SerializeField] private Transform cameraTransform;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spAnimationState = skeletonAnimation.AnimationState;
    }
    
    void Update()
    {
        if (moveInput != Vector3.zero && playerState != PlayerState.Move)
        {
            ChangeState(PlayerState.Move);
        }else if (moveInput == Vector3.zero && playerState != PlayerState.Idle)
        {
            ChangeState(PlayerState.Idle);
        }

        if (moveInput != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * moveInput.z + camRight * moveInput.x;

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            string pressedKey = context.control.name;

            switch (pressedKey)
            {
                case "w":
                    Debug.Log("前");
                    SetRotation(-90f);
                    break;
                case "a":
                    Debug.Log("左");
                    SetRotation(-90f);
                    break;
                case "s":
                    Debug.Log("後");
                    SetRotation(90f);
                    break;
                case "d":
                    Debug.Log("右");
                    SetRotation(90f);
                    break;
            }
            
            Vector2 input = context.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
            
        }

        // 入力が終了したときは移動を止める
        if (context.canceled)
        {
            moveInput = Vector3.zero;
        }
    }

    private void SetRotation(float yAngle)
    {
        targetRotation = Quaternion.Euler(0f, yAngle, 0f);
    }

    private void ChangeState(PlayerState newState)
    {
        if(playerState == newState)return;
        
        playerState = newState;

        switch (newState)
        {
            case PlayerState.Idle:
                spAnimationState.SetAnimation(0, "Idle", true);
                break;
            case PlayerState.Move:
                spAnimationState.SetAnimation(0, "Move", true);
                break;
        }
    }
}