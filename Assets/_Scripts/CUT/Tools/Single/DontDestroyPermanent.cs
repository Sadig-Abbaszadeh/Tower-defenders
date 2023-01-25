using System;
using UnityEngine;

namespace DartsGames.CUT
{
    public class DontDestroyPermanent : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}