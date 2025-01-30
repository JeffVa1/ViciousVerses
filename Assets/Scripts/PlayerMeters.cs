using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMeters : MonoBehaviour
{

    public Meter Meters { get; private set; }
    public float CurrentHealth = 100f;
    public float MaxHealth = 100f;
    private float MinHealth = 0f;
    [SerializeField] public Image HealthBar;
    public float CurrentAudienceScore = 100f;
    public float MaxAudienceScore = 100f;
    private float MinAudienceScore = 0f;
    [SerializeField] public Image AudienceBar;
    

    
    public void Initialize(int AudienceScore)
    {
        Meters = new Meter(CurrentHealth,
                         MaxHealth, 
                         MinHealth, 
                         HealthBar,
                         CurrentAudienceScore,
                         MaxAudienceScore,
                         MinAudienceScore,
                         AudienceBar);

        Meters.SetAudienceBarValue(AudienceScore) ;
    }

    
}
