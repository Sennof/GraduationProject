using TMPro;
using UnityEngine;

public class LinkBarManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _linkBarText;

    private string _linkBase = "https://";
    public void SetText(string text)
    {
        _linkBarText.text = _linkBase + text;
    }

    public void SetText(GameObject obj)
    {
        _linkBarText.text = _linkBase + obj.name;
    }

    public void SetTextWithParent(GameObject obj)
    {
        _linkBarText.text = $"{_linkBase}{obj.transform.parent.gameObject.name}/{obj.name}";
    }
}
