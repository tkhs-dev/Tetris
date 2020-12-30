using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisPlayerWPF
{
    public interface ITabablePage
    {
        public void Submit();
        public string GetSubmitText();
    }
}
