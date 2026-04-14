using System.Linq;
using UnityEngine;

public class FirstPersonAudio : MonoBehaviour
{
    #region Fields

    [Header("State")]
    [Tooltip("Enable or disable all audio playback.")]
    [SerializeField] private bool _enabled = true;

    [Header("Character References")]
    public FirstPersonMovement Character;
    public GroundCheck GroundCheck;

    [Header("Step Sounds")]
    public AudioSource StepAudio;
    public AudioSource RunningAudio;
    [Tooltip("Minimum velocity for moving audio to play.")]
    public float VelocityThreshold = .01f;

    [Header("Landing")]
    public AudioSource LandingAudio;
    public AudioClip[] LandingSFX;

    [Header("Jump")]
    public Jump Jump;
    public AudioSource JumpAudio;
    public AudioClip[] JumpSFX;

    [Header("Crouch")]
    public Crouch Crouch;
    public AudioSource CrouchStartAudio;
    public AudioSource CrouchedAudio;
    public AudioSource CrouchEndAudio;
    public AudioClip[] CrouchStartSFX;
    public AudioClip[] CrouchEndSFX;

    private Vector2 _lastCharacterPosition;
    private Vector2 CurrentCharacterPosition => new Vector2(Character.transform.position.x, Character.transform.position.z);

    private AudioSource[] MovingAudios => new AudioSource[] { StepAudio, RunningAudio, CrouchedAudio };

    #endregion


    #region Unity Methods

    private void Reset()
    {
        Character = GetComponentInParent<FirstPersonMovement>();
        GroundCheck = (transform.parent ?? transform).GetComponentInChildren<GroundCheck>();
        StepAudio = GetOrCreateAudioSource("Step Audio");
        RunningAudio = GetOrCreateAudioSource("Running Audio");
        LandingAudio = GetOrCreateAudioSource("Landing Audio");

        Jump = GetComponentInParent<Jump>();
        if (Jump)
        {
            JumpAudio = GetOrCreateAudioSource("Jump audio");
        }

        Crouch = GetComponentInParent<Crouch>();
        if (Crouch)
        {
            CrouchStartAudio = GetOrCreateAudioSource("Crouch Start Audio");
            CrouchedAudio = GetOrCreateAudioSource("Crouched Audio");
            CrouchEndAudio = GetOrCreateAudioSource("Crouch End Audio");
        }
    }

    private void OnEnable() => SubscribeToEvents();

    private void OnDisable() => UnsubscribeFromEvents();

    private void FixedUpdate()
    {
        if (_enabled)
        {
            float velocity = Vector3.Distance(CurrentCharacterPosition, _lastCharacterPosition);
            if (velocity >= VelocityThreshold && GroundCheck && GroundCheck.IsGrounded)
            {
                if (Crouch && Crouch.IsCrouched)
                {
                    SetPlayingMovingAudio(CrouchedAudio);
                }
                else if (Character.IsRunning)
                {
                    SetPlayingMovingAudio(RunningAudio);
                }
                else
                {
                    SetPlayingMovingAudio(StepAudio);
                }
            }
            else
            {
                SetPlayingMovingAudio(null);
            }

            _lastCharacterPosition = CurrentCharacterPosition;
        }
    }

    #endregion


    #region Public Methods

    public void Disable()
    {
        _enabled = false;

        StepAudio.mute = true;
        RunningAudio.mute = true;
        LandingAudio.mute = true;
        JumpAudio.mute = true;
        CrouchStartAudio.mute = true;
        CrouchedAudio.mute = true;
        CrouchEndAudio.mute = true;
    }

    public void Enable()
    {
        _enabled = true;

        StepAudio.mute = false;
        RunningAudio.mute = false;
        LandingAudio.mute = false;
        JumpAudio.mute = false;
        CrouchStartAudio.mute = false;
        CrouchedAudio.mute = false;
        CrouchEndAudio.mute = false;
    }

    #endregion


    #region Private Methods

    private void SetPlayingMovingAudio(AudioSource audioToPlay)
    {
        foreach (var audio in MovingAudios.Where(audio => audio != audioToPlay && audio != null))
        {
            audio.Pause();
        }

        if (audioToPlay && !audioToPlay.isPlaying)
        {
            audioToPlay.Play();
        }
    }

    private void PlayLandingAudio() => PlayRandomClip(LandingAudio, LandingSFX);
    private void PlayJumpAudio() => PlayRandomClip(JumpAudio, JumpSFX);
    private void PlayCrouchStartAudio() => PlayRandomClip(CrouchStartAudio, CrouchStartSFX);
    private void PlayCrouchEndAudio() => PlayRandomClip(CrouchEndAudio, CrouchEndSFX);

    private void SubscribeToEvents()
    {
        GroundCheck.Grounded += PlayLandingAudio;

        if (Jump)
        {
            Jump.Jumped += PlayJumpAudio;
        }

        if (Crouch)
        {
            Crouch.CrouchStart += PlayCrouchStartAudio;
            Crouch.CrouchEnd += PlayCrouchEndAudio;
        }
    }

    private void UnsubscribeFromEvents()
    {
        GroundCheck.Grounded -= PlayLandingAudio;

        if (Jump)
        {
            Jump.Jumped -= PlayJumpAudio;
        }

        if (Crouch)
        {
            Crouch.CrouchStart -= PlayCrouchStartAudio;
            Crouch.CrouchEnd -= PlayCrouchEndAudio;
        }
    }

    private AudioSource GetOrCreateAudioSource(string name)
    {
        AudioSource result = System.Array.Find(GetComponentsInChildren<AudioSource>(), a => a.name == name);
        if (result)
        {
            return result;
        }

        result = new GameObject(name).AddComponent<AudioSource>();
        result.spatialBlend = 1;
        result.playOnAwake = false;
        result.transform.SetParent(transform, false);
        return result;
    }

    private static void PlayRandomClip(AudioSource audio, AudioClip[] clips)
    {
        if (!audio || clips.Length <= 0)
        {
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        if (clips.Length > 1)
        {
            while (clip == audio.clip)
            {
                clip = clips[Random.Range(0, clips.Length)];
            }
        }

        audio.clip = clip;
        audio.Play();
    }

    #endregion
}