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
                PlayClip(0, 0.7f, playClipCancelSource.Token);
                break;
            case 1:
                PlayClip(1.25f, 1.7f, playClipCancelSource.Token);
                break;
            case 2:
                PlayClip(2.25f, 2.7f, playClipCancelSource.Token);
                break;
            case 3:
                PlayClip(3.25f, 3.7f, playClipCancelSource.Token);
                break;
            case 4:
                PlayClip(4.25f, 4.7f, playClipCancelSource.Token);
                break;
            case 5:
                PlayClip(5.25f, 5.7f, playClipCancelSource.Token);
                break;
            case 6:
                PlayClip(6.25f, 6.7f, playClipCancelSource.Token);
                break;
            case 7:
                PlayClip(7.25f, 7.7f, playClipCancelSource.Token);
                break;
            case 8:
                PlayClip(8.25f, 8.7f, playClipCancelSource.Token);
                break;
            case 9:
                PlayClip(9.25f, 9.7f, playClipCancelSource.Token);
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
        int rand = Random.Range(0, 10);
        if (other.tag == "Player")
        {
            SetAudioPart(rand);
        }
    }
}
