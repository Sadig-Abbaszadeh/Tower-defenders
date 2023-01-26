using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTest : MonoBehaviour
{
    private async void Start()
    {
        var time = await WaitForRandom();

        Debug.Log(time);
    }

    private async Task<float> WaitForRandom()
    {
        var randomTime = Random.Range(3f, 10f);

        await Task.Delay((int)(randomTime * 1000));

        return randomTime;
    }
}