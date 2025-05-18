using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingShaderController : MonoBehaviour
{
    public Image loadingImage;
    public Material material;
    private float progress = 0f;
    public bool isPlaying = false;
    public float speed = 1f;
    //ループの周期
    public float loopDuration = 1f;

    //effectを実行させているかのBool
    public bool isEffectLoop = false;
    
    public enum AreaState
    {
        ResidenceArea,
        RuinsArea
    }
    
    public AreaState areaState = AreaState.ResidenceArea;

    void Start()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            progress += Time.deltaTime * speed;
            
            float loopedProgress = Mathf.Repeat(progress, loopDuration);
            
            float normalized = loopedProgress / loopDuration;
            
            material.SetFloat("_EffectProgress", normalized);
        }
    }

    /// <summary>
    /// effectを発生させ、数秒後にstopEffectを実行
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayEffect()
    {
        isEffectLoop = true;
        loadingImage.enabled = true;
        isPlaying = true;
        
        
        yield return new WaitForSeconds(2.0f);
        
        StopEffect();
    }

    private void StopEffect()
    {
        isPlaying = false;
        loadingImage.enabled = false;
        
        isEffectLoop = false;
        Debug.Log("No effect");
    }
    
    public void ResetEffect()
    {
        progress = 0f;
        material.SetFloat("_EffectProgress", 0f);
    }
}