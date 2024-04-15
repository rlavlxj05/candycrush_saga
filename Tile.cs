using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public static Tile selected;
    public SpriteRenderer Renderer;
    public int Column { get; private set; }
    public int Row { get; private set; }
    public Sprite Sprite { get { return Renderer.sprite; } }

    private void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
        // Ÿ���� ���� ���� �����մϴ�.
        SetColumnAndRow();
    }

    void SetColumnAndRow()
    {
        // ���� Ÿ���� ��ġ�� �������� ���� ���� �����մϴ�.
        Column = Mathf.RoundToInt(transform.position.x / GameManager.Instance.Distance);
        Row = Mathf.RoundToInt(transform.position.y / GameManager.Instance.Distance);
    }
    public void Select()
    {
        Renderer.color = Color.grey;
        if (GameManager.Instance.firstSelectedTile == null)
        {
            GameManager.Instance.firstSelectedTile = this;

        }
        if (GameManager.Instance.firstSelectedTile != null)
        {
            if (GameManager.Instance.firstSelectedTile != null && selected != null)
            {
                int selectedColumn = Mathf.RoundToInt(selected.transform.position.x / GameManager.Instance.Distance);
                int selectedRow = Mathf.RoundToInt(selected.transform.position.y / GameManager.Instance.Distance);
                int firstSelectedColumn = Mathf.RoundToInt(GameManager.Instance.firstSelectedTile.transform.position.x / GameManager.Instance.Distance);
                int firstSelectedRow = Mathf.RoundToInt(GameManager.Instance.firstSelectedTile.transform.position.y / GameManager.Instance.Distance);

                bool isAdjacent = Mathf.Abs(selectedColumn - firstSelectedColumn) + Mathf.Abs(selectedRow - firstSelectedRow) == 1;

                if (isAdjacent)
                {
                    Debug.Log("�����մϴ�");
                    GameManager.Instance.SecondSelectedTile = selected;
                    GameManager.Instance.Exchange();

                }
                else
                {
                    Debug.Log("�������� �ʽ��ϴ�");
                    GameManager.Instance.firstSelectedTile = this;

                }
            }

        }
    }

    public void Unselect()
    {
        Renderer.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (selected != null)
        {
            selected.Unselect();
        }
        selected = this;
        Select();
    }
}