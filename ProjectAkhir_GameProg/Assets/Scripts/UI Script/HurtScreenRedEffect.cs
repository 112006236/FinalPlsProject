using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HurtScreenRedEffect : MonoBehaviour
{
    private PostProcessVolume volume;
    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out vignette);
    }

    public void PlayHurtEffect()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
