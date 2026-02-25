using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapPointData", menuName = "BaseUIData/MapPointData")]
public class MapPointData : UIBaseData
{
    #region MapPointData
    [Header("MapPointData")]
    public List<Sprite> Pictures;

    public List<MapPointFeedbackData> Feedbacks;
    #endregion
}
