using System;
using Library;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Linq;

namespace Unit
{
    public interface IController
    {
        void Register(IControlable a_Controlable);
    }
}
