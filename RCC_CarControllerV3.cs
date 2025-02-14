using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    [AddComponentMenu("BoneCracker Games/Realistic Car Controller/Main/RCC Realistic Car Controller V3")]
[RequireComponent(typeof(Rigidbody))]


public class RCC_CarControllerV3 : MonoBehaviour
{

    
   
    public enum SteeringWheelRotateAround
    {
        XAxis = 0,
        YAxis = 1,
        ZAxis = 2
    }

    public enum WheelType
    {
        FWD = 0,
        RWD = 1,
        AWD = 2,
        BIASED = 3
    }

    public enum AudioType
    {
        OneSource = 0,
        TwoSource = 1,
        ThreeSource = 2,
        Off = 3
    }

    public enum IndicatorsOn
    {
        Off = 0,
        Right = 1,
        Left = 2,
        All = 3
    }

    private struct originalMeshVerts
    {
        public Vector3[] meshVerts;
    }

    public delegate void onRCCPlayerSpawned(RCC_CarControllerV3 RCC);

    public delegate void onRCCPlayerDestroyed(RCC_CarControllerV3 RCC);

    public delegate void onRCCPlayerCollision(RCC_CarControllerV3 RCC, Collision collision);

    private RCC_Settings RCCSettingsInstance;

    internal Rigidbody rigid;

    internal bool sleepingRigid;

    internal bool isSleeping;

    public bool externalController;

    public Transform FrontLeftWheelTransform;

    public Transform FrontRightWheelTransform;

    public Transform RearLeftWheelTransform;

    public Transform RearRightWheelTransform;

    public RCC_WheelCollider FrontLeftWheelCollider;

    public RCC_WheelCollider FrontRightWheelCollider;

    public RCC_WheelCollider RearLeftWheelCollider;

    public RCC_WheelCollider RearRightWheelCollider;

    internal RCC_WheelCollider[] allWheelColliders;

    public Transform[] ExtraRearWheelsTransform;

    public RCC_WheelCollider[] ExtraRearWheelsCollider;

    public bool applyEngineTorqueToExtraRearWheelColliders = true;

    public Transform SteeringWheel;

    private Quaternion orgSteeringWheelRot;

    public SteeringWheelRotateAround steeringWheelRotateAround;

    public float steeringWheelAngleMultiplier = 3f;

    public WheelType _wheelTypeChoise = WheelType.RWD;

    [Range(0f, 100f)]
    public float biasedWheelTorque = 100f;

    public Transform COM;

    public bool canControl = true;

    public bool engineRunning;

    internal bool semiAutomaticGear;

    internal bool canGoReverseNow;

    public AnimationCurve[] engineTorqueCurve;

    public float[] targetSpeedForGear;

    public float[] maxSpeedForGear;

    public float engineTorque = 2000f;

    public float brakeTorque = 2000f;

    public float minEngineRPM = 1000f;

    public float maxEngineRPM = 7000f;

    [Range(0.75f, 2f)]
    public float engineInertia = 1f;

    public bool useRevLimiter = true;

    public bool useExhaustFlame = true;

    public bool useClutchMarginAtFirstGear = true;

    public bool steerAngleSensitivityAdjuster;

    public float steerAngle = 40f;

    public float highspeedsteerAngle = 15f;

    public float highspeedsteerAngleAtspeed = 100f;

    public float antiRollFrontHorizontal = 5000f;

    public float antiRollRearHorizontal = 5000f;

    public float antiRollVertical;

    public float downForce = 25f;

    public float speed;

    public float orgMaxSpeed;

    public float maxspeed = 220f;

    private float resetTime;

    private float orgSteerAngle;

    public bool useFuelConsumption;

    public float fuelTankCapacity = 62f;

    public float fuelTank = 62f;

    public float fuelConsumptionRate = 0.1f;

    public bool useEngineHeat;

    public float engineHeat = 15f;

    public float engineCoolingWaterThreshold = 60f;

    public float engineHeatRate = 1f;

    public float engineCoolRate = 1f;

    public int currentGear;

    public int totalGears = 6;

    [Range(0f, 0.5f)]
    public float gearShiftingDelay = 0.35f;

    [Range(0.5f, 0.95f)]
    public float gearShiftingThreshold = 0.85f;

    [Range(0.1f, 0.9f)]
    public float clutchInertia = 0.25f;

    private float orgGearShiftingThreshold;

    public bool changingGear;

    public bool NGear;

    public int direction = 1;

    public float launched;

    public bool autoGenerateGearCurves = true;

    public bool autoGenerateTargetSpeedsForChangingGear = true;

    public AudioType audioType;

    public bool autoCreateEngineOffSounds = true;

    private AudioSource engineStartSound;

    public AudioClip engineStartClip;

    internal AudioSource engineSoundHigh;

    public AudioClip engineClipHigh;

    private AudioSource engineSoundMed;

    public AudioClip engineClipMed;

    private AudioSource engineSoundLow;

    public AudioClip engineClipLow;

    public AudioSource engineSoundIdle;


    public AudioClip engineClipIdle;

    private AudioSource gearShiftingSound;

    internal AudioSource engineSoundHighOff;

    public AudioClip engineClipHighOff;

    internal AudioSource engineSoundMedOff;

    public AudioClip engineClipMedOff;

    internal AudioSource engineSoundLowOff;

    public AudioClip engineClipLowOff;

    private AudioSource crashSound;

    private AudioSource reversingSound;

    private AudioSource windSound;

    private AudioSource brakeSound;

    private AudioSource NOSSound;

    private AudioSource turboSound;

    private AudioSource blowSound;

    [Range(0f, 1f)]
    public float minEngineSoundPitch = 0.75f;

    [Range(1f, 2f)]
    public float maxEngineSoundPitch = 1.75f;

    [Range(0f, 1f)]
    public float minEngineSoundVolume = 0.05f;

    [Range(0f, 1f)]
    public float maxEngineSoundVolume = 0.85f;

    private GameObject allContactParticles;

    [HideInInspector]
    public float gasInput;

    [HideInInspector]
    public float brakeInput;

    [HideInInspector]
    public float steerInput;

    [HideInInspector]
    public float clutchInput;

    [HideInInspector]
    public float handbrakeInput;

    [HideInInspector]
    public float boostInput = 1f;

    [HideInInspector]
    public bool cutGas;

    [HideInInspector]
    public float idleInput;

    [HideInInspector]
    public float fuelInput;

    public bool permanentGas;

    internal float rawEngineRPM;

    internal float engineRPM;

    public GameObject chassis;

    public float chassisVerticalLean = 4f;

    public float chassisHorizontalLean = 4f;

    public bool lowBeamHeadLightsOn;

    public bool highBeamHeadLightsOn;

    public IndicatorsOn indicatorsOn;

    public float indicatorTimer;

    public bool useDamage = true;

    private originalMeshVerts[] originalMeshData;

    public MeshFilter[] deformableMeshFilters;

    public LayerMask damageFilter = -1;

    public float randomizeVertices = 1f;

    public float damageRadius = 0.5f;

    private float minimumVertDistanceForDamagedMesh = 0.002f;

    public bool repairNow;

    public bool repaired = true;

    public float maximumDamage = 0.5f;

    private float minimumCollisionForce = 5f;

    public float damageMultiplier = 1f;

    public int maximumContactSparkle = 5;

    private List<ParticleSystem> contactSparkeList = new List<ParticleSystem>();

    private Vector3 localVector;

    private Quaternion rot = Quaternion.identity;

    private float oldRotation;

    public Transform velocityDirection;

    public Transform steeringDirection;

    public float velocityAngle;

    private float angle;

    private float angularVelo;

    public bool ABS = true;

    public bool TCS = true;

    public bool ESP = true;

    public bool steeringHelper = true;

    public bool tractionHelper = true;

    [Range(0.05f, 0.5f)]
    public float ABSThreshold = 0.35f;

    [Range(0.05f, 0.5f)]
    public float TCSThreshold = 0.5f;

    [Range(0.05f, 1f)]
    public float TCSStrength = 1f;

    [Range(0.05f, 0.5f)]
    public float ESPThreshold = 0.25f;

    [Range(0.05f, 1f)]
    public float ESPStrength = 0.5f;

    [Range(0f, 1f)]
    public float steerHelperLinearVelStrength = 0.1f;

    [Range(0f, 1f)]
    public float steerHelperAngularVelStrength = 0.1f;

    [Range(0f, 1f)]
    public float tractionHelperStrength = 0.1f;

    public bool ABSAct;

    public bool TCSAct;

    public bool ESPAct;

    public bool underSteering;

    public bool overSteering;

    internal bool driftingNow;

    internal float driftAngle;

    public bool applyCounterSteering = true;

    [Range(0f, 1f)]
    public float counterSteeringFactor = 1f;

    public float frontSlip;

    public float rearSlip;

    public float turboBoost;

    public float NoS = 100f;

    private float NoSConsumption = 25f;

    private float NoSRegenerateTime = 10f;

    public bool useNOS;

    public bool useTurbo;

    


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

    [Obsolete("Warning 'AIController' is obsolete: 'Please use externalController.")]
    public bool AIController
    {
        get
        {
            return externalController;
        }
        set
        {
            externalController = value;
        }
    }

    public bool runEngineAtAwake
    {
        get
        {
            return RCCSettings.runEngineAtAwake;
        }
    }

    public bool autoReverse
    {
        get
        {
            return RCCSettings.autoReverse;
        }
    }

    public bool automaticGear
    {
        get
        {
            return RCCSettings.useAutomaticGear;
        }
    }

    private AudioClip[] gearShiftingClips
    {
        get
        {
            return RCCSettings.gearShiftingClips;
        }
    }

    private AudioClip[] crashClips
    {
        get
        {
            return RCCSettings.crashClips;
        }
    }

    private AudioClip reversingClip
    {
        get
        {
            return RCCSettings.reversingClip;
        }
    }

    private AudioClip windClip
    {
        get
        {
            return RCCSettings.windClip;
        }
    }

    private AudioClip brakeClip
    {
        get
        {
            return RCCSettings.brakeClip;
        }
    }

    private AudioClip NOSClip
    {
        get
        {
            return RCCSettings.NOSClip;
        }
    }

    private AudioClip turboClip
    {
        get
        {
            return RCCSettings.turboClip;
        }
    }

    private AudioClip blowClip
    {
        get
        {
            return RCCSettings.turboClip;
        }
    }

    internal float _gasInput
    {
        get
        {
            if (_fuelInput <= 0f)
            {
                return 0f;
            }
            if (!automaticGear || semiAutomaticGear)
            {
                if (!changingGear && !cutGas)
                {
                    return Mathf.Clamp01(gasInput);
                }
                return 0f;
            }
            if (!changingGear && !cutGas)
            {
                return (direction != 1) ? Mathf.Clamp01(brakeInput) : Mathf.Clamp01(gasInput);
            }
            return 0f;
        }
        set
        {
            gasInput = value;
        }
    }

    internal float _brakeInput
    {
        get
        {
            if (!automaticGear || semiAutomaticGear)
            {
                return Mathf.Clamp01(brakeInput);
            }
            if (!cutGas)
            {
                return (direction != 1) ? Mathf.Clamp01(gasInput) : Mathf.Clamp01(brakeInput);
            }
            return 0f;
        }
        set
        {
            brakeInput = value;
        }
    }

    internal float _boostInput
    {
        get
        {
            if (useNOS && NoS > 5f && _gasInput >= 0.5f)
            {
                return boostInput;
            }
            return 1f;
        }
        set
        {
            boostInput = value;
        }
    }

    internal float _steerInput
    {
        get
        {
            return steerInput + _counterSteerInput;
        }
    }

    internal float _counterSteerInput
    {
        get
        {
            if (applyCounterSteering)
            {
                return driftAngle * counterSteeringFactor;
            }
            return 0f;
        }
    }

    internal float _fuelInput
    {
        get
        {
            if (fuelTank > 0f)
            {
                return fuelInput;
            }
            if (engineRunning)
            {
                KillEngine();
            }
            return 0f;
        }
        set
        {
            fuelInput = value;
        }
    }

    public GameObject contactSparkle
    {
        get
        {
            return RCCSettings.contactParticles;
        }
    }

    public static event onRCCPlayerSpawned OnRCCPlayerSpawned;

    public static event onRCCPlayerDestroyed OnRCCPlayerDestroyed;

    public static event onRCCPlayerCollision OnRCCPlayerCollision;

    private void Awake()
    {
        if (RCCSettings.overrideFixedTimeStep)
        {
            Time.fixedDeltaTime = RCCSettings.fixedTimeStep;
        }
        rigid = GetComponent<Rigidbody>();
        rigid.maxAngularVelocity = RCCSettings.maxAngularVelocity;
        allWheelColliders = GetComponentsInChildren<RCC_WheelCollider>();
        FrontLeftWheelCollider.wheelModel = FrontLeftWheelTransform;
        FrontRightWheelCollider.wheelModel = FrontRightWheelTransform;
        RearLeftWheelCollider.wheelModel = RearLeftWheelTransform;
        RearRightWheelCollider.wheelModel = RearRightWheelTransform;
        for (int i = 0; i < ExtraRearWheelsCollider.Length; i++)
        {
            ExtraRearWheelsCollider[i].wheelModel = ExtraRearWheelsTransform[i];
        }
        orgSteerAngle = steerAngle;
        allContactParticles = new GameObject("All Contact Particles");
        allContactParticles.transform.SetParent(base.transform, false);
        SetTorqueCurves();
        SoundsInitialize();
        if (useDamage)
        {
            DamageInit();
        }
        if (runEngineAtAwake || externalController)
        {
            engineRunning = false;
            fuelInput = 1f;
        }
        if ((bool)chassis && !chassis.GetComponent<RCC_Chassis>())
        {
            chassis.AddComponent<RCC_Chassis>();
        }
        switch (RCCSettings.behaviorType)
        {
            case RCC_Settings.BehaviorType.SemiArcade:
                steeringHelper = true;
                tractionHelper = true;
                ABS = false;
                ESP = false;
                TCS = false;
                steerHelperLinearVelStrength = Mathf.Clamp(steerHelperLinearVelStrength, 0.5f, 1f);
                steerHelperAngularVelStrength = Mathf.Clamp(steerHelperAngularVelStrength, 1f, 1f);
                tractionHelperStrength = Mathf.Clamp(tractionHelperStrength, 0.25f, 1f);
                antiRollFrontHorizontal = Mathf.Clamp(antiRollFrontHorizontal, 10000f, float.PositiveInfinity);
                antiRollRearHorizontal = Mathf.Clamp(antiRollRearHorizontal, 10000f, float.PositiveInfinity);
                gearShiftingDelay = Mathf.Clamp(gearShiftingDelay, 0f, 0.1f);
                break;
            case RCC_Settings.BehaviorType.Drift:
                steeringHelper = true;
                tractionHelper = true;
                ABS = false;
                ESP = false;
                TCS = false;
                highspeedsteerAngle = Mathf.Clamp(highspeedsteerAngle, 40f, 50f);
                highspeedsteerAngleAtspeed = Mathf.Clamp(highspeedsteerAngleAtspeed, 100f, maxspeed);
                steerHelperAngularVelStrength = Mathf.Clamp(steerHelperAngularVelStrength, 0.05f, 0.05f);
                steerHelperLinearVelStrength = Mathf.Clamp(steerHelperLinearVelStrength, 0f, 0f);
                tractionHelperStrength = Mathf.Clamp(tractionHelperStrength, 0.05f, 0.05f);
                engineTorque = Mathf.Clamp(engineTorque, 2000f, float.PositiveInfinity);
                antiRollFrontHorizontal = Mathf.Clamp(antiRollFrontHorizontal, 1000f, float.PositiveInfinity);
                antiRollRearHorizontal = Mathf.Clamp(antiRollRearHorizontal, 1000f, float.PositiveInfinity);
                gearShiftingDelay = Mathf.Clamp(gearShiftingDelay, 0f, 0.15f);
                rigid.angularDrag = Mathf.Clamp(rigid.angularDrag, 0.5f, 1f);
                break;
            case RCC_Settings.BehaviorType.Fun:
                steeringHelper = true;
                tractionHelper = true;
                ABS = false;
                ESP = false;
                TCS = false;
                steerHelperLinearVelStrength = Mathf.Clamp(steerHelperLinearVelStrength, 0.5f, 1f);
                steerHelperAngularVelStrength = Mathf.Clamp(steerHelperAngularVelStrength, 1f, 1f);
                highspeedsteerAngle = Mathf.Clamp(highspeedsteerAngle, 30f, 50f);
                highspeedsteerAngleAtspeed = Mathf.Clamp(highspeedsteerAngleAtspeed, 100f, maxspeed);
                antiRollFrontHorizontal = Mathf.Clamp(antiRollFrontHorizontal, 20000f, float.PositiveInfinity);
                antiRollRearHorizontal = Mathf.Clamp(antiRollRearHorizontal, 20000f, float.PositiveInfinity);
                gearShiftingDelay = Mathf.Clamp(gearShiftingDelay, 0f, 0.1f);
                break;
            case RCC_Settings.BehaviorType.Racing:
                steeringHelper = true;
                tractionHelper = true;
                steerHelperLinearVelStrength = Mathf.Clamp(steerHelperLinearVelStrength, 0.25f, 1f);
                steerHelperAngularVelStrength = Mathf.Clamp(steerHelperAngularVelStrength, 0.25f, 1f);
                tractionHelperStrength = Mathf.Clamp(tractionHelperStrength, 0.25f, 1f);
                antiRollFrontHorizontal = Mathf.Clamp(antiRollFrontHorizontal, 10000f, float.PositiveInfinity);
                antiRollRearHorizontal = Mathf.Clamp(antiRollRearHorizontal, 10000f, float.PositiveInfinity);
                break;
            case RCC_Settings.BehaviorType.Simulator:
                antiRollFrontHorizontal = Mathf.Clamp(antiRollFrontHorizontal, 1000f, float.PositiveInfinity);
                antiRollRearHorizontal = Mathf.Clamp(antiRollRearHorizontal, 1000f, float.PositiveInfinity);
                break;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(RCCPlayerSpawned());
        currentGear = 0;
        changingGear = false;
    }

    private IEnumerator RCCPlayerSpawned()
    {
        yield return new WaitForEndOfFrame();
        if (!externalController && RCC_CarControllerV3.OnRCCPlayerSpawned != null)
        {
            RCC_CarControllerV3.OnRCCPlayerSpawned(this);
        }
    }

    public void CreateWheelColliders()
    {
        List<Transform> list = new List<Transform>();
        list.Add(FrontLeftWheelTransform);
        list.Add(FrontRightWheelTransform);
        list.Add(RearLeftWheelTransform);
        list.Add(RearRightWheelTransform);
        if (ExtraRearWheelsTransform.Length > 0 && (bool)ExtraRearWheelsTransform[0])
        {
            Transform[] extraRearWheelsTransform = ExtraRearWheelsTransform;
            foreach (Transform item in extraRearWheelsTransform)
            {
                list.Add(item);
            }
        }
        if (list != null && list[0] == null)
        {
            Debug.LogError("You haven't choose your Wheel Models. Please select all of your Wheel Models before creating Wheel Colliders. Script needs to know their sizes and positions, aye?");
            return;
        }
        Quaternion rotation = base.transform.rotation;
        base.transform.rotation = Quaternion.identity;
        GameObject gameObject = new GameObject("Wheel Colliders");
        gameObject.transform.SetParent(base.transform, false);
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;
        foreach (Transform item2 in list)
        {
            GameObject gameObject2 = new GameObject(item2.transform.name);
            gameObject2.transform.position = item2.transform.position;
            gameObject2.transform.rotation = base.transform.rotation;
            gameObject2.transform.name = item2.transform.name;
            gameObject2.transform.SetParent(gameObject.transform);
            gameObject2.transform.localScale = Vector3.one;
            gameObject2.AddComponent<WheelCollider>();
            Bounds bounds = default(Bounds);
            Renderer[] componentsInChildren = item2.GetComponentsInChildren<Renderer>();
            Renderer[] array = componentsInChildren;
            foreach (Renderer renderer in array)
            {
                if (renderer != GetComponent<Renderer>() && renderer.bounds.size.z > bounds.size.z)
                {
                    bounds = renderer.bounds;
                }
            }
            gameObject2.GetComponent<WheelCollider>().radius = bounds.extents.y / base.transform.localScale.y;
            gameObject2.AddComponent<RCC_WheelCollider>();
            JointSpring suspensionSpring = gameObject2.GetComponent<WheelCollider>().suspensionSpring;
            suspensionSpring.spring = 40000f;
            suspensionSpring.damper = 1500f;
            suspensionSpring.targetPosition = 0.5f;
            gameObject2.GetComponent<WheelCollider>().suspensionSpring = suspensionSpring;
            gameObject2.GetComponent<WheelCollider>().suspensionDistance = 0.2f;
            gameObject2.GetComponent<WheelCollider>().forceAppPointDistance = 0f;
            gameObject2.GetComponent<WheelCollider>().mass = 40f;
            gameObject2.GetComponent<WheelCollider>().wheelDampingRate = 1f;
            WheelFrictionCurve sidewaysFriction = gameObject2.GetComponent<WheelCollider>().sidewaysFriction;
            WheelFrictionCurve forwardFriction = gameObject2.GetComponent<WheelCollider>().forwardFriction;
            forwardFriction.extremumSlip = 0.3f;
            forwardFriction.extremumValue = 1f;
            forwardFriction.asymptoteSlip = 0.8f;
            forwardFriction.asymptoteValue = 0.6f;
            forwardFriction.stiffness = 1.5f;
            sidewaysFriction.extremumSlip = 0.3f;
            sidewaysFriction.extremumValue = 1f;
            sidewaysFriction.asymptoteSlip = 0.5f;
            sidewaysFriction.asymptoteValue = 0.8f;
            sidewaysFriction.stiffness = 1.5f;
            gameObject2.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction;
            gameObject2.GetComponent<WheelCollider>().forwardFriction = forwardFriction;
        }
        RCC_WheelCollider[] array2 = new RCC_WheelCollider[list.Count];
        array2 = GetComponentsInChildren<RCC_WheelCollider>();
        FrontLeftWheelCollider = array2[0];
        FrontRightWheelCollider = array2[1];
        RearLeftWheelCollider = array2[2];
        RearRightWheelCollider = array2[3];
        ExtraRearWheelsCollider = new RCC_WheelCollider[ExtraRearWheelsTransform.Length];
        for (int k = 0; k < ExtraRearWheelsTransform.Length; k++)
        {
            ExtraRearWheelsCollider[k] = array2[k + 4];
        }
        base.transform.rotation = rotation;
    }

    private void SoundsInitialize()
    {
        switch (audioType)
        {
            case AudioType.OneSource:
                engineSoundHigh = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High AudioSource", 5f, 50f, 0f, engineClipHigh, true, true, false);
                if (autoCreateEngineOffSounds)
                {
                    engineSoundHighOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High Off AudioSource", 5f, 50f, 0f, engineClipHigh, true, true, false);
                    RCC_CreateAudioSource.NewLowPassFilter(engineSoundHighOff, 3000f);
                }
                else
                {
                    engineSoundHighOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High Off AudioSource", 5f, 50f, 0f, engineClipHighOff, true, true, false);
                }
                break;
            case AudioType.TwoSource:
                engineSoundHigh = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High AudioSource", 5f, 50f, 0f, engineClipHigh, true, true, false);
                engineSoundLow = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Low AudioSource", 5f, 25f, 0f, engineClipLow, true, true, false);
                if (autoCreateEngineOffSounds)
                {
                    engineSoundHighOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High Off AudioSource", 5f, 50f, 0f, engineClipHigh, true, true, false);
                    engineSoundLowOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Low Off AudioSource", 5f, 25f, 0f, engineClipLow, true, true, false);
                    RCC_CreateAudioSource.NewLowPassFilter(engineSoundHighOff, 3000f);
                    RCC_CreateAudioSource.NewLowPassFilter(engineSoundLowOff, 3000f);
                }
                else
                {
                    engineSoundHighOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High Off AudioSource", 5f, 50f, 0f, engineClipHighOff, true, true, false);
                    engineSoundLowOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Low Off AudioSource", 5f, 25f, 0f, engineClipLowOff, true, true, false);
                }
                break;
            case AudioType.ThreeSource:
                engineSoundHigh = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High AudioSource", 5f, 50f, 0f, engineClipHigh, true, true, false);
                engineSoundMed = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Medium AudioSource", 5f, 50f, 0f, engineClipMed, true, true, false);
                engineSoundLow = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Low AudioSource", 5f, 25f, 0f, engineClipLow, true, true, false);
                if (autoCreateEngineOffSounds)
                {
                    engineSoundHighOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High Off AudioSource", 5f, 50f, 0f, engineClipHigh, true, true, false);
                    engineSoundMedOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Medium Off AudioSource", 5f, 50f, 0f, engineClipMed, true, true, false);
                    engineSoundLowOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Low Off AudioSource", 5f, 25f, 0f, engineClipLow, true, true, false);
                    if ((bool)engineSoundHighOff)
                    {
                        RCC_CreateAudioSource.NewLowPassFilter(engineSoundHighOff, 3000f);
                    }
                    if ((bool)engineSoundMedOff)
                    {
                        RCC_CreateAudioSource.NewLowPassFilter(engineSoundMedOff, 3000f);
                    }
                    if ((bool)engineSoundLowOff)
                    {
                        RCC_CreateAudioSource.NewLowPassFilter(engineSoundLowOff, 3000f);
                    }
                }
                else
                {
                    engineSoundHighOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound High Off AudioSource", 5f, 50f, 0f, engineClipHighOff, true, true, false);
                    engineSoundMedOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Medium Off AudioSource", 5f, 50f, 0f, engineClipMedOff, true, true, false);
                    engineSoundLowOff = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Low Off AudioSource", 5f, 25f, 0f, engineClipLowOff, true, true, false);
                }
                break;
        }
        engineSoundIdle = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Sound Idle AudioSource", 5f, 25f, 0f, engineClipIdle, true, true, false);
        reversingSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Reverse Sound AudioSource", 1f, 10f, 0f, reversingClip, true, false, false);
        windSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Wind Sound AudioSource", 1f, 10f, 0f, windClip, true, true, false);
        brakeSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Brake Sound AudioSource", 1f, 10f, 0f, brakeClip, true, true, false);
        if (useNOS)
        {
            NOSSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "NOS Sound AudioSource", 5f, 10f, 1f, NOSClip, true, false, false);
        }
        if (useNOS || useTurbo)
        {
            blowSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "NOS Blow", 1f, 10f, 1f, null, false, false, false);
        }
        if (useTurbo)
        {
            turboSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Turbo Sound AudioSource", 0.1f, 0.5f, 0f, turboClip, true, true, false);
            RCC_CreateAudioSource.NewHighPassFilter(turboSound, 10000f, 10);
        }
    }

    public void KillOrStartEngine()
    {
        if (engineRunning)
        {
            KillEngine();
        }
        else
        {
            StartEngine();
        }
    }

    public void StartEngine()
    {
        StartCoroutine(StartEngineDelayed());
    }

    public void StartEngine(bool instantStart)
    {
        if (instantStart)
        {
            fuelInput = 1f;
            engineRunning = true;
        }
        else
        {
            StartCoroutine(StartEngineDelayed());
        }
    }

    public IEnumerator StartEngineDelayed()
    {
        engineRunning = false;
        engineStartSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Engine Start AudioSource", 5f, 10f, 1f, engineStartClip, false, true, true);
        if (engineStartSound.isPlaying)
        {
            engineStartSound.Play();
        }
        yield return new WaitForSeconds(1f);
        engineRunning = true;
        fuelInput = 1f;
        yield return new WaitForSeconds(1f);
    }

    public void KillEngine()
    {
        fuelInput = 0f;
        engineRunning = false;
    }

    private void DamageInit()
    {
        if (deformableMeshFilters.Length == 0)
        {
            MeshFilter[] componentsInChildren = GetComponentsInChildren<MeshFilter>();
            List<MeshFilter> list = new List<MeshFilter>();
            MeshFilter[] array = componentsInChildren;
            foreach (MeshFilter meshFilter in array)
            {
                if (!meshFilter.transform.IsChildOf(FrontLeftWheelTransform) && !meshFilter.transform.IsChildOf(FrontRightWheelTransform) && !meshFilter.transform.IsChildOf(RearLeftWheelTransform) && !meshFilter.transform.IsChildOf(RearRightWheelTransform))
                {
                    list.Add(meshFilter);
                }
            }
            deformableMeshFilters = list.ToArray();
        }
        LoadOriginalMeshData();
        if ((bool)contactSparkle)
        {
            for (int j = 0; j < maximumContactSparkle; j++)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(contactSparkle, base.transform.position, Quaternion.identity);
                gameObject.transform.SetParent(allContactParticles.transform);
                contactSparkeList.Add(gameObject.GetComponent<ParticleSystem>());
                ParticleSystem.EmissionModule emission = gameObject.GetComponent<ParticleSystem>().emission;
                emission.enabled = false;
            }
        }
    }

    private void LoadOriginalMeshData()
    {
        originalMeshData = new originalMeshVerts[deformableMeshFilters.Length];
        for (int i = 0; i < deformableMeshFilters.Length; i++)
        {
            originalMeshData[i].meshVerts = deformableMeshFilters[i].mesh.vertices;
        }
    }

    private void Damage()
    {
        if (repaired || !repairNow)
        {
            return;
        }
        repaired = true;
        for (int i = 0; i < deformableMeshFilters.Length; i++)
        {
            Vector3[] vertices = deformableMeshFilters[i].mesh.vertices;
            if (originalMeshData == null)
            {
                LoadOriginalMeshData();
            }
            for (int j = 0; j < vertices.Length; j++)
            {
                vertices[j] += (originalMeshData[i].meshVerts[j] - vertices[j]) * (Time.deltaTime * 2f);
                if ((originalMeshData[i].meshVerts[j] - vertices[j]).magnitude >= minimumVertDistanceForDamagedMesh)
                {
                    repaired = false;
                }
            }
            deformableMeshFilters[i].mesh.vertices = vertices;
            deformableMeshFilters[i].mesh.RecalculateNormals();
            deformableMeshFilters[i].mesh.RecalculateBounds();
        }
        if (repaired)
        {
            repairNow = false;
        }
    }

    private void DeformMesh(Mesh mesh, Vector3[] originalMesh, Collision collision, float cos, Transform meshTransform, Quaternion rot)
    {
        Vector3[] vertices = mesh.vertices;
        ContactPoint[] contacts = collision.contacts;
        foreach (ContactPoint contactPoint in contacts)
        {
            Vector3 vector = meshTransform.InverseTransformPoint(contactPoint.point);
            for (int j = 0; j < vertices.Length; j++)
            {
                if ((vector - vertices[j]).magnitude < damageRadius)
                {
                    vertices[j] += rot * (localVector * (damageRadius - (vector - vertices[j]).magnitude) / damageRadius * cos + new Vector3(Mathf.Sin(vertices[j].y * 1000f), Mathf.Sin(vertices[j].z * 1000f), Mathf.Sin(vertices[j].x * 100f)).normalized * (randomizeVertices / 500f));
                    if (maximumDamage > 0f && (vertices[j] - originalMesh[j]).magnitude > maximumDamage)
                    {
                        vertices[j] = originalMesh[j] + (vertices[j] - originalMesh[j]).normalized * maximumDamage;
                    }
                }
            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void CollisionParticles(Vector3 contactPoint)
    {
        for (int i = 0; i < contactSparkeList.Count && !contactSparkeList[i].isPlaying; i++)
        {
            contactSparkeList[i].transform.position = contactPoint;
            ParticleSystem.EmissionModule emission = contactSparkeList[i].emission;
            emission.enabled = true;
            contactSparkeList[i].Play();
        }
    }

    private void OtherVisuals()
    {
        if ((bool)SteeringWheel)
        {
            if (orgSteeringWheelRot.eulerAngles == Vector3.zero)
            {
                orgSteeringWheelRot = SteeringWheel.transform.localRotation;
            }
            switch (steeringWheelRotateAround)
            {
                case SteeringWheelRotateAround.XAxis:
                    SteeringWheel.transform.localRotation = orgSteeringWheelRot * Quaternion.AngleAxis(FrontLeftWheelCollider.wheelCollider.steerAngle * steeringWheelAngleMultiplier, Vector3.right);
                    break;
                case SteeringWheelRotateAround.YAxis:
                    SteeringWheel.transform.localRotation = orgSteeringWheelRot * Quaternion.AngleAxis(FrontLeftWheelCollider.wheelCollider.steerAngle * steeringWheelAngleMultiplier, Vector3.up);
                    break;
                case SteeringWheelRotateAround.ZAxis:
                    SteeringWheel.transform.localRotation = orgSteeringWheelRot * Quaternion.AngleAxis(FrontLeftWheelCollider.wheelCollider.steerAngle * steeringWheelAngleMultiplier, Vector3.forward);
                    break;
            }
        }
    }

    private void Update()
    {
        if (canControl)
        {
            if (!externalController)
            {
                Inputs();
            }
        }
        else if (!externalController)
        {
            _gasInput = 0f;
            brakeInput = 0f;
            boostInput = 1f;
            handbrakeInput = 1f;
        }

        Sounds();
        ResetCar();

        if (useDamage)
        {
            Damage();
        }

        OtherVisuals();
        indicatorTimer += Time.deltaTime;

        if (_gasInput >= 0.1f)
        {
            launched += _gasInput * Time.deltaTime;
        }
        else
        {
            launched -= Time.deltaTime;
        }

        launched = Mathf.Clamp01(launched);

        // »справленный блок записи
        
            
        }
    



    private void Inputs()
    {
        if (RCCSettings.controllerType == RCC_Settings.ControllerType.Keyboard)
        {
            gasInput = Input.GetAxis(RCCSettings.verticalInput);
            brakeInput = Mathf.Clamp01(0f - Input.GetAxis(RCCSettings.verticalInput));
            handbrakeInput = ((!Input.GetKey(RCCSettings.handbrakeKB)) ? 0f : 1f);
            steerInput = Input.GetAxis(RCCSettings.horizontalInput);
            boostInput = ((!Input.GetKey(RCCSettings.boostKB)) ? 1f : 2.5f);
            if (Input.GetKeyDown(RCCSettings.lowBeamHeadlightsKB))
            {
                lowBeamHeadLightsOn = !lowBeamHeadLightsOn;
            }
            if (Input.GetKeyDown(RCCSettings.highBeamHeadlightsKB))
            {
                highBeamHeadLightsOn = true;
            }
            else if (Input.GetKeyUp(RCCSettings.highBeamHeadlightsKB))
            {
                highBeamHeadLightsOn = false;
            }
            if (Input.GetKeyDown(RCCSettings.startEngineKB))
            {
                KillOrStartEngine();
            }
            if (Input.GetKeyDown(RCCSettings.rightIndicatorKB))
            {
                if (indicatorsOn != IndicatorsOn.Right)
                {
                    indicatorsOn = IndicatorsOn.Right;
                }
                else
                {
                    indicatorsOn = IndicatorsOn.Off;
                }
            }
            if (Input.GetKeyDown(RCCSettings.leftIndicatorKB))
            {
                if (indicatorsOn != IndicatorsOn.Left)
                {
                    indicatorsOn = IndicatorsOn.Left;
                }
                else
                {
                    indicatorsOn = IndicatorsOn.Off;
                }
            }
            if (Input.GetKeyDown(RCCSettings.hazardIndicatorKB))
            {
                if (indicatorsOn != IndicatorsOn.All)
                {
                    indicatorsOn = IndicatorsOn.Off;
                    indicatorsOn = IndicatorsOn.All;
                }
                else
                {
                    indicatorsOn = IndicatorsOn.Off;
                }
            }
            if (Input.GetKeyDown(RCCSettings.NGear))
            {
                NGear = true;
            }
            if (Input.GetKeyUp(RCCSettings.NGear))
            {
                NGear = false;
            }
            if (!automaticGear)
            {
                if (Input.GetKeyDown(RCCSettings.shiftGearUp))
                {
                    GearShiftUp();
                }
                if (Input.GetKeyDown(RCCSettings.shiftGearDown))
                {
                    GearShiftDown();
                }
            }
        }
        if (permanentGas)
        {
            gasInput = 1f;
        }
    }

    private void FixedUpdate()
    {
        if (rigid.velocity.magnitude < 0.01f && Mathf.Abs(_steerInput) < 0.01f && Mathf.Abs(_gasInput) < 0.01f && Mathf.Abs(rigid.angularVelocity.magnitude) < 0.01f)
        {
            isSleeping = true;
        }
        else
        {
            isSleeping = false;
        }
        SetTorqueCurves();
        Engine();
        EngineSounds();
        if (canControl)
        {
            GearBox();
            Clutch();
        }
        AntiRollBars();
        DriftVariables();
        RevLimiter();
        Turbo();
        NOS();
        if (useFuelConsumption)
        {
            Fuel();
        }
        if (useEngineHeat)
        {
            EngineHeat();
        }
        if (steeringHelper)
        {
            SteerHelper();
        }
        if (tractionHelper)
        {
            TractionHelper();
        }
        if (ESP)
        {
            ESPCheck(FrontLeftWheelCollider.wheelCollider.steerAngle);
        }
        if (RCCSettings.behaviorType == RCC_Settings.BehaviorType.Drift && RearLeftWheelCollider.wheelCollider.isGrounded)
        {
            rigid.AddRelativeTorque(Vector3.up * (steerInput * _gasInput * (float)direction) / 1f, ForceMode.Acceleration);
        }
        rigid.centerOfMass = base.transform.InverseTransformPoint(COM.transform.position);
    }

    private void Engine()
    {
        speed = rigid.velocity.magnitude * 3.6f;
        steerAngle = Mathf.Lerp(orgSteerAngle, highspeedsteerAngle, speed / highspeedsteerAngleAtspeed);
        if (rigid.velocity.magnitude < 0.01f && Mathf.Abs(steerInput) < 0.01f && Mathf.Abs(_gasInput) < 0.01f && Mathf.Abs(rigid.angularVelocity.magnitude) < 0.01f)
        {
            sleepingRigid = true;
        }
        else
        {
            sleepingRigid = false;
        }
        float num = ((_wheelTypeChoise != 0) ? (RearLeftWheelCollider.wheelRPMToSpeed + RearRightWheelCollider.wheelRPMToSpeed) : (FrontLeftWheelCollider.wheelRPMToSpeed + FrontRightWheelCollider.wheelRPMToSpeed));
        rawEngineRPM = Mathf.Clamp(Mathf.MoveTowards(rawEngineRPM, maxEngineRPM * 1.1f * Mathf.Clamp01(Mathf.Lerp(0f, 1f, (1f - clutchInput) * (num * (float)direction / 2f / maxSpeedForGear[currentGear])) + (_gasInput * clutchInput + idleInput)), engineInertia * 100f), 0f, maxEngineRPM * 1.1f);
        rawEngineRPM *= _fuelInput;
        engineRPM = Mathf.Lerp(engineRPM, rawEngineRPM, Mathf.Lerp(Time.fixedDeltaTime * 5f, Time.fixedDeltaTime * 50f, rawEngineRPM / maxEngineRPM));
        if (autoReverse)
        {
            canGoReverseNow = true;
        }
        else if (_brakeInput < 0.5f && speed < 5f)
        {
            canGoReverseNow = true;
        }
        else if (_brakeInput > 0f && base.transform.InverseTransformDirection(rigid.velocity).z > 1f)
        {
            canGoReverseNow = false;
        }
    }

    private void Sounds()
    {
        windSound.volume = Mathf.Lerp(0f, RCCSettings.maxWindSoundVolume, speed / 300f);
        windSound.pitch = UnityEngine.Random.Range(0.9f, 1f);
        if (direction == 1)
        {
            brakeSound.volume = Mathf.Lerp(0f, RCCSettings.maxBrakeSoundVolume, Mathf.Clamp01((FrontLeftWheelCollider.wheelCollider.brakeTorque + FrontRightWheelCollider.wheelCollider.brakeTorque) / (brakeTorque * 2f)) * Mathf.Lerp(0f, 1f, FrontLeftWheelCollider.wheelCollider.rpm / 50f));
        }
        else
        {
            brakeSound.volume = 0f;
        }
    }

    private void ESPCheck(float steering)
    {
        WheelHit hit;
        FrontLeftWheelCollider.wheelCollider.GetGroundHit(out hit);
        WheelHit hit2;
        FrontRightWheelCollider.wheelCollider.GetGroundHit(out hit2);
        frontSlip = hit.sidewaysSlip + hit2.sidewaysSlip;
        WheelHit hit3;
        RearLeftWheelCollider.wheelCollider.GetGroundHit(out hit3);
        WheelHit hit4;
        RearRightWheelCollider.wheelCollider.GetGroundHit(out hit4);
        rearSlip = hit3.sidewaysSlip + hit4.sidewaysSlip;
        if (Mathf.Abs(frontSlip) >= ESPThreshold)
        {
            underSteering = true;
        }
        else
        {
            underSteering = false;
        }
        if (Mathf.Abs(rearSlip) >= ESPThreshold)
        {
            overSteering = true;
        }
        else
        {
            overSteering = false;
        }
        if (overSteering || underSteering)
        {
            ESPAct = true;
        }
        else
        {
            ESPAct = false;
        }
    }

    public void EngineSounds()
    {
        float num = 0f;
        float num2 = 0f;
        float num3 = 0f;
        num = ((!(engineRPM < maxEngineRPM / 2f)) ? Mathf.Lerp(1f, 0.25f, engineRPM / maxEngineRPM) : Mathf.Lerp(0f, 1f, engineRPM / (maxEngineRPM / 2f)));
        num2 = ((!(engineRPM < maxEngineRPM / 2f)) ? Mathf.Lerp(1f, 0.5f, engineRPM / maxEngineRPM) : Mathf.Lerp(-0.5f, 1f, engineRPM / (maxEngineRPM / 2f)));
        num3 = Mathf.Lerp(-1f, 1f, engineRPM / maxEngineRPM);
        num = Mathf.Clamp01(num) * maxEngineSoundVolume;
        num2 = Mathf.Clamp01(num2) * maxEngineSoundVolume;
        num3 = Mathf.Clamp01(num3) * maxEngineSoundVolume;
        float num4 = Mathf.Clamp(_gasInput, 0f, 1f);
        float pitch = Mathf.Lerp(engineSoundHigh.pitch, Mathf.Lerp(minEngineSoundPitch, maxEngineSoundPitch, engineRPM / 7000f), Time.fixedDeltaTime * 50f);
        switch (audioType)
        {
            case AudioType.OneSource:
                engineSoundHigh.volume = num4 * maxEngineSoundVolume;
                engineSoundHigh.pitch = pitch;
                engineSoundHighOff.volume = (1f - num4) * maxEngineSoundVolume;
                engineSoundHighOff.pitch = pitch;
                if (!engineSoundHigh.isPlaying)
                {
                    engineSoundHigh.Play();
                }
                break;
            case AudioType.TwoSource:
                engineSoundHigh.volume = num3 * num4;
                engineSoundHigh.pitch = pitch;
                engineSoundLow.volume = num * num4;
                engineSoundLow.pitch = pitch;
                engineSoundHighOff.volume = num3 * (1f - num4);
                engineSoundHighOff.pitch = pitch;
                engineSoundLowOff.volume = num * (1f - num4);
                engineSoundLowOff.pitch = pitch;
                if (!engineSoundLow.isPlaying)
                {
                    engineSoundLow.Play();
                }
                if (!engineSoundHigh.isPlaying)
                {
                    engineSoundHigh.Play();
                }
                break;
            case AudioType.ThreeSource:
                engineSoundHigh.volume = num3 * num4;
                engineSoundHigh.pitch = pitch;
                engineSoundMed.volume = num2 * num4;
                engineSoundMed.pitch = pitch;
                engineSoundLow.volume = num * num4;
                engineSoundLow.pitch = pitch;
                engineSoundHighOff.volume = num3 * (1f - num4);
                engineSoundHighOff.pitch = pitch;
                engineSoundMedOff.volume = num2 * (1f - num4);
                engineSoundMedOff.pitch = pitch;
                engineSoundLowOff.volume = num * (1f - num4);
                engineSoundLowOff.pitch = pitch;
                if (!engineSoundLow.isPlaying)
                {
                    engineSoundLow.Play();
                }
                if (!engineSoundMed.isPlaying)
                {
                    engineSoundMed.Play();
                }
                if (!engineSoundHigh.isPlaying)
                {
                    engineSoundHigh.Play();
                }
                break;
        }
        if ((bool)engineSoundIdle)
        {
            engineSoundIdle.volume = Mathf.Lerp((!engineRunning) ? 0f : 1f, 0f, engineRPM / maxEngineRPM);
            engineSoundIdle.pitch = pitch;
        }
    }

    private void AntiRollBars()
    {
        float num = 1f;
        float num2 = 1f;
        WheelHit hit;
        bool groundHit = FrontLeftWheelCollider.wheelCollider.GetGroundHit(out hit);
        if (groundHit)
        {
            num = (0f - FrontLeftWheelCollider.transform.InverseTransformPoint(hit.point).y - FrontLeftWheelCollider.wheelCollider.radius) / FrontLeftWheelCollider.wheelCollider.suspensionDistance;
        }
        bool groundHit2 = FrontRightWheelCollider.wheelCollider.GetGroundHit(out hit);
        if (groundHit2)
        {
            num2 = (0f - FrontRightWheelCollider.transform.InverseTransformPoint(hit.point).y - FrontRightWheelCollider.wheelCollider.radius) / FrontRightWheelCollider.wheelCollider.suspensionDistance;
        }
        float num3 = (num - num2) * antiRollFrontHorizontal;
        if (groundHit)
        {
            rigid.AddForceAtPosition(FrontLeftWheelCollider.transform.up * (0f - num3), FrontLeftWheelCollider.transform.position);
        }
        if (groundHit2)
        {
            rigid.AddForceAtPosition(FrontRightWheelCollider.transform.up * num3, FrontRightWheelCollider.transform.position);
        }
        float num4 = 1f;
        float num5 = 1f;
        WheelHit hit2;
        bool groundHit3 = RearLeftWheelCollider.wheelCollider.GetGroundHit(out hit2);
        if (groundHit3)
        {
            num4 = (0f - RearLeftWheelCollider.transform.InverseTransformPoint(hit2.point).y - RearLeftWheelCollider.wheelCollider.radius) / RearLeftWheelCollider.wheelCollider.suspensionDistance;
        }
        bool groundHit4 = RearRightWheelCollider.wheelCollider.GetGroundHit(out hit2);
        if (groundHit4)
        {
            num5 = (0f - RearRightWheelCollider.transform.InverseTransformPoint(hit2.point).y - RearRightWheelCollider.wheelCollider.radius) / RearRightWheelCollider.wheelCollider.suspensionDistance;
        }
        float num6 = (num4 - num5) * antiRollRearHorizontal;
        if (groundHit3)
        {
            rigid.AddForceAtPosition(RearLeftWheelCollider.transform.up * (0f - num6), RearLeftWheelCollider.transform.position);
        }
        if (groundHit4)
        {
            rigid.AddForceAtPosition(RearRightWheelCollider.transform.up * num6, RearRightWheelCollider.transform.position);
        }
        float num7 = (num - num4) * antiRollVertical;
        if (groundHit)
        {
            rigid.AddForceAtPosition(FrontLeftWheelCollider.transform.up * (0f - num7), FrontLeftWheelCollider.transform.position);
        }
        if (groundHit3)
        {
            rigid.AddForceAtPosition(RearLeftWheelCollider.transform.up * num7, RearLeftWheelCollider.transform.position);
        }
        float num8 = (num2 - num5) * antiRollVertical;
        if (groundHit2)
        {
            rigid.AddForceAtPosition(FrontRightWheelCollider.transform.up * (0f - num8), FrontRightWheelCollider.transform.position);
        }
        if (groundHit4)
        {
            rigid.AddForceAtPosition(RearRightWheelCollider.transform.up * num8, RearRightWheelCollider.transform.position);
        }
    }

    private void SteerHelper()
    {
        if (!steeringDirection || !velocityDirection)
        {
            if (!steeringDirection)
            {
                GameObject gameObject = new GameObject("Steering Direction");
                gameObject.transform.SetParent(base.transform, false);
                steeringDirection = gameObject.transform;
                gameObject.transform.localPosition = new Vector3(1f, 2f, 0f);
                gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 3f);
            }
            if (!velocityDirection)
            {
                GameObject gameObject2 = new GameObject("Velocity Direction");
                gameObject2.transform.SetParent(base.transform, false);
                velocityDirection = gameObject2.transform;
                gameObject2.transform.localPosition = new Vector3(-1f, 2f, 0f);
                gameObject2.transform.localScale = new Vector3(0.1f, 0.1f, 3f);
            }
            return;
        }
        for (int i = 0; i < allWheelColliders.Length; i++)
        {
            WheelHit hit;
            allWheelColliders[i].wheelCollider.GetGroundHit(out hit);
            if (hit.normal == Vector3.zero)
            {
                return;
            }
        }
        velocityAngle = rigid.angularVelocity.y * Mathf.Clamp(base.transform.InverseTransformDirection(rigid.velocity).z, -1f, 1f) * 57.29578f;
        velocityDirection.localRotation = Quaternion.Lerp(velocityDirection.localRotation, Quaternion.AngleAxis(Mathf.Clamp(velocityAngle / 3f, -45f, 45f), Vector3.up), Time.fixedDeltaTime * 20f);
        steeringDirection.localRotation = Quaternion.Euler(0f, FrontLeftWheelCollider.wheelCollider.steerAngle, 0f);
        int num = 1;
        num = ((steeringDirection.localRotation.y > velocityDirection.localRotation.y) ? 1 : (-1));
        float num2 = Quaternion.Angle(velocityDirection.localRotation, steeringDirection.localRotation) * (float)num;
        rigid.AddRelativeTorque(Vector3.up * (num2 * (Mathf.Clamp(base.transform.InverseTransformDirection(rigid.velocity).z, -10f, 10f) / 600f) * steerHelperAngularVelStrength), ForceMode.VelocityChange);
        if (Mathf.Abs(oldRotation - base.transform.eulerAngles.y) < 10f)
        {
            float num3 = (base.transform.eulerAngles.y - oldRotation) * (steerHelperLinearVelStrength / 2f);
            Quaternion quaternion = Quaternion.AngleAxis(num3, Vector3.up);
            rigid.velocity = quaternion * rigid.velocity;
        }
        oldRotation = base.transform.eulerAngles.y;
    }

    private void TractionHelper()
    {
        Vector3 velocity = rigid.velocity;
        velocity -= base.transform.up * Vector3.Dot(velocity, base.transform.up);
        velocity.Normalize();
        angle = 0f - Mathf.Asin(Vector3.Dot(Vector3.Cross(base.transform.forward, velocity), base.transform.up));
        angularVelo = rigid.angularVelocity.y;
        if (angle * FrontLeftWheelCollider.wheelCollider.steerAngle < 0f)
        {
            FrontLeftWheelCollider.tractionHelpedSidewaysStiffness = 1f - Mathf.Clamp01(tractionHelperStrength * Mathf.Abs(angularVelo));
        }
        else
        {
            FrontLeftWheelCollider.tractionHelpedSidewaysStiffness = 1f;
        }
        if (angle * FrontRightWheelCollider.wheelCollider.steerAngle < 0f)
        {
            FrontRightWheelCollider.tractionHelpedSidewaysStiffness = 1f - Mathf.Clamp01(tractionHelperStrength * Mathf.Abs(angularVelo));
        }
        else
        {
            FrontRightWheelCollider.tractionHelpedSidewaysStiffness = 1f;
        }
    }

    private void Clutch()
    {
        if (engineRunning)
        {
            idleInput = Mathf.Lerp(1f, 0f, engineRPM / minEngineRPM);
        }
        else
        {
            idleInput = 0f;
        }
        if (currentGear == 0)
        {
            if (useClutchMarginAtFirstGear)
            {
                if (launched >= 0.25f)
                {
                    clutchInput = Mathf.Lerp(clutchInput, Mathf.Lerp(1f, Mathf.Lerp(clutchInertia, 0f, (RearLeftWheelCollider.wheelRPMToSpeed + RearRightWheelCollider.wheelRPMToSpeed) / 2f / targetSpeedForGear[0]), Mathf.Abs(_gasInput)), Time.fixedDeltaTime * 5f);
                }
                else
                {
                    clutchInput = Mathf.Lerp(clutchInput, 1f / speed, Time.fixedDeltaTime * 5f);
                }
            }
            else
            {
                clutchInput = Mathf.Lerp(clutchInput, Mathf.Lerp(1f, Mathf.Lerp(clutchInertia, 0f, (RearLeftWheelCollider.wheelRPMToSpeed + RearRightWheelCollider.wheelRPMToSpeed) / 2f / targetSpeedForGear[0]), Mathf.Abs(_gasInput)), Time.fixedDeltaTime * 5f);
            }
        }
        else if (changingGear)
        {
            clutchInput = Mathf.Lerp(clutchInput, 1f, Time.fixedDeltaTime * 5f);
        }
        else
        {
            clutchInput = Mathf.Lerp(clutchInput, 0f, Time.fixedDeltaTime * 5f);
        }
        if (cutGas || handbrakeInput >= 0.1f)
        {
            clutchInput = 1f;
        }
        if (NGear)
        {
            clutchInput = 1f;
        }
        clutchInput = Mathf.Clamp01(clutchInput);
    }

    private void GearBox()
    {
        if (!externalController)
        {
            if (brakeInput > 0.9f && base.transform.InverseTransformDirection(rigid.velocity).z < 1f && canGoReverseNow && automaticGear && !semiAutomaticGear && !changingGear && direction != -1)
            {
                StartCoroutine(ChangeGear(-1));
            }
            else if (brakeInput < 0.1f && base.transform.InverseTransformDirection(rigid.velocity).z > -1f && direction == -1 && !changingGear && automaticGear && !semiAutomaticGear)
            {
                StartCoroutine(ChangeGear(0));
            }
        }
        if (automaticGear)
        {
            if (currentGear < totalGears - 1 && !changingGear && speed >= targetSpeedForGear[currentGear] * 0.9f && FrontLeftWheelCollider.wheelCollider.rpm > 0f)
            {
                if (!semiAutomaticGear)
                {
                    StartCoroutine(ChangeGear(currentGear + 1));
                }
                else if (semiAutomaticGear && direction != -1)
                {
                    StartCoroutine(ChangeGear(currentGear + 1));
                }
            }
            if (currentGear > 0 && !changingGear && speed < targetSpeedForGear[currentGear - 1] * 0.7f && direction != -1)
            {
                StartCoroutine(ChangeGear(currentGear - 1));
            }
        }
        if (direction == -1)
        {
            if (!reversingSound.isPlaying)
            {
                reversingSound.Play();
            }
            reversingSound.volume = Mathf.Lerp(0f, 1f, speed / targetSpeedForGear[0]);
            reversingSound.pitch = reversingSound.volume;
        }
        else
        {
            if (reversingSound.isPlaying)
            {
                reversingSound.Stop();
            }
            reversingSound.volume = 0f;
            reversingSound.pitch = 0f;
        }
    }

    public IEnumerator ChangeGear(int gear)
    {
        changingGear = true;
        if (RCCSettings.useTelemetry)
        {
          //MonoBehaviour.print("Shifted to: " + gear);
        }
        if (gearShiftingClips.Length > 0)
        {
            gearShiftingSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Gear Shifting AudioSource", 0f, 0.5f, RCCSettings.maxGearShiftingSoundVolume, gearShiftingClips[UnityEngine.Random.Range(0, gearShiftingClips.Length)], false, true, true);
            if (!gearShiftingSound.isPlaying)
            {
                gearShiftingSound.Play();
            }
        }
        yield return new WaitForSeconds(gearShiftingDelay);
        if (gear == -1)
        {
            currentGear = 0;
            if (!NGear)
            {
                direction = -1;
            }
            else
            {
                direction = 0;
            }
        }
        else
        {
            currentGear = gear;
            if (!NGear)
            {
                direction = 1;
            }
            else
            {
                direction = 0;
            }
        }
        changingGear = false;
    }

    public void GearShiftUp()
    {
        if (currentGear < totalGears - 1 && !changingGear)
        {
            if (direction != -1)
            {
                StartCoroutine(ChangeGear(currentGear + 1));
            }
            else
            {
                StartCoroutine(ChangeGear(0));
            }
        }
    }

    public void GearShiftDown()
    {
        if (currentGear >= 0)
        {
            StartCoroutine(ChangeGear(currentGear - 1));
        }
    }

    private void RevLimiter()
    {
        if (useRevLimiter && engineRPM >= maxEngineRPM)
        {
            cutGas = true;
        }
        else if (engineRPM < maxEngineRPM * 0.95f)
        {
            cutGas = false;
        }
    }

    private void NOS()
    {
        if (!useNOS)
        {
            return;
        }
        if (!NOSSound)
        {
            NOSSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "NOS Sound AudioSource", 5f, 10f, 1f, NOSClip, true, false, false);
        }
        if (!blowSound)
        {
            blowSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "NOS Blow", 1f, 10f, 1f, null, false, false, false);
        }
        if (boostInput > 1.5f && _gasInput >= 0.8f && NoS > 5f)
        {
            NoS -= NoSConsumption * Time.fixedDeltaTime;
            NoSRegenerateTime = 0f;
            if (!NOSSound.isPlaying)
            {
                NOSSound.Play();
            }
            return;
        }
        if (NoS < 100f && NoSRegenerateTime > 3f)
        {
            NoS += NoSConsumption / 1.5f * Time.fixedDeltaTime;
        }
        NoSRegenerateTime += Time.fixedDeltaTime;
        if (NOSSound.isPlaying)
        {
            NOSSound.Stop();
            blowSound.clip = RCCSettings.blowoutClip[UnityEngine.Random.Range(0, RCCSettings.blowoutClip.Length)];
            blowSound.Play();
        }
    }

    private void Turbo()
    {
        if (useTurbo)
        {
            if (!turboSound)
            {
                turboSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Turbo Sound AudioSource", 0.1f, 0.5f, 0f, turboClip, true, true, false);
                RCC_CreateAudioSource.NewHighPassFilter(turboSound, 10000f, 10);
            }
            turboBoost = Mathf.Lerp(turboBoost, Mathf.Clamp(Mathf.Pow(_gasInput, 10f) * 30f + Mathf.Pow(engineRPM / maxEngineRPM, 10f) * 30f, 0f, 30f), Time.fixedDeltaTime * 10f);
            if (turboBoost >= 25f && turboBoost < turboSound.volume * 30f && !blowSound.isPlaying)
            {
                blowSound.clip = RCCSettings.blowoutClip[UnityEngine.Random.Range(0, RCCSettings.blowoutClip.Length)];
                blowSound.Play();
            }
            turboSound.volume = Mathf.Lerp(turboSound.volume, turboBoost / 30f, Time.fixedDeltaTime * 5f);
            turboSound.pitch = Mathf.Lerp(Mathf.Clamp(turboSound.pitch, 2f, 3f), turboBoost / 30f * 2f, Time.fixedDeltaTime * 5f);
        }
    }

    private void Fuel()
    {
        fuelTank -= engineRPM / 10000f * fuelConsumptionRate * Time.fixedDeltaTime;
        fuelTank = Mathf.Clamp(fuelTank, 0f, fuelTankCapacity);
    }

    private void EngineHeat()
    {
        engineHeat += engineRPM / 10000f * engineHeatRate * Time.fixedDeltaTime;
        if (engineHeat > engineCoolingWaterThreshold)
        {
            engineHeat -= engineCoolRate * Time.fixedDeltaTime;
        }
        engineHeat -= engineCoolRate / 10f * Time.fixedDeltaTime;
        engineHeat = Mathf.Clamp(engineHeat, 15f, 120f);
    }

    private void DriftVariables()
    {
        WheelHit hit;
        RearRightWheelCollider.wheelCollider.GetGroundHit(out hit);
        if (Mathf.Abs(hit.sidewaysSlip) > 0.25f)
        {
            driftingNow = true;
        }
        else
        {
            driftingNow = false;
        }
        if (speed > 10f)
        {
            driftAngle = hit.sidewaysSlip * 0.75f;
        }
        else
        {
            driftAngle = 0f;
        }
    }

    private void ResetCar()
    {
        if (speed < 5f && !rigid.isKinematic && RCCSettings.autoReset && base.transform.eulerAngles.z < 300f && base.transform.eulerAngles.z > 60f)
        {
            resetTime += Time.deltaTime;
            if (resetTime > 3f)
            {
                base.transform.rotation = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
                base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 3f, base.transform.position.z);
                resetTime = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length < 1 || collision.relativeVelocity.magnitude < minimumCollisionForce)
        {
            return;
        }
        if (RCC_CarControllerV3.OnRCCPlayerCollision != null && this == RCC_SceneManager.Instance.activePlayerVehicle)
        {
            RCC_CarControllerV3.OnRCCPlayerCollision(this, collision);
        }
        if (!useDamage || ((1 << collision.gameObject.layer) & (int)damageFilter) == 0)
        {
            return;
        }
        CollisionParticles(collision.contacts[0].point);
        Vector3 relativeVelocity = collision.relativeVelocity;
        relativeVelocity *= 1f - Mathf.Abs(Vector3.Dot(base.transform.up, collision.contacts[0].normal));
        float num = Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, relativeVelocity.normalized));
        if (relativeVelocity.magnitude * num >= minimumCollisionForce)
        {
            repaired = false;
            localVector = base.transform.InverseTransformDirection(relativeVelocity) * (damageMultiplier / 50f);
            if (originalMeshData == null)
            {
                LoadOriginalMeshData();
            }
            for (int i = 0; i < deformableMeshFilters.Length; i++)
            {
                DeformMesh(deformableMeshFilters[i].mesh, originalMeshData[i].meshVerts, collision, num, deformableMeshFilters[i].transform, rot);
            }
        }
        if (crashClips.Length > 0 && collision.contacts[0].thisCollider.gameObject.transform != base.transform.parent)
        {
            crashSound = RCC_CreateAudioSource.NewAudioSource(base.gameObject, "Crash Sound AudioSource", 5f, 20f, RCCSettings.maxCrashSoundVolume, crashClips[UnityEngine.Random.Range(0, crashClips.Length)], false, true, true);
            if (!crashSound.isPlaying)
            {
                crashSound.Play();
            }
        }
    }

    private void OnDrawGizmos()
    {
    }

    // Ётот метод вызываетс€ через SendMessage из Respawner после респауна
    public void ResetValues()
    {
        // «апускаем корутину, котора€ "замораживает" автомобиль на 2 секунды
        StartCoroutine(FreezeAfterRespawn());
    }

    private IEnumerator FreezeAfterRespawn()
    {
        // ќтключаем управление
        bool previousControl = canControl;
        canControl = false;

        // ¬ременно делаем rigidbody кинематическим, чтобы физика не вли€ла на позицию
        bool previousKinematic = rigid.isKinematic;
        rigid.isKinematic = true;

        // ∆дем заданное врем€ (например, 2 секунды)
        yield return new WaitForSeconds(0.2f);

        // ¬озвращаем физику и управление в исходное состо€ние
        rigid.isKinematic = previousKinematic;
        canControl = previousControl;
    }


    public void SetTorqueCurves()
    {
        if (maxSpeedForGear == null)
        {
            maxSpeedForGear = new float[totalGears];
        }
        if (targetSpeedForGear == null)
        {
            targetSpeedForGear = new float[totalGears - 1];
        }
        if (maxSpeedForGear != null && maxSpeedForGear.Length != totalGears)
        {
            maxSpeedForGear = new float[totalGears];
        }
        if (targetSpeedForGear != null && targetSpeedForGear.Length != totalGears - 1)
        {
            targetSpeedForGear = new float[totalGears - 1];
        }
        for (int i = 0; i < totalGears; i++)
        {
            maxSpeedForGear[i] = Mathf.Lerp(0f, maxspeed * 1.1f, (float)(i + 1) / (float)totalGears);
        }
        if (autoGenerateTargetSpeedsForChangingGear)
        {
            for (int j = 0; j < totalGears - 1; j++)
            {
                targetSpeedForGear[j] = Mathf.Lerp(0f, maxspeed * Mathf.Lerp(0f, 1f, gearShiftingThreshold), (float)(j + 1) / (float)totalGears);
            }
        }
        if (!autoGenerateGearCurves || (orgMaxSpeed == maxspeed && orgGearShiftingThreshold == gearShiftingThreshold))
        {
            return;
        }
        if (totalGears < 1)
        {
            Debug.LogError("You are trying to set your vehicle gear to 0 or below! Why you trying to do this???");
            totalGears = 1;
            return;
        }
        engineTorqueCurve = new AnimationCurve[totalGears];
        currentGear = 0;
        for (int k = 0; k < engineTorqueCurve.Length; k++)
        {
            engineTorqueCurve[k] = new AnimationCurve(new Keyframe(0f, 1f));
        }
        for (int l = 0; l < totalGears; l++)
        {
            if (l != 0)
            {
                engineTorqueCurve[l].MoveKey(0, new Keyframe(0f, Mathf.Lerp(1f, 0.05f, (float)(l + 1) / (float)totalGears)));
                engineTorqueCurve[l].AddKey(Mathf.Lerp(0f, maxspeed * 0.5f, (float)l / (float)totalGears), Mathf.Lerp(1f, 0.5f, (float)l / (float)totalGears));
                engineTorqueCurve[l].AddKey(Mathf.Lerp(0f, maxspeed * 1f, (float)(l + 1) / (float)totalGears), 0.15f);
                engineTorqueCurve[l].AddKey(Mathf.Lerp(0f, maxspeed, (float)(l + 1) / (float)totalGears) * 2f, -3f);
                engineTorqueCurve[l].postWrapMode = WrapMode.Once;
            }
            else
            {
                engineTorqueCurve[l].MoveKey(0, new Keyframe(0f, 2f));
                engineTorqueCurve[l].AddKey(maxSpeedForGear[l] / 5f, 2.5f);
                engineTorqueCurve[l].AddKey(maxSpeedForGear[l], 0f);
                engineTorqueCurve[l].postWrapMode = WrapMode.Once;
            }
            orgMaxSpeed = maxspeed;
            orgGearShiftingThreshold = gearShiftingThreshold;
        }
    }

    public void PreviewSmokeParticle(bool state)
    {
        canControl = state;
        permanentGas = state;
        rigid.isKinematic = state;
    }

    private void OnDestroy()
    {
        if (RCC_CarControllerV3.OnRCCPlayerDestroyed != null)
        {
            RCC_CarControllerV3.OnRCCPlayerDestroyed(this);
        }
        if (canControl && (bool)base.gameObject.GetComponentInChildren<RCC_Camera>())
        {
            base.gameObject.GetComponentInChildren<RCC_Camera>().transform.SetParent(null);
        }
    }

    public void SetCanControl(bool state)
    {
        canControl = state;
    }

    public void SetEngine(bool state)
    {
        if (state)
        {
            StartEngine();
        }
        else
        {
            KillEngine();
        }
    }
}
