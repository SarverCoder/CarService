using CarService.Domain.Common;
using CarService.Domain.Enums;

namespace CarService.Domain.Entities;

public class AiConversation : BaseEntity
{
    public Guid? UserId { get; set; }
    public AiChannel Channel { get; set; }
    public ICollection<AiMessage> Messages { get; set; } = [];
}

public class AiMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public AiMessageRole Role { get; set; }
    public string Content { get; set; } = null!;
}
