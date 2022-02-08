using System.Collections;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D[] m_characterControllers;

    private IEnumerator m_characterControllerIterator;
    private CharacterController2D m_currentCharacterController;

    public CharacterController2D CurrentCharacterController 
    { 
        get
        {
            InitIfNull();
            return m_currentCharacterController;
        }
    }

    void Awake()
    {
        Debug.Assert(m_characterControllers != null, "Unexpected null reference to m_characterControllers");
        Debug.Assert(m_characterControllers.Length > 0, "Empty container m_characterControllers");

        InitIfNull();
    }

    private void InitIfNull()
    {
        if (m_characterControllerIterator == null)
        {
            m_characterControllerIterator = m_characterControllers.GetEnumerator();
            PickNextCharacterController();
        }
    }

    public void Reset()
    {
        m_characterControllerIterator.Reset();
        PickNextCharacterController();
    }

    public void PickNextCharacterController()
    {
        if (m_characterControllerIterator.MoveNext())
            m_currentCharacterController = m_characterControllerIterator.Current as CharacterController2D;
    }
}
