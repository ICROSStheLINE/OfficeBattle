using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
	Slider slider;
	PlayerStats attachedPlayerStats;

    void Start()
    {
		slider = GetComponent<Slider>();
        attachedPlayerStats = GetComponentInParent<PlayerStats>();
    }

    void Update()
    {
        slider.value = attachedPlayerStats.health / attachedPlayerStats.maxHealth;
		if (slider.value == 0)
		{
			GameObject hpMeter = gameObject.transform.Find("Fill Area").gameObject.transform.Find("Fill").gameObject;
			hpMeter.SetActive(false);
		}
    }
	
	
}
