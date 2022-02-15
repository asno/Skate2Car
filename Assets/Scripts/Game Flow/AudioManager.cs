using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [SerializeField]
    private bool m_muteAudio = false;
    [SerializeField]
    AudioSource m_bgm;
    [SerializeField]
    AudioSource m_sfxColor;
    [SerializeField]
    AudioSource m_sfxJump;
    [SerializeField]
    AudioSource m_sfxBonus;

    public static AudioManager Instance { get => _instance ??= FindObjectOfType<AudioManager>(); }

    void Awake()
    {
        Debug.Assert(m_bgm != null, "Unexpected null reference to m_bgm");
        Debug.Assert(m_sfxColor != null, "Unexpected null reference to m_sfxColor");
        Debug.Assert(m_sfxJump != null, "Unexpected null reference to m_sfxJump");
        Debug.Assert(m_sfxBonus != null, "Unexpected null reference to m_sfxBonus");
    }

    public void PlayBGM()
    {
        PlayAudioSource(m_bgm);
    }

    public void StopBGM()
    {
        m_bgm.Stop();
    }

    public void PlayColorSelect()
    {
        PlayAudioSource(m_sfxColor);
    }

    public void PlayJump()
    {
        PlayAudioSource(m_sfxJump);
    }

    public void PlayBonusPick()
    {
        PlayAudioSource(m_sfxBonus);
    }

    private void PlayAudioSource(AudioSource aAudioSource)
    {
        if (!m_muteAudio)
            aAudioSource.Play();
    }
}
