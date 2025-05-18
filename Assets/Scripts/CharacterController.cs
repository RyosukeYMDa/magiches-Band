using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

//コンポーネント付け忘れ防止用Attribute(属性)
[RequireComponent(typeof(SkeletonAnimation))]
public class CharacterController : MonoBehaviour
{
    enum PlayerState
    {
        Idle,
        Walk
    }
    
    //playerのstate
    private PlayerState playerState = PlayerState.Idle;
    
    //playerの動き関連
    private float speed = 10f;
    private Vector3 moveInput;
    [SerializeField] private string testAnimationName = "walk";
    private Quaternion targetRotation;
    private float rotationSpeed = 10f;

    //spineAnimation関連
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spAnimationState;

    //カメラの位置
    [SerializeField] private Transform cameraTransform;
   
    [SerializeField] private LoadingShaderController loadingShaderController;
    [SerializeField] private CameraController cameraController;
    
    void Start()
    {
        //これが無いとspineのanimationが動かない
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spAnimationState = skeletonAnimation.AnimationState;
         
    }
    
    void Update()
    {
        //state管理
        if (moveInput != Vector3.zero && playerState != PlayerState.Walk)
        {
            ChangeState(PlayerState.Walk);
        }else if (moveInput == Vector3.zero && playerState != PlayerState.Idle)
        {
            ChangeState(PlayerState.Idle);
        }

        //
        if (moveInput != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //カメラの回転関連
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        
        Vector3 moveDirection = camForward * moveInput.z + camRight * moveInput.x;
        
        if (moveInput.z > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            Vector3 backward = -camForward;
            targetRotation = Quaternion.LookRotation(backward);
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    //playerの動きのInputSystem
    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //playerの移動
            Vector2 input = context.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
            
        }

        // 入力が終了したときは移動を止める
        if (context.canceled)
        {
            moveInput = Vector3.zero;
        }
    }

    /// <summary>
    /// Stateを変える関数
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(PlayerState newState)
    {
        if(playerState == newState)return;
        
        playerState = newState;

        //stateの状態によって再生するSpineAnimationを変える
        switch (newState)
        {
            case PlayerState.Idle:
                spAnimationState.SetAnimation(0, "idle", true);
                break;
            case PlayerState.Walk:
                spAnimationState.SetAnimation(0, "walk", true);
                break;
        }
    }
    
    
    /// <summary>
    /// 今はAreaを移動した際の処理を書いている
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //Area境にあるBoxColliderに当たった時のみ処理
        if (other.gameObject.CompareTag("CameraRotation"))
        {
            //loading用のShaderを再生
            StartCoroutine(loadingShaderController.PlayEffect());
            
            //スライドをしないように動きを止めさせる
            moveInput = Vector3.zero; 
            
            //Playerが今いるAreaのStateによって処理を変える
            if (loadingShaderController.areaState == LoadingShaderController.AreaState.ResidenceArea)
            {
                cameraController.CamRotation(90f);
                Debug.Log("Area移動");
                transform.position = new Vector3(other.transform.position.x + 3f, transform.position.y,
                    transform.position.z);
                loadingShaderController.areaState = LoadingShaderController.AreaState.RuinsArea;
            }
            else if (loadingShaderController.areaState == LoadingShaderController.AreaState.RuinsArea)
            {
                cameraController.CamRotation(-90f);
                Debug.Log("RuinsArea");
                transform.position = new Vector3(other.transform.position.x - 4f, transform.position.y,
                    transform.position.z);
                loadingShaderController.areaState = LoadingShaderController.AreaState.ResidenceArea;
            }

            // キャラの向きも調整（今の移動方向に合わせて）
            if (moveInput != Vector3.zero)
            {
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();
                Vector3 moveDirection = camForward * moveInput.z + camRight * moveInput.x;
                targetRotation = Quaternion.LookRotation(moveDirection);
            }
        }else if (other.gameObject.CompareTag("Enemy1"))
        {
            //Scene遷移時に再生する
            StartCoroutine(loadingShaderController.PlayEffect());
            
            //moveInput = Vector3.zero;
            
            //scene遷移時にplayerの座標を記録させる
            GameManager.Instance.playerPosition = other.gameObject.transform;
        }
    }
}