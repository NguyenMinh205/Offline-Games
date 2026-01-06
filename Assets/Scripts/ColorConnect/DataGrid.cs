using System.Collections.Generic;
using UnityEngine;

namespace NguyenQuangMinh.ColorConnect
{
    [System.Serializable]
    public class DataNode
    {
        public TypeOfColor colorType;
        public Vector2Int startPos;
        public Vector2Int endPos;
    }

    [CreateAssetMenu(fileName = "DataGrid", menuName = "ScriptableObject/ColorConnect/DataGrid")]
    public class DataGrid : ScriptableObject
    {
        public Vector2Int gridSize = new Vector2Int(8,8);
        public List<DataNode> dataNodes;
    }
}