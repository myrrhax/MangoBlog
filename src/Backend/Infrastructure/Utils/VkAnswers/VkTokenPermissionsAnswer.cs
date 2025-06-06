namespace Infrastructure.Utils.VkAnswers;

internal class VkTokenPermissionsAnswer
{
    public int Mask { get; set; }
    public IEnumerable<VkPermission> Permissions { get; set; } = [];
}
