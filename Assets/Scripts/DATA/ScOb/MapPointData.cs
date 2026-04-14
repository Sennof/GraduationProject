using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapPointData", menuName = "BaseUIData/MapPointData")]
public class MapPointData : UIBaseData
{
    #region MapPointData

    [Header("MapPointData")]
    [Tooltip("Pictures associated with this map point.")]
    public List<Sprite> Pictures;

    [Tooltip("Feedbacks associated with this map point.")]
    public List<MapPointFeedbackData> Feedbacks;

    #endregion
}