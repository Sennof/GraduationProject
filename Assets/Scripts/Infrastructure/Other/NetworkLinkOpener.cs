using System.Diagnostics;
using UnityEngine;

public class NetworkLinkOpener : MonoBehaviour
{
    #region Fields

    private string _vkLink = "https://vk.com/sennof";
    private string _tgLink = "https://t.me/SennoProduction";

    #endregion


    #region Public Methods

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

    #endregion
}