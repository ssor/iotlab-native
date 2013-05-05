using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsServer
{
    public class LightState
    {
        public bool State;
        public string Name;
        public LightState(string _name, bool _state)
        {
            this.Name = _name;
            this.State = _state;
        }
    }
}
