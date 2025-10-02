using System.Runtime.InteropServices;

namespace Ticketing.Query.Domain.Abstractions;

public abstract class Entity(Guid id)
{
    protected Entity() : this(default)
    {

    }

    public Guid Id { get; set; } = id;
    public DateTime? CreateOn { get; set; }
    public string? CreateBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }     
    public string? LastModifiedBy { get; set; } 

}
