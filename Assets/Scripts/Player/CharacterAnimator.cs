using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private MovementController controller;
    [SerializeField] private Animator _animator;

    public FootstepAudioData footstepAudioData;
    public AudioSource currentFootAuido;
    [SerializeField]private float timePass = 0;

    bool isRun;
    bool isAir;
    bool isWallRun;
    bool isDash;

    private void Awake()
    {
        controller = GetComponentInParent<MovementController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (!controller.IsControllable)
        {
            _animator.SetBool("isAir", false);
            return;
        }

        AnimatiorCheck();
        SetAnimation();
        SoundHandler();
    }

    private void AnimatiorCheck()
    {
        isRun = PlayerInput._instance.HorizontalInput != 0;
        isAir = !controller.Grounded;
        isWallRun = controller.IsWallSliding || controller.IsWalkBamboo;
        isDash = controller.IsDashing;
    }

    private void SetAnimation()
    {
        _animator.SetBool("isDash", isDash);
        _animator.SetBool("isWallRuning", isWallRun);
        _animator.SetBool("isRun", isRun);
        _animator.SetBool("isAir", isAir);
        //_animator.SetBool("isDown", controller.Velocity.y < 0);

        if (controller.IsJumping)
        {
            _animator.SetTrigger("Jump");
        }

        if (controller.Grounded)
        {
            _animator.ResetTrigger("Jump");
        }
    }

    private void SoundHandler()
    {
        if (isDash)
        {
            FootstepAudio audioList = GetAudioClip("Dash");

            currentFootAuido.clip = audioList.audioClips[0];
            currentFootAuido.volume = audioList.audioVolume;

            currentFootAuido.Play();
        }

        if ((controller.IsJumping || isAir) && !isDash && controller.Velocity.y<-20 &&!isWallRun)
        {
            FootstepAudio audioList = GetAudioClip("Jump");
            //if (isAir && !isWallRun && !isDash)
            //{
            //    currentFootAuido.clip = audioList.audioClips[0];
            //    currentFootAuido.Play();
            //    return;
            //}
            currentFootAuido.volume = audioList.audioVolume;
            currentFootAuido.clip = audioList.audioClips[0];
            currentFootAuido.Play();
        }

        if (isRun && controller.Grounded|| controller.IsWallSliding)
        {
            timePass += Time.deltaTime;
            FootstepAudio audioList = GetAudioClip("Walk");
            float tempInterval = audioList.voiceInterval;

            if (timePass >= tempInterval)
            {
                int index = Random.Range(0, audioList.audioClips.Count);

                currentFootAuido.volume = audioList.audioVolume;
                currentFootAuido.clip = audioList.audioClips[index];

                currentFootAuido.Play();
                timePass = 0;
            }
        }

        if (controller.IsWalkBamboo)
        {
            timePass += Time.deltaTime;
            FootstepAudio audioList = GetAudioClip("Wood");
            float tempInterval = audioList.voiceInterval;

            if (timePass >= tempInterval)
            {
                int index = Random.Range(0, audioList.audioClips.Count);

                currentFootAuido.volume = audioList.audioVolume;
                currentFootAuido.clip = audioList.audioClips[index];
                currentFootAuido.Play();
                timePass = 0;
            }
        }

    }

    private FootstepAudio GetAudioClip(string tag)
    {
        FootstepAudio currentAudio = null;
        foreach (var audioList in footstepAudioData.footstepAudios)
        {
            if (audioList.Tag == tag)
            {
                currentAudio = audioList;
                break;
            }
        }
        return currentAudio;
    }
}
