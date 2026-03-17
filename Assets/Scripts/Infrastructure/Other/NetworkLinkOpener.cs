using System.Diagnostics;
using UnityEngine;

public class NetworkLinkOpener : MonoBehaviour
{
    private string _vkLink = "https://vk.com/sennof";
    private string _tgLink = "https://t.me/SennoProduction";

    public void OpenLink(string id)
    {
        switch (id)
        {
            case "vk":
                Process.Start(_vkLink);
                break;
            case "tg":
                Process.Start(_tgLink);
                break;
        }
    }
}
