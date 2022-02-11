using UnityEngine;

public class SelectScreen : Screen
{
    [SerializeField]
    private SpriteRenderer m_cursor;
    [SerializeField]
    private Row<Transform>[] m_selectGridPosition;
    [SerializeField]
    private Row<RuntimeAnimatorController>[] m_animatorControllers;
    private int m_currentX;
    private int m_currentY;
    private int m_dirX;
    private int m_dirY;
    private int m_numberOfRows;
    private bool m_toPause;

    private Timer m_timer;

    public override void Initialize()
    {
        base.Initialize();
        m_currentX = 0;
        m_currentY = 0;
        m_dirX = 0;
        m_dirY = 0;
        m_toPause = true;
        m_numberOfRows = m_selectGridPosition.Length;
        m_timer = GetComponent<Timer>();
        Deactivate();
    }

    protected override void Deactivate()
    {
        gameObject.SetActive(false);
        //m_timer.StopAndUnbind();
    }

    public override void Begin()
    {
        m_isSkipped = false;
        gameObject.SetActive(true);
        m_timer.RegisterCallbackEveryTick(0.25f, PollGridDirection);
        m_timer.Restart(true);
    }

    protected override void Exit()
    {
        base.Exit();
        Deactivate();
    }

    public override void DoUpdate()
    {
        int x = Mathf.CeilToInt(Input.GetAxis("Horizontal")) + Mathf.FloorToInt(Input.GetAxis("Horizontal"));
        int y = Mathf.CeilToInt(Input.GetAxis("Vertical")) + Mathf.FloorToInt(Input.GetAxis("Vertical"));
        if ((x != 0 && m_dirX == 0) || (y != 0 && m_dirY == 0))
        {
            m_toPause = false;
            m_timer.Resume();
            PollGridDirection();
        }
        else if (x == 0 && y == 0 && m_timer.IsPaused)
        {
            m_toPause = true;
        }
        //bool select = Input.GetButtonDown("Fire1");
    }

    private void PollGridDirection()
    {
        if (m_toPause)
            m_timer.Pause();

        m_dirX = Mathf.CeilToInt(Input.GetAxis("Horizontal")) + Mathf.FloorToInt(Input.GetAxis("Horizontal"));
        m_dirY = Mathf.CeilToInt(Input.GetAxis("Vertical")) + Mathf.FloorToInt(Input.GetAxis("Vertical"));
        var row = GetNearestRow(m_dirY);
        var item = GetNearestColumn(m_dirX, row);
        m_cursor.transform.position = item.position;
    }

    private Row<Transform> GetNearestRow(int aDirection)
    {
        Row<Transform> row;
        do
        {
            m_currentY += aDirection;
            m_currentY = ((m_currentY + m_numberOfRows) % m_numberOfRows);
            row = m_selectGridPosition[m_currentY];
        } while (row == null);

        return row;
    }

    private Transform GetNearestColumn(int aDirection, Row<Transform> aRow)
    {
        int numberOfColumns = aRow.m_column.Length;
        Transform column;
        do
        {
            m_currentX += aDirection;
            m_currentX = ((m_currentX + numberOfColumns) % numberOfColumns);
            column = aRow.m_column[m_currentX];
        } while (column == null);

        return column;
    }
}

[System.Serializable]
public class Row<T>
{
    public T[] m_column;
}
