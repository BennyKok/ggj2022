using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class FairySound : MonoBehaviour
{
    private AudioSource fairySound;

    private void Awake()
    {
        fairySound = GetComponent<AudioSource>();
    }

    private void SetAudioPart(int part)
    {
        if (playClipCancelSource != null)
        {
            playClipCancelSource.Cancel();
            playClipCancelSource.Dispose();
            playClipCancelSource = null;
        }
        playClipCancelSource = new CancellationTokenSource();
        switch (part)
        {
            case 0:
                PlayClip(0, 1.1f, playClipCancelSource.Token);
                break;
            case 1:
                PlayClip(1.1f, 2.22f, playClipCancelSource.Token);
                break;
            case 2:
                PlayClip(2.22f, 3.33f, playClipCancelSource.Token);
                break;
            case 3:
                PlayClip(3.33f, 4.44f, playClipCancelSource.Token);
                break;
            case 4:
                PlayClip(4.44f, 5.55f, playClipCancelSource.Token);
                break;
            case 5:
                PlayClip(5.55f, 6.66f, playClipCancelSource.Token);
                break;
            case 6:
                PlayClip(6.66f, 7.77f, playClipCancelSource.Token);
                break;
            case 7:
                PlayClip(7.77f, 8.88f, playClipCancelSource.Token);
                break;
            case 8:
                PlayClip(8.88f, 9.99f, playClipCancelSource.Token);
                break;
        }
    }

    private void OnDestroy()
    {
        if (playClipCancelSource != null)
        {
            playClipCancelSource.Cancel();
            playClipCancelSource.Dispose();
            playClipCancelSource = null;
        }
    }

    private CancellationTokenSource playClipCancelSource = null;
    private async void PlayClip(float start, float end, CancellationToken token)
    {
        fairySound.Stop();
        fairySound.time = start;
        fairySound.Play();
        do
        {
            if (token.IsCancellationRequested)
            {
                fairySound.Stop();
                return;
            }
            await Task.Yield();
        } while (fairySound.time <= end);
        fairySound.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        int rand = Random.Range(0, 9);
        if (other.tag == "Player")
        {
            SetAudioPart(rand);
        }
    }
}
