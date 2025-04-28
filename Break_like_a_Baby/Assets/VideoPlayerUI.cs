using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class VideoPlayerUI : MonoBehaviour
{
    public RawImage rawImage;                 // UI RawImage to show video
    public string videoFileName = "intro.mp4"; // File name in StreamingAssets folder

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = transform.parent.GetChild(0).GetComponent<RawImage>();
        if(rawImage == null)
        {
            rawImage = transform.parent.GetChild(2).GetComponent<RawImage>();
        }
       
        if (videoPlayer == null)
        {
            Debug.LogError("No VideoPlayer component found!");
            return;
        }

        if (rawImage == null)
        {
            Debug.LogError("RawImage not assigned in inspector!");
            return;
        }
        //videoFileName = videoPlayer.clip.name;
        StartCoroutine(PlayVideoFromStreamingAssets());
    }

    IEnumerator PlayVideoFromStreamingAssets()
    {
        string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        RenderTexture rt = new RenderTexture(1920, 1080, 0);
        videoPlayer.targetTexture = rt;
        rawImage.texture = rt;

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }
}