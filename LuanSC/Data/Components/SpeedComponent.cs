using LuanSC.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuanSC.Data.Components
{
    public class SpeedComponent : IComponent
    {
        public string Name => "Speed Component";

        public GameObject Owner { get; set; }

        public int Speed { get; set; } = 100;
        public bool CanAct => Speed > 0;
        public SpeedComponent(int speed) 
        {
            Speed = speed;
        }

        public SpeedComponent() { }
        public void SetOwner(GameObject owner)
        {
            Owner = owner;
        }
    }
}
