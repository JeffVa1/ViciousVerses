using UnityEngine;

public class Meter : MonoBehaviour
{
    // FIELDS
    private int barValue;

    // CONSTRUCTER
    public Meter(int barValue) {
        this.barValue = barValue;
    }

    // GETTERS AND SETTERS
    public int GetBarValue() {
        return barValue;
    }

    public void SetBarValue(int value) {
        this.barValue = value;
    }

    // METHODS

    public void AddToBar(int value) {
        SetBarValue(GetBarValue() + value);
    }

    public void TakeFromBar(int value) {
        SetBarValue(GetBarValue() - value);
    }
}
