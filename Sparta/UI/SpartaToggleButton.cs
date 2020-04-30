using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sparta.UI;
using System.Runtime.Serialization;

namespace Sparta.UI
{
    [DataContract]
    public class SpartaToggleButton : SpartaButton
    {
        public SpartaToggleButton()
        {
        }

        public SpartaToggleButton(string imageName, int width, int height, int enabledFrame, int disabledFrame)
        {
        }
    }
}
