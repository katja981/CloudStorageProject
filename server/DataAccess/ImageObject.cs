using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess;

public class ImageObject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;

    public string ObjectKey { get; set; } = null!;
    
    public string FileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;
    
    public DateTimeOffset UploadedAt { get; set; }

    public string UploadedByUserId { get; set; } = null!;
}