
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stories.Model;
public class Theme
{
    public int Id { get; set; }
    [Column(TypeName = "varchar(64)")]
    public string Name { get; set; }
    [MaxLength(1024)]
    public string Description { get; set; }
    [Column(TypeName = "varchar(64)")]
    private string CssFolder { get; } = "/css/";
    public ICollection<User> Users { get; set; }
}
