using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TetrisCore.Source.Config;
using static TetrisAI.Source.Evaluator;

namespace TetrisAI.Source
{
    public class ParameterConfig : ConfigBase
    {
        public EvaluationNNParameter Parameter { get; set; }
        public override Type GetType()
        {
            return typeof(ParameterConfig);
        }
        public static ParameterConfig Load(string path,string file_name)
        {
            return ConfigBase.Load(typeof(ParameterConfig),path,file_name) as ParameterConfig;
        }
    }
}
