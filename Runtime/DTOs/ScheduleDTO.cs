using System;

using UnityEngine;

namespace Virtuademy.SDK.ApiData
{
    [Serializable]
    public class ScheduleDTO
    {
        [SerializeField] private string day;
        [SerializeField] private TimeSpan startHour;
        [SerializeField] private TimeSpan endHour;
        [SerializeField] private string status;
        [SerializeField] private bool isOpen;
        [SerializeField] private DateTime now;

        public string Day { get => day; set => day = value; }
        public TimeSpan StartHour { get => startHour; set => startHour = value; }
        public TimeSpan EndHour { get => endHour; set => endHour = value; }
        public string Status { get => status; set => status = value; }
        public bool IsOpen { get => isOpen; set => isOpen = value; }
        public DateTime Now { get => now; set => now = value; }
    }
}

