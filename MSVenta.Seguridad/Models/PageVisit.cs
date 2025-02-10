using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSVenta.Seguridad.Models
{
    public class PageVisit
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }

        [Required]
        [Column("Page_Name")]
        public string PageName { get; set; }

        [Column("Visit_Count")]
        public int VisitCount { get; set; } = 0;
    }
}
