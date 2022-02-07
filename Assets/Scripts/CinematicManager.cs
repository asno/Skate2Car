using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    private static CinematicManager _instance;

    [SerializeField]
    private Cinematic[] m_cinematics;
    [SerializeField]
    private CharacterController2D m_characterController;

    private IEnumerator m_currentCinematic;
    private Coroutine m_currentCinematicProcess;
    private Game m_game;

    public static CinematicManager Instance { get => _instance; }

    void Awake()
    {
        _instance = this;

        Debug.Assert(m_cinematics != null, "Unexpected null reference to m_cinematics");
        Debug.Assert(m_cinematics.Length > 0, "Empty container m_cinematics");

        m_currentCinematic = m_cinematics.GetEnumerator();
    }

    private void OnEnable()
    {
        Reset();
    }

    void Start()
    {
        m_game = Game.Instance;
        Debug.Assert(m_game != null, "Unexpected null reference to m_game");
    }

    public void Reset()
    {
        m_currentCinematic.Reset();
    }

    public void StartNextCinematic()
    {
        if (!m_currentCinematic.MoveNext())
            return;

        m_characterController.CanControl = false;
        m_game.IsTimerPaused = true;

        if (m_currentCinematicProcess != null)
            StopCoroutine(m_currentCinematicProcess);

        m_currentCinematicProcess = StartCoroutine(PlayingCinematic());
    }

    private IEnumerator PlayingCinematic()
    {
        while (!m_characterController.CanMove())
            yield return null;

        Cinematic cinematic = m_currentCinematic.Current as Cinematic;
        cinematic.Play();
        while(cinematic.IsPlaying)
            yield return null;

        ResumeGame();
    }

    private void ResumeGame()
    {
        m_characterController.SwitchToNextState();
        m_characterController.CanControl = true;
        m_game.IsTimerPaused = false;
    }
}
