using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using DartsGames.CUT.UnityExtensions;

public class ThreadingManager : MonoBehaviour
{
    private Queue<Action> requestsQueue = new Queue<Action>();

    public void RegisterRequest(Action requestAction, Action finishedCallback)
    {
        ThreadStart threadBody = () =>
        {
            requestAction();

            lock (requestsQueue)
            {
                requestsQueue.Enqueue(finishedCallback);
            }
        };

        var evaluationThread = new Thread(threadBody);
        evaluationThread.Start();
    }

    public void RegisterRequest<TResult>(Func<TResult> requestAction, Action<TResult> finishedCallback)
    {
        ThreadStart threadBody = () =>
        {
            var result = requestAction();

            lock (requestsQueue)
            {
                requestsQueue.Enqueue(() => finishedCallback(result));
            }
        };

        var evaluationThread = new Thread(threadBody);
        evaluationThread.Start();
    }

    private void Update()
    {
        var count = requestsQueue.Count;

        if (count > 0)
        {
            lock (requestsQueue)
            {
                for (int i = 0; i < count; i++)
                {
                    requestsQueue.Dequeue()();
                }
            }
        }
    }
}