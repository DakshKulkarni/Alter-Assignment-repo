using System.Collections.Generic;
using UnityEngine;

public class HandUITestSpawner : MonoBehaviour
{
    public HandUI handUI;
    private void Start()
    {
        handUI.SetHand(new List<int> { 1, 2, 3 });
    }
}
