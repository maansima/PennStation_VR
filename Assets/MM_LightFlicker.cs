using UnityEngine;

public class MM_LightFlicker : MonoBehaviour
{
    Light pointLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 5f;

    float randomOffset;

    void Start()
    {
        pointLight = GetComponent<Light>();
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
        pointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}
