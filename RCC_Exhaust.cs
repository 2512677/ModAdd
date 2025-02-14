using UnityEngine;
using System.Collections;

[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC Exhaust")]
public class RCC_Exhaust : MonoBehaviour
{

    private RCC_Settings RCCSettingsInstance;
    private RCC_Settings RCCSettings
    {
        get
        {
            if (RCCSettingsInstance == null)
            {
                RCCSettingsInstance = RCC_Settings.Instance;
            }
            return RCCSettingsInstance;
        }
    }

    private RCC_CarControllerV3 carController;
    private ParticleSystem particle;
    private ParticleSystem.EmissionModule emission;
    public ParticleSystem flame;
    private ParticleSystem.EmissionModule subEmission;

    private Light flameLight;
    private LensFlare lensFlare;

    public float flareBrightness = 1f;
    private float finalFlareBrightness;

    public float flameTime = 0f;
    private AudioSource flameSource;

    public Color flameColor = Color.red;
    public Color boostFlameColor = Color.blue;

    public bool previewFlames = false;

    public float minEmission = 5f;
    public float maxEmission = 50f;

    public float minSize = 2.5f;
    public float maxSize = 5f;

    public float minSpeed = .5f;
    public float maxSpeed = 5f;

    void Start()
    {

        if (RCCSettings == null || RCCSettings.dontUseAnyParticleEffects)
        {
            Destroy(gameObject);
            return;
        }

        carController = GetComponentInParent<RCC_CarControllerV3>();
        particle = GetComponent<ParticleSystem>();

        if (particle != null)
        {
            emission = particle.emission;
        }
        else
        {
            Debug.LogWarning("Particle system is missing on the exhaust object.", this);
        }

        if (flame != null)
        {
            subEmission = flame.emission;
            flameLight = flame.GetComponentInChildren<Light>();

            if (RCCSettings.exhaustFlameClips != null && RCCSettings.exhaustFlameClips.Length > 0)
            {
                flameSource = RCC_CreateAudioSource.NewAudioSource(gameObject, "Exhaust Flame AudioSource", 10f, 25f, 1f, RCCSettings.exhaustFlameClips[0], false, false, false);
            }
            else
            {
                Debug.LogWarning("No flame audio clips assigned in RCCSettings.", this);
            }

            if (flameLight != null)
            {
                flameLight.renderMode = RCCSettings.useLightsAsVertexLights ? LightRenderMode.ForceVertex : LightRenderMode.ForcePixel;
            }
        }
        else
        {
            Debug.LogWarning("Flame particle system is not assigned.", this);
        }

        lensFlare = GetComponentInChildren<LensFlare>();

        if (flameLight && flameLight.flare != null)
        {
            flameLight.flare = null;
        }
    }

    void Update()
    {
        if (carController == null || particle == null)
            return;

        Smoke();
        Flame();

        if (lensFlare != null)
            LensFlare();
    }

    void Smoke()
    {
        if (carController.engineRunning)
        {
            var main = particle.main;

            if (carController.speed < 50)
            {
                if (!emission.enabled)
                    emission.enabled = true;

                if (carController._gasInput > .35f)
                {
                    emission.rateOverTime = maxEmission;
                    main.startSpeed = maxSpeed;
                    main.startSize = maxSize;
                }
                else
                {
                    emission.rateOverTime = minEmission;
                    main.startSpeed = minSpeed;
                    main.startSize = minSize;
                }
            }
            else
            {
                if (emission.enabled)
                    emission.enabled = false;
            }
        }
        else
        {
            if (emission.enabled)
                emission.enabled = false;
        }
    }

    void Flame()
    {
        if (carController.engineRunning && flame != null && flameSource != null)
        {
            var main = flame.main;

            if (carController._gasInput >= .25f)
                flameTime = 0f;

            if (((carController.useExhaustFlame && carController.engineRPM >= 5000 && carController.engineRPM <= 5500 && carController._gasInput <= .25f && flameTime <= .5f) || carController._boostInput >= 1.5f) || previewFlames)
            {
                flameTime += Time.deltaTime;
                subEmission.enabled = true;

                if (flameLight != null)
                    flameLight.intensity = flameSource.pitch * 3f * Random.Range(.25f, 1f);

                if (carController._boostInput >= 1.5f)
                {
                    main.startColor = boostFlameColor;
                    if (flameLight != null) flameLight.color = main.startColor.color;
                }
                else
                {
                    main.startColor = flameColor;
                    if (flameLight != null) flameLight.color = main.startColor.color;
                }

                if (!flameSource.isPlaying)
                {
                    flameSource.clip = RCCSettings.exhaustFlameClips[Random.Range(0, RCCSettings.exhaustFlameClips.Length)];
                    flameSource.Play();
                }
            }
            else
            {
                subEmission.enabled = false;
                if (flameLight != null) flameLight.intensity = 0f;
                if (flameSource.isPlaying) flameSource.Stop();
            }
        }
        else
        {
            if (emission.enabled)
                emission.enabled = false;
            subEmission.enabled = false;
            if (flameLight != null)
                flameLight.intensity = 0f;
            if (flameSource != null && flameSource.isPlaying)
                flameSource.Stop();
        }
    }

    private void LensFlare()
    {
        if (RCC_SceneManager.Instance.activePlayerCamera == null || flameLight == null)
            return;

        float distanceToCam = Vector3.Distance(transform.position, RCC_SceneManager.Instance.activePlayerCamera.thisCam.transform.position);
        float angle = Vector3.Angle(transform.forward, RCC_SceneManager.Instance.activePlayerCamera.thisCam.transform.position - transform.position);

        if (angle != 0)
            finalFlareBrightness = flareBrightness * (4 / distanceToCam) * ((100f - (1.11f * angle)) / 100f) / 2f;

        lensFlare.brightness = finalFlareBrightness * flameLight.intensity;
        lensFlare.color = flameLight.color;
    }
}
