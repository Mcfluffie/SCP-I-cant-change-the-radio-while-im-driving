using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeVignette : MonoBehaviour
{
    
    public Volume globalVolume;
    public Shader redWoodLands;

    [Range(0f, 1f)]
    public float strength;

    [Range (-100f, 100f)]
    public float saturation;



    // Update is called once per frame
    void Update()
    {
        //if (xr controller button b is used, use below code)
        //{
            //add x amount to the strength of each variable in this script to allow maddness increase
        //}

        // the volume profile of the post processing is name is profile (feels a bit redundant but needs to stay for the project prototype)
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


}
