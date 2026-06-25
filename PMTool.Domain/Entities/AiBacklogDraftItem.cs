namespace PMTool.Domain.Entities;

public class AiBacklogDraftItem
{
    public Guid Id { get; set; }
    public Guid DraftId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; }        // 1=BRD 2=UserStory 3=UseCase 4=Epic 6=Feature 9=Bug
    public int Priority { get; set; }    // 1=Low 2=Medium 3=High 4=Critical
    public int StoryPoints { get; set; }
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; }

    public AiBacklogDraft? Draft { get; set; }
}
