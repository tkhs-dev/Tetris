﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TetrisCore.Source.Config
{
    public interface ISerializeable
    {
        public Type GetType();
    }
}
