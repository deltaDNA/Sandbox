using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeltaDNA
{
    public class DDNA_Parameter
    {
        public string name;
        public string description;
        public string type;
        public string[] enumeration = null;
        public string format = null;


    }

    public class DDNA_ParameterBlock
    {
        public DDNA_Parameter[] parameters;
    }
}