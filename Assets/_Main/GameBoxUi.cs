using System;
using UnityEngine;

public class GameBoxUi : MonoBehaviour
{
    public GameBoxUiLayout[] layouts;
    private GameBoxUiLayout _currentLayout;
    
    public void SetSpots(int effectsCount)
    {
        foreach (var layout in layouts)
        {
            if (layout.spots.Length != effectsCount) continue;
            _currentLayout = layout;
            _currentLayout.parent.gameObject.SetActive(true);
        }
    }
        
    public RectTransform GetFreeSpot()
    {
        foreach (var spot in _currentLayout.spots)
        {
            if (spot.isTaken) continue;
            spot.isTaken = true;
            return spot.spotTransform;
        }
        return null;
    }

    public void ClearSpots()
    {
        foreach (var spot in _currentLayout.spots)
        { spot.isTaken = false; }
        _currentLayout.parent.gameObject.SetActive(false);
        _currentLayout = null;
    }
    
    [Serializable]
    public class GameBoxUiLayout
    {
        public RectTransform parent;
        public LayoutSpot[] spots;

        [Serializable]
        public class LayoutSpot
        {
            public RectTransform spotTransform;
            public bool isTaken;
        }
    }
}