using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator
{
    public class Enums
    {
        public enum ReceiveType
        {
            QueryNBE = 1,
            API = 2,
        }

        public enum BaseCodeType
        {
            new_programId_query,
            add_programId_query,
            add_auth_query,
            controller_code,
            service_code,
            implement_code,
            grid_col
        }    
    }
}
