using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//コンポーネント付け忘れ防止用Attribute(属性)
[RequireComponent(typeof(SkeletonAnimation))]
public class CharacterController : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Walk
    }

    //playerのstate
    private PlayerState playerState = PlayerState.Idle;

    //playerの動き関連
    private const float MoveSpeed = 10f;
    private Vector3 moveInput;
    [SerializeField] private string testAnimationName = "walk";
    private Quaternion targetRotation;
    private const float RotationSpeed = 10f;

    //spineAnimation関連
    private SkeletonAnimation skeletonAnimation;
    private Spine.AnimationState spAnimationState;

    //カメラの位置
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private LoadingShaderController loadingShaderController;
    [SerializeField] private CameraController cameraController;

    private void Start()
    {
        gameObject.transform.position = GameManager.Instance.playerPosition;

        //これが無いとspineのanimationが動かない
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spAnimationState = skeletonAnimation.AnimationState;
    }

    private void Update()
    {
        //state管理
        if (moveInput != Vector3.zero && playerState != PlayerState.Walk)
            ChangeState(PlayerState.Walk);
        else if (moveInput == Vector3.zero && playerState != PlayerState.Idle) ChangeState(PlayerState.Idle);

        //
        if (moveInput != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        //カメラの回転関連
        var camForward = cameraTransform.forward;
        var camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        var moveDirection = camForward * moveInput.z + camRight * moveInput.x;

        if (moveInput.z > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            var backward = -camForward;
            targetRotation = Quaternion.LookRotation(backward);
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        transform.Translate(moveDirection * (MoveSpeed * Time.deltaTime), Space.World);
    }

    //playerの動きのInputSystem
    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //playerの移動
            var input = context.ReadValue<Vector2>();
            moveInput = new Vector3(input.x, 0f, input.y);
        }

        // 入力が終了したときは移動を止める
        if (context.canceled) moveInput = Vector3.zero;
    }

    /// <summary>
    /// Stateを変える関数
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeState(PlayerState newState)
    {
        if (playerState == newState) return;

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
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
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

            switch (loadingShaderController.areaState)
            {
                //Playerが今いるAreaのStateによって処理を変える
                case LoadingShaderController.AreaState.ResidenceArea:
                    cameraController.CamRotation(90f);
                    Debug.Log("Area移動");
                    transform.position = new Vector3(other.transform.position.x + 3f, transform.position.y,
                        transform.position.z);
                    loadingShaderController.areaState = LoadingShaderController.AreaState.RuinsArea;
                    break;
                case LoadingShaderController.AreaState.RuinsArea:
                    cameraController.CamRotation(-90f);
                    Debug.Log("RuinsArea");
                    transform.position = new Vector3(other.transform.position.x - 4f, transform.position.y,
                        transform.position.z);
                    loadingShaderController.areaState = LoadingShaderController.AreaState.ResidenceArea;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // キャラの向きも調整（今の移動方向に合わせて）
            if (moveInput == Vector3.zero) return;
            var camForward = cameraTransform.forward;
            var camRight = cameraTransform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();
            var moveDirection = camForward * moveInput.z + camRight * moveInput.x;
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        else if (other.gameObject.CompareTag("Enemy1"))
        {
            //Scene遷移時に再生する
            StartCoroutine(loadingShaderController.PlayEffect());

            //scene遷移時にplayerの座標を記録させる
            GameManager.Instance.playerPosition = other.transform.position;

            GameManager.Instance.enemyType = 1;

            SceneManager.LoadScene("BattleScene");
        }
    }
}