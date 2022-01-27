using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class PlayerJumpSound : MonoBehaviour
{
    private AudioSource jumpSound;

    private void Awake()
    {
        jumpSound = GetComponent<AudioSource>();
    }

    private async void SetAudioPart(int part)
    {
        if (playClipCancelSource != null)
        {
            playClipCancelSource.Cancel();
            playClipCancelSource.Dispose();
            playClipCancelSource = null;
            await Task.Delay(10);
        }
        playClipCancelSource = new CancellationTokenSource();
        switch (part)
        {
            case 0:
                PlayClip(0, 0.455f, playClipCancelSource.Token);
                break;
            case 1:
                PlayClip(0.455f, 0.91f, playClipCancelSource.Token);
                break;
            case 2:
                PlayClip(0.91f, 1.365f, playClipCancelSource.Token);
                break;
            case 3:
                PlayClip(1.365f, 1.82f, playClipCancelSource.Token);
                break;
            case 4:
                PlayClip(1.82f, 2.275f, playClipCancelSource.Token);
                break;
            case 5:
                PlayClip(2.275f, 2.73f, playClipCancelSource.Token);
                break;
            case 6:
                PlayClip(2.73f, 3.185f, playClipCancelSource.Token);
                break;
            case 7:
                PlayClip(3.185f, 3.64f, playClipCancelSource.Token);
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
        jumpSound.Stop();
        jumpSound.volume = 1;
        jumpSound.time = start;
        jumpSound.Play();
        do
        {
            jumpSound.volume -= (end - start) / 30;
            await Task.Yield();
            if (token.IsCancellationRequested)
            {
                jumpSound.volume = 1;
                jumpSound.Stop();
                return;
            }
        } while (jumpSound.time <= end);
        jumpSound.Stop();
        if (playClipCancelSource != null)
        {
            playClipCancelSource.Cancel();
            playClipCancelSource.Dispose();
            playClipCancelSource = null;
        }
    }

    public void JumpSound()
    {
        if (playClipCancelSource == null)
        {
            int rand = Random.Range(0, 8);
            SetAudioPart(rand);
        }
    }
}
