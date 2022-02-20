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
            /// <summary>
            /// 그리드 컬럼 자동생성
            /// </summary>
            grid_col,
            /// <summary>
            /// SearchAreaComponent
            /// </summary>
            js_search_area,
            /// <summary>
            /// ResultAreaComponent
            /// </summary>
            js_result_area,
            /// <summary>
            /// Service
            /// </summary>
            js_service,
            /// <summary>
            /// ContainerComponent
            /// </summary>
            js_container
        }    
    }
}
