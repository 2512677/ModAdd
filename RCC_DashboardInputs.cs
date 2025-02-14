//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2017 BoneCracker Games
// http://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Receiving inputs from active vehicle on your scene, and feeds dashboard needles, texts, images.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC UI Dashboard Inputs")]
public class RCC_DashboardInputs : MonoBehaviour
{

    // Getting an Instance of Main Shared RCC Settings.
    #region RCC Settings Instance

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

    #endregion

    public GameObject RPMNeedle;
    public GameObject KMHNeedle;
    public GameObject turboGauge;
    public GameObject turboNeedle;
    public GameObject NOSGauge;
    public GameObject NoSNeedle;
    public GameObject heatGauge;
    public GameObject heatNeedle;
    public GameObject fuelGauge;
    public GameObject fuelNeedle;

    private float RPMNeedleRotation = 0f;
    private float KMHNeedleRotation = 0f;
    private float BoostNeedleRotation = 0f;
    private float NoSNeedleRotation = 0f;
    private float heatNeedleRotation = 0f;
    private float fuelNeedleRotation = 0f;

    internal float RPM;
    internal float KMH;
    internal int direction = 1;
    internal float Gear;
    internal bool changingGear = false;
    internal bool NGear = false;

    internal bool ABS = false;
    internal bool ESP = false;
    internal bool Park = false;
    internal bool Headlights = false;

    internal RCC_CarControllerV3.IndicatorsOn indicators;

    void Update()
    {

        if (RCCSettings.uiType == RCC_Settings.UIType.None)
        {

            gameObject.SetActive(false);
            enabled = false;
            return;

        }

        GetValues();

    }

    void GetValues()
    {
        // Находим объект с тегом "Player".
        GameObject playerVehicle = GameObject.FindGameObjectWithTag("Player");

        if (playerVehicle == null)
            return; // Если машина игрока не найдена, ничего не делаем.

        RCC_CarControllerV3 playerCarController = playerVehicle.GetComponent<RCC_CarControllerV3>();

        if (playerCarController == null)
            return; // Если на объекте нет RCC_CarControllerV3, ничего не делаем.

        // Проверяем, может ли игрок управлять машиной.
        if (!playerCarController.canControl || playerCarController.externalController)
            return;

        if (NOSGauge)
        {
            NOSGauge.SetActive(playerCarController.useNOS);
        }

        if (turboGauge)
        {
            turboGauge.SetActive(playerCarController.useTurbo);
        }

        if (heatGauge)
        {
            heatGauge.SetActive(playerCarController.useEngineHeat);
        }

        if (fuelGauge)
        {
            fuelGauge.SetActive(playerCarController.useFuelConsumption);
        }

        RPM = playerCarController.engineRPM;
        KMH = playerCarController.speed;
        direction = playerCarController.direction;
        Gear = playerCarController.currentGear;
        changingGear = playerCarController.changingGear;
        NGear = playerCarController.NGear;

        ABS = playerCarController.ABSAct;
        ESP = playerCarController.ESPAct;
        Park = playerCarController.handbrakeInput > .1f;
        Headlights = playerCarController.lowBeamHeadLightsOn || playerCarController.highBeamHeadLightsOn;
        indicators = playerCarController.indicatorsOn;

        if (RPMNeedle)
        {
            RPMNeedleRotation = (playerCarController.engineRPM / 50f);
            RPMNeedle.transform.eulerAngles = new Vector3(RPMNeedle.transform.eulerAngles.x, RPMNeedle.transform.eulerAngles.y, -RPMNeedleRotation);
        }

        if (KMHNeedle)
        {
            KMHNeedleRotation = (RCCSettings.units == RCC_Settings.Units.KMH) ? playerCarController.speed : playerCarController.speed * 0.62f;
            KMHNeedle.transform.eulerAngles = new Vector3(KMHNeedle.transform.eulerAngles.x, KMHNeedle.transform.eulerAngles.y, -KMHNeedleRotation);
        }

        if (turboNeedle)
        {
            BoostNeedleRotation = (playerCarController.turboBoost / 30f) * 270f;
            turboNeedle.transform.eulerAngles = new Vector3(turboNeedle.transform.eulerAngles.x, turboNeedle.transform.eulerAngles.y, -BoostNeedleRotation);
        }

        if (NoSNeedle)
        {
            NoSNeedleRotation = (playerCarController.NoS / 100f) * 270f;
            NoSNeedle.transform.eulerAngles = new Vector3(NoSNeedle.transform.eulerAngles.x, NoSNeedle.transform.eulerAngles.y, -NoSNeedleRotation);
        }

        if (heatNeedle)
        {
            heatNeedleRotation = (playerCarController.engineHeat / 110f) * 270f;
            heatNeedle.transform.eulerAngles = new Vector3(heatNeedle.transform.eulerAngles.x, heatNeedle.transform.eulerAngles.y, -heatNeedleRotation);
        }

        if (fuelNeedle)
        {
            fuelNeedleRotation = (playerCarController.fuelTank / playerCarController.fuelTankCapacity) * 270f;
            fuelNeedle.transform.eulerAngles = new Vector3(fuelNeedle.transform.eulerAngles.x, fuelNeedle.transform.eulerAngles.y, -fuelNeedleRotation);
        }
    }
}


