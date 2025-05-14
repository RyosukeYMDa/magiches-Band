using UnityEngine;

namespace  VariableSpace
{
    public enum AreaState
    {
        ResidenceArea,
        RuinArea
    }
    
    [CreateAssetMenu(fileName = "VariableDate", menuName = "Scriptable Object/VariableDate")]
    public class VariableDate : ScriptableObject
    {
        public AreaState areaState = AreaState.ResidenceArea;
    }
}

