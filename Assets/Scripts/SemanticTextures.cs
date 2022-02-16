using System.Collections;
using System.Collections.Generic;

using Niantic.ARDK;
using Niantic.ARDK.AR;
using Niantic.ARDK.Utilities;

using Niantic.ARDK.Extensions;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.AR.Awareness;
using Niantic.ARDK.AR.Awareness.Semantics;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SemanticTextures : MonoBehaviour
{
    //pass in our semantic manager
    public ARSemanticSegmentationManager _semanticManager;

    public Material _shaderMaterial;

    private Texture2D _semanticTexture;

    int channel;

    public TMP_Text _text;

    void Start()
    {
        //add a callback for catching the updated semantic buffer
        _semanticManager.SemanticBufferUpdated += OnSemanticsBufferUpdated;
    }

    //will be called when there is a new buffer
    private void OnSemanticsBufferUpdated(ContextAwarenessStreamUpdatedArgs<ISemanticBuffer> args)
    {
        //get the buffer that has been surfaced.
        ISemanticBuffer semanticBuffer = args.Sender.AwarenessBuffer;

        //channel = semanticBuffer.GetChannelIndex("sky");

        //get the channel from the buffer we would like to use using create or update.
        semanticBuffer.CreateOrUpdateTextureARGB32(
                   ref _semanticTexture, channel
               );
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //pass in our texture
        //Our Depth Buffer
        _shaderMaterial.SetTexture("_SemanticTex", _semanticTexture);

        //pass in our transform - samplerTransform will translate it to align with screen orientation
        _shaderMaterial.SetMatrix("_semanticTransform", _semanticManager.SemanticBufferProcessor.SamplerTransform);

        //blit everything with our shader
        Graphics.Blit(source, destination, _shaderMaterial);
    }

    private void Update()
    {
        
        if (PlatformAgnosticInput.touchCount <= 0) { return; }

        var touch = PlatformAgnosticInput.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            //list the channels that are available
            //Debug.Log("Number of Channels available " + _semanticManager.SemanticBufferProcessor.ChannelCount);
            //foreach (var c in _semanticManager.SemanticBufferProcessor.Channels)
              //  Debug.Log(c);


            int x = (int)touch.position.x;
            int y = (int)touch.position.y;

            //return the indices
            int[] channelsInPixel = _semanticManager.SemanticBufferProcessor.GetChannelIndicesAt(x, y);

            //print them to console
            foreach (var i in channelsInPixel)
            {
                channel = i;
                Debug.Log("channel " + i);
            }

            //return the names
            string[] channelsNamesInPixel = _semanticManager.SemanticBufferProcessor.GetChannelNamesAt(x, y);

            //print them to console
            foreach (var i in channelsNamesInPixel)
            {
                Debug.Log(i);

                //print to screen
                _text.text = i;
            }

        
           
        }
        
    }
}