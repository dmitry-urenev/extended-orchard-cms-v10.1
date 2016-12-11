using Orchard.Alias.Records;
using Orchard.ContentManagement.Records;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Orchard.StaticPages.Models {
    public class StaticPagePartRecord : ContentPartRecord
    {
        public virtual int RouteId { get; set; }

        /// <summary>
        /// Defines should system or not to use route values to controller action
        /// or simply transfer request to specified static url
        /// </summary>
        public virtual bool IsAction { get; set; }       

        /// <summary>
        /// Target static path.
        /// </summary>
        [StringLength(2048)]
        public virtual string StaticPageUrl { get; set; }        
             
    }    
}
