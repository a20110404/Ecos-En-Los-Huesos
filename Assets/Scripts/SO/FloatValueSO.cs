using UnityEngine;

[CreateAssetMenu(menuName = "Data/FloatData")]
public class FloatValueSO : ScriptableObject
{
    [SerializeField]
    private float _value;

    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            Debug.Log($"Valor actualizado: {_value}"); // Depuración
            OnValueChange?.Invoke(_value);
        }
    }
    public event System.Action<float> OnValueChange;
}