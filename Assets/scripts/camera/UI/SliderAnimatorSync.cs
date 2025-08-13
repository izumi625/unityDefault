using UnityEngine;
using UnityEngine.UI;

public class SliderAnimatorSync : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;

    [Header("Animator")]
    public Animator animator;
    int stateHash;
    public int layer = 0;

    void Reset()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (slider != null) slider.onValueChanged.AddListener(OnSliderChanged);

    }

    void OnDisable()
    {
        if (slider != null) slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {

        value = Mathf.Clamp01(value);

        if (stateHash == 0) CacheCurrentStateHash();


        animator.Play(stateHash, layer, value);
        animator.Update(0f);
    }

    // �i�C�Ӂj�Đ����ɃX���C�_�[�֔��f���������ꍇ
    void Update()
    {
        if (animator != null && animator.speed > 0f && slider != null)
        {
            var info = animator.GetCurrentAnimatorStateInfo(layer);
            float norm = info.normalizedTime % 1f;
            if (norm < 0f) norm += 1f;
            slider.SetValueWithoutNotify(norm);
        }
    }

    void CacheCurrentStateHash()
    {
        var info = animator.IsInTransition(layer)
            ? animator.GetNextAnimatorStateInfo(layer)
            : animator.GetCurrentAnimatorStateInfo(layer);
        stateHash = info.fullPathHash;
    }

}
