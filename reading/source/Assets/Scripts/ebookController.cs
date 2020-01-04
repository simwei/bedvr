using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ebookController : MonoBehaviour
{
    public int m_currentPage = 0;

    private TextMeshProUGUI m_textMesh;

    private const string BOOK_FILE = "TheManOfTheForest.txt" ;

    private string m_bookText;
    private string[] m_bookTextArr;


    // Start is called before the first frame update
    void Start()
    {
        m_textMesh = GetComponent<TextMeshProUGUI>();

        m_bookText = File.ReadAllText(BOOK_FILE);
        m_bookTextArr = m_bookText.Split('\n');

        UpdateText();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            m_currentPage++;
            if (m_currentPage >= m_bookTextArr.Length / 25) m_currentPage = m_bookTextArr.Length / 25 - 1;
            UpdateText();
        }

        else if (Input.GetButtonDown("Fire1"))
        {
            m_currentPage--;
            if (m_currentPage < 0) m_currentPage = 0;
            UpdateText();
        }

    }


    void UpdateText()
    {
        m_textMesh.text = string.Join("\n", new List<string>(m_bookTextArr).GetRange(m_currentPage * 25, 30).ToArray()) + "\nPage " + (m_currentPage + 1) + " / " +  ( m_bookTextArr.Length / 25 );
    }
}
