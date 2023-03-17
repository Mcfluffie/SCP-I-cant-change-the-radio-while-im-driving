using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class ChangeVignette : MonoBehaviour
{
    
    public Volume globalVolume;

    [Range(0f, 1f)]
    public float strength;

    [Range (-100f, 100f)]
    public float saturation;
    private float currentStrength;
    private float strengthToReach;

    public float timer;
    public float lerpTime;


    private InputDevice targetDevice;

    [Range (0, 100f)]
    public float sanityBar;


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

        if (primaryButtonValue || Input.GetKeyDown(KeyCode.K))
        {
            InsanityIncrease();

            
        }

//#if UNITY_EDITOR

//        if (primaryButtonValue)
//#else
//            if (primaryButtonValue)
//#endif
//        {

//            Debug.Log("pressing the button");
            
//            redWoodLands.SetFloat("_Saturation", shaderSaturation + 0.1f);
//            redWoodLands.SetFloat("_ColourRange", shaderBloodiness + 0.1f);


//            strength += 0.1f;
//            saturation += 0.1f;



//        }
            

        VolumeProfile profile = globalVolume.sharedProfile;


        UnityEngine.Rendering.VolumeProfile volumeProfile = GetComponent<UnityEngine.Rendering.Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

        UnityEngine.Rendering.Universal.Vignette vignette;

        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        vignette.intensity.Override(strength / 1.5f);

        // get the specific part of the unity engine rendering to access the colour adjustments override in the post processing volume
        UnityEngine.Rendering.Universal.ColorAdjustments colourAdj;

        // if volume profile is null try and find one via the unity engine code stated above
        // then attempt to create an exeption (throw exepetion) so it can find the correct variable required since URP doesn't like post processing
        if (!volumeProfile.TryGet(out colourAdj)) throw new System.NullReferenceException(nameof(colourAdj));

        colourAdj.saturation.Override(saturation);

       
        
    }

    public void InsanityIncrease()
    {
        // below will be based off of the scripts running to make 
        sanityBar += 15f;
        SanityMeter();
        strengthToReach = currentStrength + 1f;
        ChangeVignetteStrength();
        
    }

    public void SanityMeter()
    {
        // this code affects what events can occur

        // reset timer for sanity removal

        if (sanityBar >= 80)
        {
            // activate scripted events to have a chance to happen at this point
            Debug.Log("top brachet");

        }
        else if (sanityBar >= 65)
        {
            Debug.Log("65");
        }
        else if (sanityBar >= 60)
        {
            Debug.Log("60");
        }
        else if (sanityBar >= 55)
        {
            Debug.Log("55");
        }
        else if (sanityBar >= 30)
        {
            Debug.Log("30");
        }
      


        if (sanityBar >= 100)
        {
            sanityBar = 100;
        }
    }

    // Sanity and ChangeVignetteStrength need to be affected at once when the change comes in

    public void ChangeVignetteStrength()
    {
        Debug.Log("pressing the button");

        currentStrength = strength;

        // starts the coroutine and give it the timer it will take in seconds
        StartCoroutine(LerpingVignette(2));

        

        //strengthToReach += strengthIncrease

        //strength += 0.1f;

    }

    IEnumerator LerpingVignette(float time)
    {
        float i = 0;
        float rate = 1 / time;

        while(i <= 1f)
        {
            Debug.Log(i);
            Debug.Log(currentStrength);
            strength = Mathf.Lerp(currentStrength, currentStrength + 0.2f, i);
            Shader.SetGlobalFloat(Shader.PropertyToID("_Bloodiness"), strength);
            i += Time.deltaTime * rate;
            yield return 0;

        }
    }

    private void OnDisable()
    {
        Shader.SetGlobalFloat(Shader.PropertyToID("_Bloodiness"), 0);
    }

    private void OnEnable()
    {
        Shader.SetGlobalFloat(Shader.PropertyToID("_Bloodiness"), 0);
    }


}
