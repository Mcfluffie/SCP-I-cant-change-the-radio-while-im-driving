using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class ChangeVignette : MonoBehaviour
{
    
    public Volume globalVolume;
    public Material redWoodLands;

    [Range(0f, 1f)]
    public float strength;

    [Range (-100f, 100f)]
    public float saturation;

    [Range (0f, 1f)]
    public float shaderSaturation;

    [Range(0f, 1f)]
    public float shaderBloodiness;

    

    private InputDevice targetDevice;

    private void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {



        // create a list of input devices and find the one with specific characteristics 
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
        // find amungst the flaming ruins the controller set in the variables'primary button if it has it and give the bool of that primary button
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if (primaryButtonValue)
        {

            Debug.Log("pressing the button");
            redWoodLands.SetFloat("Saturation", shaderSaturation + 0.1f);
            redWoodLands.SetFloat("ColourRange", shaderBloodiness + 0.1f);

            strength += 0.1f;
            saturation += 0.1f;



        }
            

        VolumeProfile profile = globalVolume.sharedProfile;


        UnityEngine.Rendering.VolumeProfile volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

        UnityEngine.Rendering.Universal.Vignette vignette;

        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        vignette.intensity.Override(strength);

        // get the specific part of the unity engine rendering to access the colour adjustments override in the post processing volume
        UnityEngine.Rendering.Universal.ColorAdjustments colourAdj;

        // if volume profile is null try and find one via the unity engine code stated above
        // then attempt to create an exeption (throw exepetion) so it can find the correct variable required since URP doesn't like post processing
        if (!volumeProfile.TryGet(out colourAdj)) throw new System.NullReferenceException(nameof(colourAdj));

        colourAdj.saturation.Override(saturation);



    }

    public void ChangeVignetteStrength()
    {

    }


}
