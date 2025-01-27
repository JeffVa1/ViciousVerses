using UnityEngine;

public class Meter : MonoBehaviour
{
    // FIELDS
    private int barValue;
    private int maxBarValue;

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

    public int GetMaxBarValue() {
        return maxBarValue;
    }

    public void SetMaxBarValue(int value) {
        this.maxBarValue = value;
    }

    // METHODS

    public void AddToBar(int value) {
        SetBarValue(GetBarValue() + value);
    }

    public void TakeFromBar(int value) {
        SetBarValue(GetBarValue() - value);
    }
}
